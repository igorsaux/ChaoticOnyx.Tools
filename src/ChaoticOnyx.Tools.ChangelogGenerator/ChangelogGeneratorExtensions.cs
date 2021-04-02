using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using ChaoticOnyx.Tools.ChangelogGenerator.Parsers;
using ChaoticOnyx.Tools.Core.resources;
using Microsoft.Extensions.Logging;
using YamlDotNet.Core;

namespace ChaoticOnyx.Tools.ChangelogGenerator
{
	public static class ChangelogGeneratorExtensions
	{
		/// <summary>
		///		Парсинг чейнджлогов в директории. Игнорируются файлы начинающиеся с точки.
		/// </summary>
		/// <param name="folderPath">Путь до директории.</param>
		/// <param name="logger"></param>
		/// <returns></returns>
		public static IDictionary<string, Changelog> ParseChangelogs(this string folderPath, ILogger? logger = null)
		{
			Dictionary<string, Changelog> changelogs = new();

			IEnumerable<string> files = from f in Directory.GetFiles(folderPath)
										where Path.GetFileName(f)[0] != '.'
										select f;

			foreach (var file in files)
			{
				string           extension = Path.GetExtension(file);
				ChangelogParser? parser    = ChangelogParserProvider.GetParser(extension);

				if (parser == null)
				{
					continue;
				}

				Changelog? changelog = null;

				try
				{
					logger.LogInformation($"{ChangelogGeneratorResources.PARSING_FILE} {file}");
					changelog = parser.Parse(File.ReadAllText(file));
				}
				catch (Exception e)
				{
					logger.LogWarning($"{ChangelogGeneratorResources.PARSING_ERROR} {e.InnerException?.Message}");
					logger.LogTrace(e, e.Message);

					continue;
				}

				changelogs.Add(file, changelog);
			}

			return changelogs;
		}

		public static ICollection<Changelog> ParseChangelogsCache(this string filePath, ILogger? logger = null)
		{
			ChangelogParser?       parser = ChangelogParserProvider.GetParser(Path.GetExtension(filePath));
			ICollection<Changelog> cache;
			
			if (parser == null)
			{
				throw new InvalidOperationException($"{filePath} is not convertible!");
			}

			try
			{
				logger.LogInformation($"{ChangelogGeneratorResources.PARSING_CACHE_FILE} {filePath}");
				cache = parser.ParseCache(File.ReadAllText(filePath));
			}
			catch (Exception e)
			{
				logger.LogWarning($"{ChangelogGeneratorResources.PARSING_ERROR} {e.InnerException?.Message}");
				logger.LogTrace(e, e.Message);

				throw;
			}

			return cache;
		}

		public static void SaveCache(this string filePath, ICollection<Changelog> cache, ILogger? logger = null)
		{
			ChangelogParser? parser = ChangelogParserProvider.GetParser(Path.GetExtension(filePath));

			if (parser == null)
			{
				throw new InvalidOperationException($"{filePath} is not convertible!");
			}
			
			logger.LogInformation($"{ChangelogGeneratorResources.SAVING_CACHE}");
			string result = parser.SerializeCache(cache);
			File.WriteAllText(filePath, result);
		}
	}
}
