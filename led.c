#include <malloc.h>
#include "led.h"




// TODO might be better to create like an overall config that we just include for these
// constant variables
#define LED_PARAMS 4

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
        byteArr = malloc(ledArraySize * 4);
    }

    // byte array size shouldn't need to be queried from the other classes
    // the size is always going to be dependent on the number of parameters we're sending i.e.
    // the LED struct parameters r,g,b,a
    for (int i = 0; i < ledArraySize; ++i)
    {
        byteArr[i * LED_PARAMS] = LedArray[i].r;
        byteArr[i * LED_PARAMS + 1] = LedArray[i].g;
        byteArr[i * LED_PARAMS + 2] = LedArray[i].b;
        byteArr[i * LED_PARAMS + 3] = LedArray[i].a;
    }

}

void UpdateStrip(Led *LedArray, int stripSize)
{
    // todo call in a second thread if required
    for (int i = stripSize; i > 0; --i)
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

void AddLed(Led LedToAdd, Led *LedArray, int stripSize)
{
    UpdateStrip(LedArray, stripSize);
    LedArray[0] = LedToAdd;
    // push this data out? or do another update strip? -- I think another update is overkill
}

void sendData(Led *LedArray, int stripSize)
{
    // this will basically just send the update packet so lets prepare the data to send
    uint8_t *byteArray = malloc(stripSize * LED_PARAMS);
    serializeLedData(LedArray, stripSize, byteArray);
    // should now be written into the byte array so all we need to do is send it, and then free memory
    // todo sockets and other network stuff
    free(byteArray);
}