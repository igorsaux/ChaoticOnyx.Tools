using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace ChaoticOnyx.Tools.ChangelogGenerator
{
	/// <summary>
	///     Настройки генератор чейнджлогов.
	/// </summary>
	public sealed record ChangelogSettings
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

			string basePath = Directory.GetCurrentDirectory();

			return settings with
			{
				ChangelogsFolder = Path.GetFullPath(settings.ChangelogsFolder, basePath),
				ChangelogsCache = Path.GetFullPath(settings.ChangelogsCache, basePath),
				HtmlChangelog = Path.GetFullPath(settings.HtmlChangelog, basePath),
				HtmlTemplate = Path.GetFullPath(settings.HtmlTemplate, basePath)
			};
		}
	}
}
