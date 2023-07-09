#ifndef CAUDIOCLIENT_COLOUR_H
#define CAUDIOCLIENT_COLOUR_H

// TODO i guess the idea here is to allow for name representation of a wide range of colours
// alternatively we just use r,g,b vals from 0 - 255 
typedef struct
{
    unsigned char r;
    unsigned char g;
    unsigned char b;
} Colour;

void DetermineColour(Colour *colour, float freq, float mag);

#endif //CAUDIOCLIENT_COLOUR_H
