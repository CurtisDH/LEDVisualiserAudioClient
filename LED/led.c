#include <malloc.h>
#include "led.h"
#include "../Network/network.h"
#include "../Configs/constants.h"



// TODO might be better to create like an overall config that we just include for these
// constant variables


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
    // todo call in a second thread if required
    for (int i = stripSize; i >= 0; --i)
    {
        LedArray[i + 1] = LedArray[i];
    }
    // The way this will work is starting from the led count, in a for loop and working backwards
    // this way we will get the update effect starting from the opposite side of the strip
    // We do need to allow for a splitting function, so might be worth taking into account when
    // writing the initial update
}

void ColourBlend()
{

}

void AddLed(int r, int g, int b, int a, Led *LedArray, int stripSize)
{
    UpdateStrip(LedArray, stripSize);

    LedArray[0].r = r;
    LedArray[0].g = g;
    LedArray[0].b = b;

    // push this data out? or do another update strip? -- I think another update is overkill
    sendData(LedArray, stripSize);
}

