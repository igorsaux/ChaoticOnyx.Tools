using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChaoticOnyx.Tools.ChangelogGenerator.Parsers;
using ChaoticOnyx.Tools.Core.resources;
using Spectre.Console;
using Console = ChaoticOnyx.Tools.Utils.Console;

namespace ChaoticOnyx.Tools.ChangelogGenerator
{
	internal static class ChangelogGeneratorExtensions
	{
		/// <summary>
		///     Парсинг чейнджлогов в директории. Игнорируются файлы начинающиеся с точки.
		/// </summary>
		/// <param name="folderPath">Путь до директории.</param>
		/// <returns></returns>
		public static IDictionary<string, Changelog> ParseChangelogs(this string folderPath)
		{
			IDictionary<string, Changelog>? changelogs = null;

			AnsiConsole.Status()
					   .Start($"{ChangelogGeneratorResources.SEARCHING_CHANGELOGS_IN} {folderPath}", ctx =>
					   {
						   ctx.Spinner(Spinner.Known.Line);

						   IEnumerable<string> files = from f in Directory.GetFiles(folderPath)
													   where Path.GetFileName(f)[0] != '.'
													   select f;

						   changelogs = ParseChangelogsInternal(files, ctx);
					   });

			return changelogs!;
		}

		private static IDictionary<string, Changelog> ParseChangelogsInternal(
			IEnumerable<string> files,
			StatusContext       context)
		{
			var changelogs = new Dictionary<string, Changelog>();

			foreach (var file in files)
			{
				string           extension = Path.GetExtension(file);
				ChangelogParser? parser    = ChangelogParserProvider.GetParser(extension);

				if (parser == null) { continue; }

				Changelog changelog;

				try
				{
					context.Status($"{ChangelogGeneratorResources.PARSING_FILE} {file}");
					changelog = parser.Parse(File.ReadAllText(file));
					Console.PrintInfo($"{ChangelogGeneratorResources.FILE_PARSED} {file}");
				}
				catch (Exception e)
				{
					Console.PrintError($"{ChangelogGeneratorResources.PARSING_ERROR} {e.InnerException?.Message}");
					Console.PrintException(e);

					continue;
				}

				changelogs.Add(file, changelog);
			}

			return changelogs;
		}

		public static ICollection<Changelog> ParseChangelogsCache(this string filePath)
		{
			ChangelogParser?       parser = ChangelogParserProvider.GetParser(Path.GetExtension(filePath));
			ICollection<Changelog> cache;

			if (parser == null) { throw new InvalidOperationException($"{filePath} is not convertible!"); }

			try
			{
				Console.PrintInfo($"{ChangelogGeneratorResources.PARSING_CACHE_FILE} {filePath}");
				cache = parser.ParseCache(File.ReadAllText(filePath));
			}
			catch (Exception e)
			{
				Console.PrintError($"{ChangelogGeneratorResources.PARSING_ERROR} {e.InnerException?.Message}");
				Console.PrintException(e);

				throw;
			}

			return cache;
		}

		public static void SaveCache(this string filePath, ICollection<Changelog> cache)
		{
			ChangelogParser? parser = ChangelogParserProvider.GetParser(Path.GetExtension(filePath));

			if (parser == null) { throw new InvalidOperationException($"{filePath} is not convertible!"); }

			Console.PrintInfo($"{ChangelogGeneratorResources.SAVING_CACHE}");
			string result = parser.SerializeCache(cache);
			File.WriteAllText(filePath, result);
		}
	}
}
