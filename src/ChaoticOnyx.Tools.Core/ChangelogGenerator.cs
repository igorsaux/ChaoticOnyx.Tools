using System;
using ChaoticOnyx.Tools.ChangelogGenerator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Console = ChaoticOnyx.Tools.Utils.Console;

namespace ChaoticOnyx.Tools.Core
{
	[Tool("CHANGELOG_DESCRIPTION", "changelog")]
	public sealed class ChangelogGenerator : Tool
	{
		private readonly ChangelogSettings _settings = new();

		protected override void OnConfigured(IHost host, IConfiguration configuration)
		{
			configuration.GetSection("Settings")
						 .Bind(_settings);
		}

		public override int Run()
		{
			var program = new Tools.ChangelogGenerator.Program(_settings);

			try { return program.Run(); }
			catch (Exception e)
			{
				Console.PrintException(e);

				return -1;
			}
		}
	}
}
