#include <malloc.h>
#include "led.h"



// and the creation of the byte array that we can populate

// are we going to serialize it in some sort of fashion? do we create a union?


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
        byteArr[i * 4] = LedArray[i].r;
        byteArr[i * 4 + 1] = LedArray[i].g;
        byteArr[i * 4 + 2] = LedArray[i].b;
        byteArr[i * 4 + 3] = LedArray[i].a;
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
