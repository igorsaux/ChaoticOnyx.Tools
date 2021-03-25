#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.Converters;
using YamlDotNet.Serialization.NamingConventions;

#endregion

namespace ChaoticOnyx.Tools.ChangelogGenerator
{
    public static class Program
    {
        public static ILogger Logger
        {
            get;
            set;
        }
        
        public static Options Options
        {
            get;
            set;
        }

        /// <summary>
        ///     Путь - Класс.
        /// </summary>
        private static Dictionary<string, Changelog> s_changelog;

        private static IDeserializer s_deserializer;
        private static ISerializer   s_serializer;

        /// <summary>
        ///     Для сериализации/десериализации даты.
        /// </summary>
        private static DateTimeConverter s_dateTimeConverter;

        /// <summary>
        ///     Содержимое кэш файла.
        /// </summary>
        private static List<Changelog> s_cache;

        public static int Main(string[] args)
        {
            try
            {
                Configure(args);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                Logger.LogTrace(e, e.Message);

                return -1;
            }

            Logger.LogDebug($"{ChangelogGeneratorResources.DBG_CONFIGURED}");

            try
            {
                ParseChangelogs();
            }
            catch (InvalidOperationException e)
            {
                Logger.LogTrace(e, e.Message);

                return -1;
            }

            if (!s_changelog.Any())
            {
                Logger.LogInformation($"{ChangelogGeneratorResources.NO_NEW_CHANGELOGS}");

                return 0;
            }

            try
            {
                ParseCache();
            }
            catch (FileNotFoundException)
            {
                s_cache = new();
            }

            UpdateCache();

            if (!Options.DryRun && Options.GenerateHtml)
            {
                GenerateHTML();
            }

            if (!Options.DryRun)
            {
                SaveCache();
                DeleteChangelogs();
            }

            return 0;
        }

        private static void SaveCache()
        {
            Logger.LogInformation($"{ChangelogGeneratorResources.SAVING_CACHE}");
            // У DeleteAfter установлен аттрибут скрывающий его во время сериализации при значении равным default
            s_cache.ForEach(e => e.DeleteAfter = default);
            var result = s_serializer.Serialize(s_cache.OrderByDescending(e => e.Date));
            File.WriteAllText(Options.ChangelogCache, result);
        }

        /// <summary>
        ///     Объединяет все изменения одного автора сделанные в один день.
        /// </summary>
        private static void UpdateCache()
        {
            var             totalChangelogs = s_cache.Count;
            List<Changelog> newCache        = new(s_cache.Count);
            newCache.AddRange(s_cache);
            newCache.AddRange(s_changelog.Values);
            s_cache = CacheMerger.Merge(newCache);
            Logger.LogInformation($"{ChangelogGeneratorResources.CHANGELOGS_DELTA} {s_cache.Count - totalChangelogs}");
        }

        /// <summary>
        ///     Генерация HTML чейнджлога.
        /// </summary>
        private static void GenerateHTML()
        {
            Logger.LogInformation($"{ChangelogGeneratorResources.GENERATING_HTML}");

            var builder = new HtmlChangelogBuilder(File.ReadAllText(Options.Template));

            Logger.LogInformation($"{ChangelogGeneratorResources.SAVING_HTML}");
            File.WriteAllText(Options.OutputChangelog, builder.Build(s_cache.OrderByDescending(e => e.Date)));
        }

        /// <summary>
        ///     Парсинг файла указанного в ChangelogCache.
        /// </summary>
        /// <exception cref="YamlException">Ошибка парсинга.</exception>
        private static void ParseCache()
        {
            var parser = new CacheParser(s_deserializer, Logger, Options.AutoConvert);
            s_cache = parser.ParseCacheFile(Options.ChangelogCache);
        }

        /// <summary>
        ///     Парсинг .yml файлов по пути указанному в ChangelogsFolder.
        /// </summary>
        public static void ParseChangelogs()
        {
            ChangelogParser parser = new(s_deserializer, s_serializer, Logger, Options.AutoConvert);
            s_changelog = parser.ParseFromFolder(Options.ChangelogsFolder);

            foreach (var (key, value) in s_changelog)
            {
                if (string.IsNullOrEmpty(value.Author))
                {
                    var msg = $"{ChangelogGeneratorResources.AUTHOR_NOT_SPECIFIED}";
                    Logger.LogError(msg);

                    throw new InvalidOperationException(msg);
                }

                foreach (var change in value.Changes)
                {
                    if (!Options.ValidPrefixes.Any(p => p == change.Prefix))
                    {
                        var msg = $"{ChangelogGeneratorResources.INVALID_PREFIX} `{change.Prefix}` ({key})";
                        Logger.LogError(msg);

                        throw new InvalidOperationException(msg);
                    }
                }
            }
        }

        /// <summary>
        ///     Удаление чейнджлогов помеченных deleteAfter: true при DryRun = false.
        /// </summary>
        private static void DeleteChangelogs()
        {
            if (Options.DryRun)
            {
                return;
            }

            foreach (var (path, changelog) in s_changelog)
            {
                if (!changelog.DeleteAfter)
                {
                    continue;
                }

                Logger.LogInformation($"{ChangelogGeneratorResources.DELETING_FILE} {Path.GetFileName(path)}");
                File.Delete(path);
            }
        }

        /// <summary>
        ///     Конфигурация программы.
        /// </summary>
        /// <param name="args"></param>
        private static void Configure(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                             .UseContentRoot(AppContext.BaseDirectory)
                             .ConfigureAppConfiguration(
                                 b => b.AddJsonFile(
                                     $"{Directory.GetCurrentDirectory()}/ChaoticOnyx.Tools.ChangelogGenerator.json",
                                     true))
                             .Build();

            var loggerFactory = host.Services.GetService<ILoggerFactory>();
            var configuration = host.Services.GetService<IConfiguration>();
            Debug.Assert(loggerFactory is not null);
            Debug.Assert(configuration is not null);
            Logger  = loggerFactory.CreateLogger("ChaoticOnyx.ChangelogGenerator");
            Options = new();

            configuration.GetSection("Options")
                         .Bind(Options);

            Options.Validate(Logger);
            List<string> formats = new();
            formats.AddRange(CultureInfo.InvariantCulture.DateTimeFormat.GetAllDateTimePatterns());
            formats.AddRange(new CultureInfo(Options.DateCulture).DateTimeFormat.GetAllDateTimePatterns());
            s_dateTimeConverter = new(DateTimeKind.Local, new CultureInfo(Options.DateCulture), formats.ToArray());

            s_deserializer = new DeserializerBuilder().WithTypeConverter(s_dateTimeConverter)
                                                      .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                                      .Build();

            s_serializer = new SerializerBuilder().WithTypeConverter(s_dateTimeConverter)
                                                  .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                                  .Build();
        }
    }
}
