namespace AudioClient
{
    internal static class Program
    {
        private static double defaultThreshold = 125; // this is kinda like volume

        private static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                string ip = "127.0.0.1";
                Console.WriteLine($"###WARNING###");
                Console.WriteLine($"No arguments provided Starting audio client with target IP address set to:\n{ip}");
                Console.WriteLine($"###WARNING###");

                var audioAudClient = new AudClient(defaultThreshold, ip);
                audioAudClient.Init();
                return;
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
                Console.WriteLine("Starting audio client...");
                var audioAudClient = new AudClient(threshold, targetIP);
                audioAudClient.Init();
            }
        }
    }
}