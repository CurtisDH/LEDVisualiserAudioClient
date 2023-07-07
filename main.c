#include <stdio.h>
#include <pulse/simple.h>
#include <pulse/error.h>
#include <math.h>
#include <complex.h>

#include "fft.h"

#define SAMPLE_RATE 44100
#define CHANNELS 1
#define BUFFER_SIZE 2048
#define N 2048  // Number of samples for FFT (must be a power of 2)


int main()
{
    pa_simple *record_handle = NULL;
    pa_sample_spec record_spec;
    const char *record_device = "bluez_sink.EC_81_93_58_81_91.a2dp_sink.monitor";
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

    printf("Real-time audio analysis from Bluetooth A2DP sink device...\n");

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

        // If the buffer is full, perform FFT and analysis
        if (buffer_index >= BUFFER_SIZE)
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

            // Shift buffer contents to the beginning
            memmove(buffer, buffer + N, (BUFFER_SIZE - N) * sizeof(int16_t));
            buffer_index -= N;
        }
    }

    // Clean up and close the streams
    pa_simple_free(record_handle);

    return 0;
}


