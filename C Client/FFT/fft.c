#include <stdio.h>
#include <math.h>
#include <complex.h>
#include <string.h>
#include <pulse/simple.h>
#include <pulse/error.h>
#include <malloc.h>
#include <pthread.h>


#include "../LED/led.h"
#include "../Configs/constants.h"

// TODO major refactor this class is gross

int isRunning = 1;

void *stripUpdateThread(void *arg);


int ProcessBufferAndSendData(int16_t *buffer, int buffer_index, Led *LedArray, Colour *prevColour);

int ShiftBuffer(int16_t *buffer, int buffer_index);

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

double calculateAverageMagnitude(const double *magnitudes, int start, int end)
{
    double sum = 0.0;
    for (int i = 0; i < end; ++i)
    {
        sum += magnitudes[i];
    }
    return sum / (end - start);
}


void AnalyseAudio(pa_simple *record_handle, int *error, int16_t *buffer, int buffer_index)
{// Continuously capture and analyze audio
// Create the LED array all values are initialised to 0 from malloc.
    Led *LedArray = malloc(LED_STRIP_SIZE * LED_PARAMS); // TODO wtf check this again
    // random values were appearing,
    // from research malloc simply allocates memory without initialising so they can be arbitrary?
    // still dont really know why this is // TODO research this
    for (int i = 0; i < LED_STRIP_SIZE; ++i)
    {
        LedArray[i].r = 0;
        LedArray[i].g = 0;
        LedArray[i].b = 0;
    }


// basically we need to determine the pattern and the colour based on the frequency that comes up.
    pthread_t pthread;
    if (pthread_create(&pthread, NULL, stripUpdateThread, (void *) LedArray) != 0)
    {
        fprintf(stderr, "Failed to create the thread\n");
        return;
    }
    Colour prevColour = {0, 0, 0};
    

    while (1)
    {
        // TODO, allow for the user to interrupt this so they can change device without restart
        // Read audio samples into the circular buffer

        if (pa_simple_read(record_handle, buffer + buffer_index,
                           (BUFFER_SIZE - buffer_index) * sizeof(int16_t), error) < 0)
        {
            fprintf(stderr, "Failed to capture audio: %s\n", pa_strerror((*error)));
            break;
        }

        buffer_index += (BUFFER_SIZE - buffer_index);

        // If enough samples are available, perform FFT and analysis
        buffer_index = ProcessBufferAndSendData(buffer, buffer_index, LedArray, &prevColour);
    }
    // Currently no way to exit, but once we do I don't want to have forgotten later
    free(LedArray);
    isRunning = 0;
    pthread_join(pthread, NULL);
}

int ProcessBufferAndSendData(int16_t *buffer, int buffer_index, Led *LedArray, Colour *prevColour)
{
    while (buffer_index >= SAMPLE_RESOLUTION)
    {
// Convert captured audio buffer to complex double type
        complex double complex_buffer[SAMPLE_RESOLUTION];
        for (int i = 0; i < SAMPLE_RESOLUTION; ++i)
        {
            complex_buffer[i] = (double) buffer[i] / (double) INT16_MAX;
        }

        // Perform FFT on the captured audio
        fft(complex_buffer, SAMPLE_RESOLUTION);

        // Calculate frequency and magnitude for each sample
        double frequencies[SAMPLE_RESOLUTION];
        double magnitudes[SAMPLE_RESOLUTION];
        double max_magnitude = 0.0;
        int max_magnitude_index = 0;

        for (int i = 0; i < SAMPLE_RESOLUTION; ++i)
        {
            frequencies[i] = (double) SAMPLE_RATE * i / SAMPLE_RESOLUTION;
            magnitudes[i] = cabs(complex_buffer[i]);

            if (magnitudes[i] > max_magnitude && frequencies[i] < FILTER_MAGNITUDE)
            {
                max_magnitude = magnitudes[i];
                max_magnitude_index = i;
            }
        }
        // just avoiding spamming the terminal
        if (max_magnitude > 0)
        {
            printf("Maximum magnitude: %f at frequency: %f Hz\n", max_magnitude,
                   frequencies[max_magnitude_index]);
        }

        // Calculate the average magnitude
        int start = max_magnitude_index - AVERAGE_WINDOW_SIZE / 2;
        int end = start + AVERAGE_WINDOW_SIZE;

        if (start < 0)
        {
            start = 0;
            end = AVERAGE_WINDOW_SIZE;
        }
        if (end > SAMPLE_RESOLUTION)
        {
            end = SAMPLE_RESOLUTION;
            start = end - AVERAGE_WINDOW_SIZE;
        }
        double averageMagnitude = calculateAverageMagnitude(magnitudes, start, end);

        Colour *colour = malloc(sizeof(Colour));
        colour->r = 0;
        colour->g = 0;
        colour->b = 0;
        DetermineColour(colour, frequencies[max_magnitude_index]);
        Colour blended = ColourBlend(*colour, (*prevColour));
        (*prevColour) = blended;
        // this adds an led to the strip, and the update thread will display it 
        // from my testing this delay isn't enough to cause visual delay on critical audio moments e.g. beat drop
        AddLed(blended, LedArray, averageMagnitude);
        free(colour);

        buffer_index = ShiftBuffer(buffer, buffer_index);
    }
    return buffer_index;
}

int ShiftBuffer(int16_t *buffer, int buffer_index)
{
    // Shift buffer contents by hop size
    memmove(buffer, buffer
                    + HOP_SIZE, (BUFFER_SIZE - HOP_SIZE) * sizeof(int16_t));
    buffer_index -= HOP_SIZE;
    return buffer_index;
}


void *stripUpdateThread(void *arg)
{
    Led *LedArray = (Led *) arg;

    while (isRunning)
    {
        UpdateStrip(LedArray, LED_STRIP_SIZE);
    }

    return NULL;
}