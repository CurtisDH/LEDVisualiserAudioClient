#ifndef CAUDIOCLIENT_FFT_H
#define CAUDIOCLIENT_FFT_H

#include <complex.h>

void fft(complex double *x, int n);

void
AnalyseAudio(pa_simple *record_handle, int *error, int16_t *buffer, int buffer_index, int BUFFER_SIZE, int SAMPLE_RES,
             int SAMPLE_RATE, int HOP_SIZE);

#endif //CAUDIOCLIENT_FFT_H


