#include <malloc.h>
#include "led.h"
#include "../Network/network.h"
#include "../Configs/constants.h"
#include "colour.h"
#include <stdio.h>
#include <stdlib.h>


int ledStripState(Led *LedArray);


// TODO we need to figure out how we turn off the leds, maybe we literally just make it flow 
//  through when mag is 0 but we probs want to stop sending data when all the leds are off 
//  something to think about
void serializeLedData(const Led *LedArray, int ledArraySize, uint8_t *byteArr)
{
    if (LedArray == NULL)
        return;

    if (byteArr == NULL)
    {
        // we could dynamically create one, but we'd then have the responsibility of freeing the memory
        // could cause some issues for us if we lose track down the line
        fprintf(stderr, "Warning: %s\n",
                "Byte array was null, memory has been allocated, MAKE SURE THIS IS FREED LATER TO AVOID LEAKS");
        byteArr = malloc((ledArraySize * LED_PARAMS));
    }

    // byte array size shouldn't need to be queried from the other classes
    // the size is always going to be dependent on the number of parameters we're sending i.e.
    // the LED struct parameters r,g,b,a
    for (int i = 0; i < ledArraySize; ++i)
    {
        byteArr[i * LED_PARAMS] = LedArray[i].r;
        byteArr[i * LED_PARAMS + 1] = LedArray[i].g;
        byteArr[i * LED_PARAMS + 2] = LedArray[i].b;
    }

}

void UpdateStrip(Led *LedArray, int stripSize)
{
    if (ledStripState(LedArray) == 0)
    {
        usleep(DETECTION_DELAY_IN_MS * 1000);
        // dont send an update packet if the strip  are all off
        return;
    }
    usleep(DELAY_IN_MS * 1000);
    // todo call in a second thread if required
    for (int i = stripSize; i >= 0; --i)
    {
        LedArray[i + 1] = LedArray[i];
    }
    // The way this will work is starting from the led count, in a for loop and working backwards
    // this way we will get the update effect starting from the opposite side of the strip
    // We do need to allow for a splitting function, so might be worth taking into account when
    // writing the initial update
    sendData(LedArray, stripSize);

}

Colour ColourBlend(Colour initial, Colour previous)
{
    float weight = 0.5f;
    float initialR = (float) initial.r * weight;
    float initialG = (float) initial.g * weight;
    float initialB = (float) initial.b * weight;

    float prevR = (float) previous.r * (1 - weight);
    float prevG = (float) previous.g * (1 - weight);
    float prevB = (float) previous.b * (1 - weight);


    uint8_t calcR = (uint8_t) (initialR + prevR);
    uint8_t calcG = (uint8_t) (initialG + prevG);
    uint8_t calcB = (uint8_t) (initialB + prevB);

//    printf("Blended Color: R=%u, G=%u, B=%u\n", calcR, calcG, calcB);

    Colour c = {calcR, calcG, calcB};
    return c;
}


void AddLed(Colour colour, Led *LedArray, double averageMagnitude)
{
//    int brightness = (int) (averageMagnitude * MAX_BRIGHTNESS);
//    printf("brightness: %u avg mag %f:", brightness, averageMagnitude);
    LedArray[0].r = ((colour.r * averageMagnitude) / (255 - MAX_BRIGHTNESS));
    LedArray[0].g = ((colour.g * averageMagnitude) / (255 - MAX_BRIGHTNESS));
    LedArray[0].b = ((colour.b * averageMagnitude) / (255 - MAX_BRIGHTNESS));

//    printf("r:%d,g:%d,b:%d", LedArray[0].r, LedArray[0].g, LedArray[0].b);

}

int ledStripState(Led *LedArray)
{
    for (int i = 0; i < LED_STRIP_SIZE; ++i)
    {
        if (LedArray[i].r > 0 || LedArray[i].g > 0 || LedArray[i].b > 0)
        {
            // Strip has an LED active
//            printf("LEDS ACTIVE\n");
            return 1;
        }
    }
    // No leds are currently active
//    printf("NO LEDS ACTIVE\n");
    return 0;
}

