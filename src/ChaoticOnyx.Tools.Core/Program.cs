using System;
using ChaoticOnyx.Tools.Core.resources;

namespace ChaoticOnyx.Tools.Core
{
	public static class Program
	{
		private static void PrintAvailableTools()
		{
			Console.WriteLine($"{CoreResources.AVAILABLE_COMMAND}");

			foreach (var tool in ToolProvider.Tools)
			{
				string? description = CoreResources.ResourceManager.GetString(tool.Description);
				Console.WriteLine($"	{tool.RunCommand} - {description}");
			}
		}

		public static int Main(string[] args)
		{
			if (args.Length == 0)
			{
				PrintAvailableTools();

				return -1;
			}

			string         command = args[0];
			ToolAttribute? tool    = ToolProvider.GetTool(command);

			if (tool is null)
			{
				Console.WriteLine($"{CoreResources.UNKNOWN_COMMAND} `{command}`");
				PrintAvailableTools();

				return -1;
			}

			return tool.Run(args[1..]);
		}
	}
}
