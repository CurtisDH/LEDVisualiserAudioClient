#include <stdio.h>
#include <math.h>
#include <complex.h>


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