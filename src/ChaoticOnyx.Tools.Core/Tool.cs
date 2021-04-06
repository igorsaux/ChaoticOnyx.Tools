using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChaoticOnyx.Tools.Core
{
	/// <summary>
	///     Абстрактный класс представляющий запускаемый инструмент.
	/// </summary>
	public abstract class Tool
	{
		public abstract int Run();

		protected abstract void OnConfigured(IHost host, IConfiguration configuration);

		public void Configure(string[]? args = null)
		{
			string toolName = GetType()
				.Name;

			Console.WriteLine(GetType()
								  .Name);

			var builder = new HostBuilder();

			builder.ConfigureAppConfiguration((_, config) =>
			{
				var    configFile          = $"{toolName}.json";
				string appFolderConfig     = Path.GetFullPath(configFile, AppContext.BaseDirectory);
				string workingFolderConfig = Path.GetFullPath(configFile, Directory.GetCurrentDirectory());
				config.AddJsonFile(appFolderConfig, true);
				config.AddJsonFile(workingFolderConfig, true);
				config.AddEnvironmentVariables();

				if (args != null) { config.AddCommandLine(args); }
			});

			IHost host          = builder.Build();
			var   configuration = host.Services.GetService<IConfiguration>();
			Debug.Assert(configuration != null);
			OnConfigured(host, configuration);
		}
	}
}
