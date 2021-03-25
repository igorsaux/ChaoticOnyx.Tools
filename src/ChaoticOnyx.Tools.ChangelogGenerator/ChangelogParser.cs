#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

#endregion

namespace ChaoticOnyx.Tools.ChangelogGenerator
{
    public class ChangelogParser
    {
        /// <summary>
        ///     Список конвертеров чейнджлогов.
        /// </summary>
        private static readonly Func<IDeserializer, string, Changelog>[] s_converters =
        {
            ChangelogFormatConverter.VgToChaoticOnyx
        };

        private readonly IDeserializer _deserializer;
        private readonly bool          _doConvert;
        private readonly ILogger?      _logger;
        private readonly ISerializer   _serializer;

        public ChangelogParser(IDeserializer deserializer, ISerializer serializer, bool doConvert = false)
        {
            _deserializer = deserializer;
            _serializer   = serializer;
            _doConvert    = doConvert;
        }

        public ChangelogParser(IDeserializer deserializer,
                               ISerializer   serializer,
                               ILogger       logger,
                               bool          doConvert = false) : this(deserializer, serializer, doConvert)
        {
            _logger = logger;
        }

        /// <summary>
        ///     Парсинг .yml файлов в папке. Файлы которые начинаются с точки - не парсятся.
        /// </summary>
        /// <exception cref="YamlException"></exception>
        public Dictionary<string, Changelog> ParseFromFolder(string fullPath)
        {
            _logger?.LogInformation($"{ChangelogGeneratorResources.SEARCHING_CHANGELOGS_IN} {fullPath}");

            var files = from f in Directory.GetFiles(fullPath)
                        where Path.GetExtension(f) == ".yml" && Path.GetFileName(f)[0] != '.'
                        select f;

            Dictionary<string, Changelog> result = new();

            foreach (var file in files)
            {
                try
                {
                    var c = ParseFromFile(file);

                    if (c.Date == default)
                    {
                        c.Date = DateTime.Now.Date;
                    }

                    result.Add(file, c);
                }
                catch (YamlException e)
                {
                    _logger?.LogError(
                        $"{ChangelogGeneratorResources.PARSING_ERROR} {Path.GetFileName(file)}\n{e.InnerException?.Message}");

                    _logger?.LogTrace(e, e.Message);

                    throw;
                }
            }

            return result;
        }

        /// <summary>
        ///     Парсинг файла.
        /// </summary>
        /// <param name="fullPath">Абсолютный путь до файла.</param>
        /// <exception cref="YamlException"></exception>
        public Changelog ParseFromFile(string fullPath)
        {
            _logger?.LogInformation($"{ChangelogGeneratorResources.PARSING_FILE} {Path.GetFileName(fullPath)}");

            return ParseFromText(File.ReadAllText(fullPath));
        }

        /// <summary>
        ///     Парсинг текста.
        /// </summary>
        /// <exception cref="YamlException"></exception>
        public Changelog ParseFromText(string text)
        {
            Changelog result;

            try
            {
                result = _deserializer.Deserialize<Changelog>(text);
            }
            catch (YamlException e)
            {
                _logger?.LogError(
                    $"{ChangelogGeneratorResources.PARSING_ERROR} {e.InnerException?.Message ?? e.Message}");

                _logger?.LogTrace(e, e.Message);

                if (!_doConvert)
                {
                    throw;
                }

                foreach (var converter in s_converters)
                {
                    _logger?.LogWarning($"{ChangelogGeneratorResources.TRYING_TO_CONVERT} {{{converter.Method.Name}}}");

                    try
                    {
                        result = converter.Invoke(_deserializer, text);
                        _logger?.LogInformation($"{ChangelogGeneratorResources.FILE_CONVERTED}");

                        return result;
                    }
                    catch (YamlException) { }
                }

                _logger?.LogError($"{ChangelogGeneratorResources.CANT_CONVERT_FILE}");

                throw;
            }

            return result;
        }

        /// <summary>
        ///     Сохранить чейнджлоги в файлы.
        /// </summary>
        /// <param name="fileToChangelog">Словарь путь-чейнджлог.</param>
        public void SaveChangelogs(Dictionary<string, Changelog> fileToChangelog)
        {
            _logger?.LogInformation($"{ChangelogGeneratorResources.SAVING_CHANGELOGS}");

            foreach (var (file, changelog) in fileToChangelog)
            {
                var result = _serializer.Serialize(changelog);
                File.WriteAllText(file, result);
                _logger?.LogInformation($"{ChangelogGeneratorResources.CHANGELOG_SAVED} {file}");
            }
        }
    }
}
