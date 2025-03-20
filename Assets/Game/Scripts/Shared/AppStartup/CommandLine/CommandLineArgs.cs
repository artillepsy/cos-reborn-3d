using System;

namespace Game.Scripts.Shared.AppStartup.CommandLine
{
    public static class CommandLineArgs
    {
        public static bool AsServer { get; private set; }
        public static string Secret { get; private set; }

        static CommandLineArgs()
        {
            InitCommandLine();
        }

        private static void InitCommandLine()
        {
            var args = Environment.GetCommandLineArgs();
            foreach (var arg in args)
            {
                var nameValuePair = arg.Split("=");
                if (nameValuePair.Length != 2)
                {
                    continue;
                }

                try
                {
                    switch (nameValuePair[0])
                    {
                        case "as-server":
                            AsServer = bool.Parse(nameValuePair[1]);
                            break;
                        case "secret":
                            Secret = nameValuePair[1];
                            break;
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
    }
}