#ifndef CAUDIOCLIENT_CONSTANTS_H
#define CAUDIOCLIENT_CONSTANTS_H
#define LED_PARAMS 3 // How many variables are in the struct i.e. r,g,b,a. important for determining the byte array
#define FILTER_MAGNITUDE 35000 // gets rid of the 'noise' that was previously spiking upto 4nnnn
#define SAMPLE_RATE 44100 // 44100 is the standard for most apps i.e. spotify and most audio services.
#define CHANNELS 1 // fft gets processed differently if its anything other than mono // TODO look into this
#define BUFFER_SIZE 2048 // audio buffer size 
#define SAMPLE_RESOLUTION 2048  // Number of samples for FFT (must be a power of 2)
#define HOP_SIZE 512  // Number of samples to hop between segments
#define LED_STRIP_SIZE 150 // Num of LED in the physical array

#define IP_ADDRESS "192.168.1.11"
#define PORT 5555
#define DELAY_IN_MS 10

#endif //CAUDIOCLIENT_CONSTANTS_H
