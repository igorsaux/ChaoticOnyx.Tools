#region

using System;
using ChaoticOnyx.Tools.Core.resources;

#endregion

namespace ChaoticOnyx.Tools.Core
{
    public static class Program
    {
        private static readonly string[] s_commands =
        {
            "changelog", "help"
        };

        private static void PrintAvaileCommands()
        {
            Console.WriteLine($"{CoreResources.AVAILABLE_COMMAND}");

            foreach (var tool in s_commands)
            {
                Console.WriteLine($"  {tool}");
            }
        }

        public static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintAvaileCommands();

                return -1;
            }

            var input = args[0]
                .ToLower();

            switch (input)
            {
                case "changelog":
                    return ChangelogGenerator.Program.Main(args[1..]);
                case "help":
                    Console.WriteLine(CoreResources.HELP);

                    return 0;
                default:
                    Console.WriteLine($"{CoreResources.UNKNOWN_COMMAND} `{args[0]}`");
                    PrintAvaileCommands();

                    return -1;
            }
        }
    }
}
