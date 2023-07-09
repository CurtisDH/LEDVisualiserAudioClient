#include <stdio.h>
#include <pulse/simple.h>
#include <pulse/error.h>

#include "FFT/fft.h"
#include "FetchDevices/getDevices.h"
#include "Configs/constants.h"




// TODO 
// create a byte array to simulate each led -- DONE READY TO POPULATE WITH DATA
// we still need to filter out the random noise jumps to over 40k freq -- DONE
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
// create a json parser so we don't have to hard code the led variables


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

    // This is the main loop of the program, sending data will be done through this method
    // same with all the colour related actions. 
    // TODO should this be abstracted or should it just be in the main method?? 
    AnalyseAudio(record_handle, &error, buffer, buffer_index);

    // Clean up and close the streams
    pa_simple_free(record_handle);

    return 0;
}



