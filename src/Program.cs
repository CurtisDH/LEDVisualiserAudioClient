using System.Drawing;
using AudioClient.Client;

namespace AudioClient
{
    internal static class Program
    {
        public static bool Debug = false;

        private static async Task Main(string[] args)
        {
            // TODO move this to a json config so the default can be automatically run
            double threshold = 75;
            string ip = "192.168.1.11";
            int port = 5555;
            int stripSize = 150;
            int speed = 15;

            // TODO make this reflect the change in the switch statement below
            string[] runtimeArgs = new[] { "-threshold", "-ip", "-port", "-strip", "-speed" };
            Console.WriteLine("Available runtime args:");
            foreach (var arg in runtimeArgs)
            {
                Console.WriteLine(arg);
            }

            if (args.Length > 1)
            {
                Console.WriteLine($"Runtime arguments found attempting to process..");
                for (var i = 0; i < args.Length; i++)
                {
                    if (args.Length <= i + 1)
                    {
                        continue;
                    }

                    switch (args[i])
                    {
                        case "-threshold":
                            double.TryParse(args[i + 1], out threshold);
                            Console.WriteLine($"Volume threshold set to:{threshold}");
                            break;
                        case "-ip":
                            ip = args[i + 1];
                            Console.WriteLine($"Target IP Address set to:{ip}");

                            break;
                        case "-port":
                            int.TryParse(args[i + 1], out port);
                            Console.WriteLine($"Target port set to:{port}");
                            break;
                        case "-strip":
                            int.TryParse(args[i + 1], out stripSize);
                            Console.WriteLine($"LED Strip Size set to:{stripSize}");
                            break;
                        case "-speed":
                            int.TryParse(args[i + 1], out speed);
                            Console.WriteLine($"Update speed (in ms) set to:{speed}");

                            break;
                    }
                }
            }

            var ac = new AudClient(threshold, ip, port, stripSize, speed);
            Console.WriteLine("Starting Audio Client");
            await ac.Init();
        }
    }
}