using System;
using Shared.Logging;

namespace Shared.AppStartup.CommandLine
{
    public static class CommandLineArgs
    {
        public const string Separator = "=";
        public static bool AsServer => ArgsS != null;
        public static ArgsS ArgsS { get; private set; }

        static CommandLineArgs()
        {
            InitCommandLine();
        }

        private static void InitCommandLine()
        {
            var args = Environment.GetCommandLineArgs();
            foreach (var arg in args)
            {
                var nameValuePair = arg.Split(Separator);
                if (nameValuePair.Length != 2)
                {
                    continue;
                }
                try
                {
                    switch (nameValuePair[0])
                    {
                        case "as-server":
                            var asServer = bool.Parse(nameValuePair[1]);
                            if (asServer)
                            {
                                ArgsS = new ArgsS(args);
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    Log.Err(nameof(CommandLineArgs), e.Message);
                }
            }
        }
    }
}