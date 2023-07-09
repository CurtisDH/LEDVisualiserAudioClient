#include <stdio.h>
#include <pulse/simple.h>
#include <pulse/error.h>

#include "fft.h"
#include "getDevices.h"


#define SAMPLE_RATE 44100
#define CHANNELS 1
#define BUFFER_SIZE 2048
#define SAMPLE_RESOLUTION 2048  // Number of samples for FFT (must be a power of 2)
#define HOP_SIZE 128  // Number of samples to hop between segments



// TODO 
// create a byte array to simulate each led
// we still need to filter out the random noise jumps to over 40k freq
// we want to take the average magnitude so that its not blindingly bright all the time
// this will also allow for songs to have a high visualisation definition if the start volume is low
// Also still need to implement functionality that we had in the C# one,
// colour blending,
// splitting (can make this even better tho)
// json file for the colours so we can dynamically update them?
// colour rotation cycles
// maybe a gui if i can be bothered
// might need to rewrite for just ALSA support so 
// it'll work on the majority of linux devices (think this would apply to android too?)
// bluetooth?
// live latency adjustment, might need intermediary hardware to successfully measure this


void SendData();

int main()
{
    const uint8_t size = 16;

    // This is where we'll store the input device list
    pa_devicelist_t inputDevices[size];

    // This is where we'll store the output device list
    pa_devicelist_t outputDevices[size];

    // limitation would be anything over 16 output or input devices
    // which granted is unlikely but //TODO should probably account for this
    getDeviceList(inputDevices, outputDevices);

    int input = audioDeviceSelection(size, outputDevices);
    pa_simple *record_handle;
    int error;
    SetupAudioCapture(outputDevices, input, &record_handle, &error, SAMPLE_RATE, CHANNELS);

    if (!record_handle)
    {
        fprintf(stderr, "Failed to create recording stream: %s\n", pa_strerror(error));
        return -1;
    }

    // Circular buffer to hold the audio samples
    int16_t buffer[BUFFER_SIZE];
    int buffer_index = 0;
    printf("Starting Real-time audio analysis on device: %s\n", outputDevices[input].name);

    AnalyseAudio(record_handle, &error, buffer, buffer_index, BUFFER_SIZE, SAMPLE_RESOLUTION, SAMPLE_RATE, HOP_SIZE);

    SendData();
    // Clean up and close the streams
    pa_simple_free(record_handle);

    return 0;
}

void SendData()
{
    // TODO
    // this will be the implementation which sends our byte array with all the data to the client

}


