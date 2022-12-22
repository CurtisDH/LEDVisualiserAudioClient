using System;
using NAudio.Wave;

namespace AudioClient
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            using (WasapiLoopbackCapture capture = new())
            {
                capture.WaveFormat = new WaveFormat(44100, 16, 2);
                // Set up an event handler to receive the audio data
                capture.DataAvailable += (sender, e) =>
                {
                    // TODO 
                    // where do we want to send this, and how do we want to process it? // e.Buffer;
                };

                // Start capturing audio
                capture.StartRecording();

                // Wait for the user to stop capturing audio
                Console.WriteLine("Press any key to stop capturing audio...");
                Console.ReadKey();

                // Stop capturing audio
                capture.StopRecording();
            }
        }
    }
}