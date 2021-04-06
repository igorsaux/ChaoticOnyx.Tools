using System.Collections.Generic;

namespace ChaoticOnyx.Tools.ChangelogGenerator.Parsers
{
	public abstract class ChangelogParser
	{
		/// <summary>
		///     Парсинг чейнджлога из текста.
		/// </summary>
		/// <param name="text">Текст.</param>
		/// <returns></returns>
		public abstract Changelog Parse(string text);

		/// <summary>
		///     Парсинг кэша чейнджлога из текста.
		/// </summary>
		/// <param name="text">Текст.</param>
		/// <returns></returns>
		public abstract ICollection<Changelog> ParseCache(string text);

		/// <summary>
		///     Сериализует кэш.
		/// </summary>
		/// <param name="cache">Кэш.</param>
		/// <returns>Кэш в сериализованном виде.</returns>
		public abstract string SerializeCache(ICollection<Changelog> cache);
	}
}
