using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChaoticOnyx.Tools.Core.resources;
using Scriban;
using Console = ChaoticOnyx.Tools.Utils.Console;

namespace ChaoticOnyx.Tools.ChangelogGenerator
{
	public class Program
	{
		private readonly ChangelogSettings _settings;

		public Program(ChangelogSettings settings) { _settings = settings; }

		private static void DeleteFiles(ICollection<string> files)
		{
			foreach (var file in files) { File.Delete(file); }
		}

		public int Run()
		{
			IDictionary<string, Changelog> fileToChangelog = _settings.ChangelogsFolder.ParseChangelogs();
			ICollection<string>            parsedFiles     = fileToChangelog.Keys;
			ICollection<Changelog>         changelogs      = Changelog.Merge(fileToChangelog.Values);

			if (!changelogs.Any())
			{
				Console.PrintInfo($"{ChangelogGeneratorResources.NO_NEW_CHANGELOGS}");

				return 0;
			}

			if (!ValidatePrefixes(changelogs)) { return -1; }

			Console.PrintInfo($"{ChangelogGeneratorResources.NEW_CHANGELOGS} {changelogs.Count}");

			List<Changelog> cache = _settings.ChangelogsCache.ParseChangelogsCache()
											 .ToList();

			int originalCacheSize = cache.Count;
			cache.AddRange(changelogs);

			ICollection<Changelog> resultCache = Changelog.Merge(cache)
														  .OrderByDescending(c => c.Date)
														  .ToList();

			Console.PrintInfo(
				$"{ChangelogGeneratorResources.CHANGELOGS_DELTA} {resultCache.Count - originalCacheSize}");

			if (!_settings.DryRun)
			{
				if (_settings.GenerateHtml)
				{
					Console.PrintInfo($"{ChangelogGeneratorResources.SAVING_HTML} {_settings.HtmlChangelog}");
					SaveHtml(resultCache);
				}

				_settings.ChangelogsCache.SaveCache(resultCache);
				DeleteFiles(parsedFiles);
			}

			return 0;
		}

		private void SaveHtml(ICollection<Changelog> changelogs)
		{
			Template template = Template.Parse(File.ReadAllText(_settings.HtmlTemplate));
			var      context  = new { GeneratingTime = DateTime.Now, Changelogs = changelogs };
			File.WriteAllText(_settings.HtmlChangelog, template.Render(context));
		}

		private bool ValidatePrefixes(ICollection<Changelog> changelogs)
		{
			var fail = false;
		
			foreach (Change change in from changelog in changelogs
									  from change in changelog.Changes
									  where !_settings.ValidPrefixes.Contains(change.Prefix)
									  select change)
			{
				Console.PrintError($"{ChangelogGeneratorResources.INVALID_PREFIX} {change.Prefix}");
				fail = true;
			}

			return !fail;
		}
	}
}
