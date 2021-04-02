using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChaoticOnyx.Tools.ChangelogGenerator;
using ChaoticOnyx.Tools.Core.resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Scriban;

namespace ChaoticOnyx.Tools.Core
{
	[Tool("CHANGELOG_DESCRIPTION", "changelog")]
	public sealed class ChangelogGenerator : Tool
	{
		private readonly ILogger           _logger;
		private readonly ChangelogSettings _settings;

		public ChangelogGenerator(ILogger logger, IConfiguration configuration) : base(logger, configuration)
		{
			_logger   = logger;
			_settings = ChangelogSettings.FromConfiguration(configuration);
		}

		public override int Run()
		{
			try
			{
				return RunInternal();
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message);
				_logger.LogTrace(e, e.Message);

				return -1;
			}
		}

		private int RunInternal()
		{
			IDictionary<string, Changelog> fileToChangelog = _settings.ChangelogsFolder.ParseChangelogs(_logger);
			ICollection<string>            parsedFiles     = fileToChangelog.Keys;
			ICollection<Changelog>         changelogs      = Changelog.Merge(fileToChangelog.Values);

			if (!changelogs.Any())
			{
				_logger.LogInformation($"{ChangelogGeneratorResources.NO_NEW_CHANGELOGS}");

				return 0;
			}

			if (ValidatePrefixes(changelogs) == false)
			{
				return -1;
			}
			
			_logger.LogInformation($"{ChangelogGeneratorResources.NEW_CHANGELOGS} {changelogs.Count}");
			List<Changelog> cache             = _settings.ChangelogsCache.ParseChangelogsCache(_logger).ToList();
			var             originalCacheSize = cache.Count;
			cache.AddRange(changelogs);
			ICollection<Changelog> resultCache = Changelog.Merge(cache).OrderByDescending(c => c.Date).ToList();
			_logger.LogInformation($"{ChangelogGeneratorResources.CHANGELOGS_DELTA} {resultCache.Count - originalCacheSize}");
			
			if (!_settings.DryRun)
			{
				if (_settings.GenerateHtml)
				{
					_logger.LogInformation($"{ChangelogGeneratorResources.SAVING_HTML} {_settings.HtmlChangelog}");
					SaveHtml(resultCache);
				}

				_settings.ChangelogsCache.SaveCache(resultCache, _logger);
				DeleteFiles(parsedFiles);
			}

			return 0;
		}

		private void SaveHtml(ICollection<Changelog> changelogs)
		{
			Template template = Template.Parse(File.ReadAllText(_settings.HtmlTemplate));

			var context = new
			{
				GeneratingTime = DateTime.Now,
				Changelogs = changelogs
			};
			
			File.WriteAllText(_settings.HtmlChangelog, template.Render(context));
		}
		
		private bool ValidatePrefixes(ICollection<Changelog> changelogs)
		{
			foreach (Change change in from changelog in changelogs
									  from change in changelog.Changes
									  where !_settings.ValidPrefixes.Contains(change.Prefix)
									  select change)
			{
				_logger.LogError($"{ChangelogGeneratorResources.INVALID_PREFIX} {change.Prefix}");
						
				return false;
			}

			return true;
		}

		private void DeleteFiles(ICollection<string> files)
		{
			foreach (var file in files)
			{
				File.Delete(file);
			}
		}

		/// <summary>
		///     Настройки генератор чейнджлогов.
		/// </summary>
		private sealed record ChangelogSettings
		{
			/// <summary>
			///     Папка с чейнджлогами.
			/// </summary>
			public string ChangelogsFolder { get; init; } = string.Empty;

			/// <summary>
			///     Путь до кэша чейнджлогов.
			/// </summary>
			public string ChangelogsCache { get; init; } = string.Empty;

			/// <summary>
			///		
			/// </summary>
			public string OutputChangelogsCache { get; init; } = string.Empty;

			/// <summary>
			///     Путь до генерации файла с HTML чейнджлога, включает имя файла и расширение.
			/// </summary>
			public string HtmlChangelog { get; init; } = string.Empty;

			/// <summary>
			///     Холостой запуск. Выполняет все действия но не сохраняет и не удаляет файлы.
			/// </summary>
			public bool DryRun { get; init; }

			/// <summary>
			///     Генерация HTML чейнджлога.
			/// </summary>
			public bool GenerateHtml { get; init; } = true;

			/// <summary>
			///     Путь до HTML шаблона.
			/// </summary>
			public string HtmlTemplate { get; init; } = string.Empty;

			/// <summary>
			///     Допустимые префиксы в чейнджлогах.
			/// </summary>
			public string[] ValidPrefixes { get; init; } = Array.Empty<string>();

			public static ChangelogSettings FromConfiguration(IConfiguration configuration)
			{
				ChangelogSettings settings = new();

				configuration.GetSection("Settings")
							 .Bind(settings);

				string dir = Directory.GetCurrentDirectory();

				return settings with
				{
					ChangelogsFolder = Path.GetFullPath(settings.ChangelogsFolder, dir),
					ChangelogsCache = Path.GetFullPath(settings.ChangelogsCache, dir),
					HtmlChangelog = Path.GetFullPath(settings.HtmlChangelog, dir),
					HtmlTemplate = Path.GetFullPath(settings.HtmlTemplate, dir)
				};
			}
		}
	}
}
