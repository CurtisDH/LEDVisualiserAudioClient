#include <stdio.h>
#include <pulse/simple.h>
#include <pulse/error.h>
#include <string.h>
#include <complex.h>

#include "fft.h"
#include "getDevices.h"


#define SAMPLE_RATE 44100
#define CHANNELS 1
#define BUFFER_SIZE 2048
#define N 2048  // Number of samples for FFT (must be a power of 2)
#define HOP_SIZE 128  // Number of samples to hop between segments


int main()
{

    // This is where we'll store the input device list
    pa_devicelist_t inputDevices[16];

    // This is where we'll store the output device list
    pa_devicelist_t outputDevices[16];

    getDeviceList(inputDevices, outputDevices);

    int input = -1;
    // wait for a valid device to be selected
    while (input < 0 || input > 16)
    {
        printf("Select from the following devices:\n");
        for (int i = 0; i < 16; ++i)
        {
            if (strlen(outputDevices[i].name) == 0)
            {
                // empty entry.
                break;
            }
            printf("=======[ Output Device #%d ]=======\n", i);
            printf("Description: %s\n", outputDevices[i].description);
            printf("Name: %s\n", outputDevices[i].name);
        }
        scanf("%d", &input);
    }


    pa_simple *record_handle = NULL;
    pa_sample_spec record_spec;
    char *record_device = strcat(outputDevices[input].name, ".monitor");
    int error;

    // Set up the sample specifications for recording
    record_spec.format = PA_SAMPLE_S16LE;
    record_spec.rate = SAMPLE_RATE;
    record_spec.channels = CHANNELS;

    // Create a new recording stream
    record_handle = pa_simple_new(NULL, "Record", PA_STREAM_RECORD, record_device, "Record", &record_spec, NULL, NULL,
                                  &error);
    if (!record_handle)
    {
        fprintf(stderr, "Failed to create recording stream: %s\n", pa_strerror(error));
        return -1;
    }

    // Circular buffer to hold the audio samples
    int16_t buffer[BUFFER_SIZE];
    int buffer_index = 0;

    printf("Real-time audio analysis from %s...\n", outputDevices[input].name);

    // Continuously capture and analyze audio
    while (1)
    {
        // Read audio samples into the circular buffer
        if (pa_simple_read(record_handle, buffer + buffer_index, (BUFFER_SIZE - buffer_index) * sizeof(int16_t),
                           &error) < 0)
        {
            fprintf(stderr, "Failed to capture audio: %s\n", pa_strerror(error));
            break;
        }

        buffer_index += (BUFFER_SIZE - buffer_index);

        // If enough samples are available, perform FFT and analysis
        while (buffer_index >= N)
        {
            // Convert captured audio buffer to complex double type
            complex double complex_buffer[N];
            for (int i = 0; i < N; ++i)
            {
                complex_buffer[i] = (double) buffer[i] / (double) INT16_MAX;
            }

            // Perform FFT on the captured audio
            fft(complex_buffer, N);

            // Calculate frequency and magnitude for each sample
            double frequencies[N];
            double magnitudes[N];
            double max_magnitude = 0.0;
            int max_magnitude_index = 0;

            for (int i = 0; i < N; ++i)
            {
                frequencies[i] = (double) SAMPLE_RATE * i / N;
                magnitudes[i] = cabs(complex_buffer[i]);

                if (magnitudes[i] > max_magnitude)
                {
                    max_magnitude = magnitudes[i];
                    max_magnitude_index = i;
                }
            }

            printf("Maximum magnitude: %f at frequency: %f Hz\n", max_magnitude, frequencies[max_magnitude_index]);

            // Shift buffer contents by hop size
            memmove(buffer, buffer + HOP_SIZE, (BUFFER_SIZE - HOP_SIZE) * sizeof(int16_t));
            buffer_index -= HOP_SIZE;
        }
    }

    // Clean up and close the streams
    pa_simple_free(record_handle);

    return 0;
}
