namespace AudioClient
{
    internal static class Program
    {
        private static double _threshold = 0.025; // this is kinda like volume

        private static void Main(string[] args)
        {
            AudClient audioAudClient = new AudClient(_threshold);
            audioAudClient.Init();
        }
    }
}