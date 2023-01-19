using System.Drawing;
using AudioClient.Client;

namespace AudioClient
{
    internal static class Program
    {
        public static bool debug = false;
        private static double defaultThreshold = 75; // this is kinda like volume

        private static async Task Main(string[] args)
        {
            if (args.Contains("debug"))
            {
                debug = true;
                Console.WriteLine("Debug mode enabled. Parsing additional arguments");
                Console.WriteLine("Running tests");
                var Led = new LedStrip(150);
                Led.IncrementStrip(Color.Red);
                Led.IncrementStrip(Color.FromArgb(0, 255, 0));
                Led.IncrementStrip(Color.Blue);
                Led.IncrementStrip(Color.Crimson);
                Led.ConvertByteArrayToColorArray(Led.GetByteArray());
            }

            if (args.Length < 1)
            {
                Console.WriteLine("No launch arguments were found. Run the default config? (y/n)");
                var response = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(response) || response.ToLower()[0] == 'y')
                {
                    string ip = "192.168.1.11";
                    Console.WriteLine($"###WARNING###");
                    Console.WriteLine(
                        $"No arguments provided Starting audio client with target IP address set to:\n{ip}" +
                        $"\n" +
                        $"Threshold set to: {defaultThreshold}");
                    Console.WriteLine($"###WARNING###");

                    var audioAudClient = new AudClient(defaultThreshold, ip, 5555);
                    await audioAudClient.Init();
                    return;
                }

                Console.WriteLine("'n' Selected. Please enter the desired program type (Server/Client)");
                var res = Console.ReadLine().ToLower();
                switch (res)
                {
                    case "server":
                        Console.WriteLine("Server was selected. Provide the port");
                        Console.Write(">");
                        int.TryParse(Console.ReadLine(), out int port);
                        Console.WriteLine($"Port set to {port}");
                        Console.WriteLine("dataSize (EXPECTED BYTE ARRAY SIZE):");
                        Console.Write(">");
                        var dataSize = int.Parse(Console.ReadLine());
                        Console.WriteLine($"DataSize set to {dataSize} bytes");
                        Console.WriteLine("Enter number of LEDS in your strip (Expected int)");
                        Console.WriteLine("Attempting to Start server...");
                        var networkServer = new NetworkServer(port, dataSize);
                        networkServer.Listen();
                        break;
                    case "client":
                        Console.WriteLine("Client selected. Enter the target magnitude");
                        float.TryParse(Console.ReadLine(), out float threshold);
                        Console.WriteLine("Provide the target IP address");
                        var targetIP = Console.ReadLine();
                        Console.WriteLine("Provide the target port");
                        port = int.Parse(Console.ReadLine());
                        Console.WriteLine("Attempting to start client...");
                        var audioAudClient = new AudClient(threshold, targetIP, port);
                        audioAudClient.Init();

                        break;
                    default:
                        Console.WriteLine("invalid response, exiting.");
                        return;
                }
            }

            if (args[0].ToLower() == "server")
            {
                Console.WriteLine("Starting as server");
                Console.WriteLine("Attempting to read additional arguments:");
                Console.WriteLine("PORT:");
                var port = int.Parse(args[1]);
                Console.WriteLine($"Port set to {port}");
                Console.WriteLine("dataSize (EXPECTED BYTE ARRAY SIZE):");
                var dataSize = int.Parse(args[2]);
                Console.WriteLine($"DataSize set to {dataSize} bytes");
                Console.WriteLine("Starting server...");
                var networkServer = new NetworkServer(port, dataSize);
                networkServer.Listen();
            }
            else if (args[0].ToLower() == "client")
            {
                Console.WriteLine("Starting as client");
                Console.WriteLine("Attempting to read additional arguments:");
                Console.WriteLine("Magnitude threshold: (float value expected)");
                var threshold = float.Parse(args[1]);
                Console.WriteLine($"Magnitude threshold set to {threshold}");
                Console.WriteLine("TargetIP: (string value expected)");
                var targetIP = (args[2]);
                Console.WriteLine($"TargetIP set to: {targetIP}");
                Console.WriteLine($"Port: (int value expected)");
                var port = int.Parse(args[3]);
                Console.WriteLine($"Port set to: {port})");
                Console.WriteLine("Starting audio client...");
                var audioAudClient = new AudClient(threshold, targetIP, port);
                audioAudClient.Init();
            }
        }
    }
}