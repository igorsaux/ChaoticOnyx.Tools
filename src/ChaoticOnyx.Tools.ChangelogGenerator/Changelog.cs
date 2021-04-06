using System;
using System.Collections.Generic;
using System.Linq;

namespace ChaoticOnyx.Tools.ChangelogGenerator
{
	public sealed record Changelog
	{
		/// <summary>
		///     Автор изменений.
		/// </summary>
		public string Author { get; init; }

		/// <summary>
		///     Дата изменении.
		/// </summary>
		public DateTime Date { get; init; }

		/// <summary>
		///     Изменения.
		/// </summary>
		public IEnumerable<Change> Changes { get; init; }

		public Changelog()
		{
			Author  = "Unknown";
			Date    = DateTime.Now;
			Changes = new List<Change>();
		}

		public Changelog(string author, DateTime date, List<Change> changes)
		{
			Author  = author;
			Date    = date;
			Changes = changes;
		}

		public static ICollection<Changelog> Merge(ICollection<Changelog> changelogs)
		{
			List<Changelog> result = new();

			foreach (var changelog in changelogs)
			{
				if (result.Any(e => e.Author == changelog.Author && e.Date.Date == changelog.Date.Date)) { continue; }

				HashSet<Change> changelogChanges = changelogs
												   .Where(e => e.Author == changelog.Author
															   && e.Date.Date == changelog.Date.Date)
												   .SelectMany(e => e.Changes)
												   .ToHashSet();

				result.Add(new Changelog
				{
					Author = changelog.Author, Date = changelog.Date, Changes = changelogChanges
				});
			}

			return result;
		}
	}

	public sealed record Change
	{
		/// <summary>
		///     Префикс изменения.
		/// </summary>
		public string Prefix { get; init; }

		/// <summary>
		///     Описание изменения.
		/// </summary>
		public string Message { get; init; }

		public Change(string prefix, string message)
		{
			Prefix  = prefix;
			Message = message;
		}
	}
}
