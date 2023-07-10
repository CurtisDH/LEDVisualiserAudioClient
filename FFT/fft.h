#ifndef CAUDIOCLIENT_FFT_H
#define CAUDIOCLIENT_FFT_H

#include <complex.h>

void fft(complex double *x, int n);

void AnalyseAudio(pa_simple *record_handle, int *error, int16_t *buffer, int buffer_index);

#endif //CAUDIOCLIENT_FFT_H


