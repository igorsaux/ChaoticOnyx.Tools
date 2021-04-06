using System;
using Spectre.Console;

namespace ChaoticOnyx.Tools.Utils
{
	public static class Console
	{
		public static void PrintException(Exception exception)
		{
			AnsiConsole.WriteException(exception, ExceptionFormats.ShortenEverything);
		}

		public static void PrintInfo(string message) { AnsiConsole.MarkupLine($"[grey]info:[/] {message}"); }

		public static void PrintError(string message) { AnsiConsole.MarkupLine($"[red bold]error:[/] {message}"); }
	}
}
