#include <stdio.h>
#include <math.h>
#include <complex.h>
#include <string.h>
#include <pulse/simple.h>
#include <pulse/error.h>

void fft(complex double *x, int n)
{
    if (n <= 1)
    {
        return;
    }

    complex double even[n / 2];
    complex double odd[n / 2];

    for (int i = 0; i < n / 2; ++i)
    {
        even[i] = x[2 * i];
        odd[i] = x[2 * i + 1];
    }

    fft(even, n / 2);
    fft(odd, n / 2);

    for (int k = 0; k < n / 2; ++k)
    {
        complex double t = cexp(-I * 2 * M_PI * k / n) * odd[k];
        x[k] = even[k] + t;
        x[k + n / 2] = even[k] - t;
    }
}

void AnalyseAudio(pa_simple *record_handle, int *error, int16_t *buffer, int buffer_index, int BUFFER_SIZE, int SAMPLE_RES,
                  int SAMPLE_RATE, int HOP_SIZE)
{// Continuously capture and analyze audio
    while (1)
    {
        // TODO, allow for the user to interrupt this so they can change device without restart
        // Read audio samples into the circular buffer
        if (pa_simple_read(record_handle, buffer + buffer_index, (BUFFER_SIZE - buffer_index) * sizeof(int16_t),
                           error) < 0)
        {
            fprintf(stderr, "Failed to capture audio: %s\n", pa_strerror((*error)));
            break;
        }

        buffer_index += (BUFFER_SIZE - buffer_index);

        // If enough samples are available, perform FFT and analysis
        while (buffer_index >= SAMPLE_RES)
        {
            // Convert captured audio buffer to complex double type
            complex double complex_buffer[SAMPLE_RES];
            for (int i = 0; i < SAMPLE_RES; ++i)
            {
                complex_buffer[i] = (double) buffer[i] / (double) INT16_MAX;
            }

            // Perform FFT on the captured audio
            fft(complex_buffer, SAMPLE_RES);

            // Calculate frequency and magnitude for each sample
            double frequencies[SAMPLE_RES];
            double magnitudes[SAMPLE_RES];
            double max_magnitude = 0.0;
            int max_magnitude_index = 0;

            for (int i = 0; i < SAMPLE_RES; ++i)
            {
                frequencies[i] = (double) SAMPLE_RATE * i / SAMPLE_RES;
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
}