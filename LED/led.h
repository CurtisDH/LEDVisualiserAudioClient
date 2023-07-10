//
// Created by curtis on 9/07/23.
//
#include <stdio.h>
#include <stdint.h>
#include "colour.h"


#ifndef CAUDIOCLIENT_LED_H
#define CAUDIOCLIENT_LED_H
// This will include the struct for the LED
typedef struct
{
    uint8_t r;
    uint8_t g;
    uint8_t b;

} Led;

void AddLed(Colour *colour, Led *LedArray, int stripSize, double averageMagnitude);

void serializeLedData(const Led *LedArray, int ledArraySize, uint8_t *byteArr);

#endif //CAUDIOCLIENT_LED_H
