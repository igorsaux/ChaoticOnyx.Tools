using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ChaoticOnyx.Tools.ChangelogGenerator.Parsers
{
	/// <summary>
	///		Поставщик парсеров.
	/// </summary>
	public static class ChangelogParserProvider
	{
		private static readonly Dictionary<ChangelogParserAttribute, Type> s_parsers      = new();
		private static readonly Dictionary<string, ChangelogParser>        s_parsersCache = new();
		
		public static IReadOnlyCollection<ChangelogParserAttribute> Parsers { get; }
		
		/// <summary>
		///		Собирает все парсеры чейнджлогов в сборке.
		/// </summary>
		static ChangelogParserProvider()
		{
			var assembly = Assembly.GetAssembly(typeof(ChangelogParser));
			Debug.Assert(assembly != null);

			var parserTypes = assembly.GetTypes()
									  .Where(t => t.IsAbstract == false && t.IsSubclassOf(typeof(ChangelogParser)))
									  .ToList();

			foreach (var type in parserTypes)
			{
				ChangelogParserAttribute? attribute = type.GetCustomAttribute<ChangelogParserAttribute>();

				if (attribute == null)
				{
					continue;
				}
				
				s_parsers.Add(attribute, type);
			}

			Parsers = s_parsers.Keys.ToList()
							   .AsReadOnly();
		}
		
		/// <summary>
		///		Возвращает парсер для указанного расширения файла (вместе с точкой).
		/// </summary>
		/// <param name="forExtension"></param>
		/// <returns></returns>
		public static ChangelogParser? GetParser(string forExtension)
		{
			foreach (var (extension, parser) in s_parsersCache)
			{
				if (extension == forExtension)
				{
					return parser;
				}
			}
			
			Type? type = null;

			foreach (var (key, value) in s_parsers)
			{
				if (key.Extension == forExtension)
				{
					type = value;

					break;
				}
			}

			if (type == null)
			{
				return null;
			}

			var instance = (ChangelogParser) Activator.CreateInstance(type)!;
			s_parsersCache.Add(forExtension, instance);
			
			return instance;
		}
	}
}
