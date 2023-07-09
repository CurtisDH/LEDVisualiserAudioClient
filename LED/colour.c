//
// Created by curtis on 9/07/23.
//

#include "colour.h"
#include <stdio.h>


Colour Blend(Colour initial, Colour previous, float weight)
{
    float initialR = initial.r * weight;
    float initialG = initial.g * weight;
    float initialB = initial.b * weight;

    float pR = previous.r * (1 - weight);
    float pG = previous.g * (1 - weight);
    float pB = previous.b * (1 - weight);

    unsigned char calculatedR = initialR + pR;
    unsigned char calculatedG = initialG + pG;
    unsigned char calculatedB = initialB + pB;

    Colour blendedColor = {calculatedR, calculatedG, calculatedB};
    return blendedColor;
}

Colour NormaliseColor(Colour color)
{
    float r = ((float) color.r / 255) * 10;
    float g = ((float) color.g / 255) * 10;
    float b = ((float) color.b / 255) * 10;

    Colour normalizedColor = {(unsigned char) r, (unsigned char) g, (unsigned char) b};
    return normalizedColor;
}

Colour Colours[] = {
        {0,   0,   0},      // First entry ignored
        {255, 0,   0},      // Red
        {255, 128, 0},      // Orange
        {255, 255, 0},      // Yellow
        {128, 255, 0},      // Lime
        {0,   255, 0},      // Green
        {0,   255, 128},    // Spring Green
        {0,   255, 255},    // Cyan
        {0,   128, 255},    // Sky Blue
        {0,   0,   255},    // Blue
        {128, 0,   255},    // Indigo
        {255, 0,   255},    // Magenta
        {255, 0,   128},    // Rose
        {255, 128, 128},    // Light Pink
        {255, 192, 128},    // Peach
        {255, 255, 128},    // Light Yellow
        {192, 255, 128},    // Light Lime
        {128, 255, 128},    // Light Green
        {128, 255, 192},    // Light Aqua
        {128, 255, 255},    // Light Cyan
        {128, 192, 255},    // Light Sky Blue
        {128, 128, 255},    // Light Blue
        {192, 128, 255},    // Light Indigo
        {255, 128, 255},    // Light Magenta
        {255, 128, 192},    // Light Rose
        {255, 192, 192},    // Light Salmon
        {255, 224, 192},    // Light Peach
        {255, 255, 192},    // Light Ivory
        {224, 255, 192},    // Light Lime Green
        {192, 255, 192},    // Light Sea Green
        {192, 255, 224},    // Light Mint
        {192, 255, 255},    // Light Aqua Blue
        {192, 224, 255},    // Light Baby Blue
        {192, 192, 255},    // Light Lavender
        {224, 192, 255},    // Light Orchid
        {255, 192, 255},    // Light Orchid Pink
        {255, 192, 224},    // Light Salmon Pink
        {255, 224, 224},    // Light Baby Pink
        {255, 240, 224},    // Light Cream
        {255, 255, 224},    // Light Yellow Ivory
        {240, 255, 224},    // Light Lime Ivory
        {224, 255, 224},    // Light Green Ivory
        {224, 255, 240},    // Light Mint Ivory
        {224, 255, 255},    // Light Aqua Ivory
        {224, 240, 255},    // Light Baby Blue Ivory
        {224, 224, 255},    // Light Lavender Ivory
        {240, 224, 255},    // Light Orchid Ivory
        {255, 224, 255},    // Light Orchid Pink Ivory
        {255, 224, 240},    // Light Salmon Pink Ivory
        {255, 240, 240},    // Light Baby Pink Ivory
        {255, 248, 240},    // Light Cream Ivory
        {255, 255, 240},    // Light Yellow Ivory Ivory
        {248, 255, 240},    // Light Lime Ivory Ivory
        {240, 255, 240},    // Light Green Ivory Ivory
        {240, 255, 248},    // Light Mint Ivory Ivory
        {240, 255, 255},    // Light Aqua Ivory Ivory
        {240, 248, 255},    // Light Baby Blue Ivory Ivory
        {240, 240, 255},    // Light Lavender Ivory Ivory
        {248, 240, 255},    // Light Orchid Ivory Ivory
        {255, 240, 255},    // Light Orchid Pink Ivory Ivory
        {255, 240, 248},    // Light Salmon Pink Ivory Ivory
        {255, 248, 248},    // Light Baby Pink Ivory Ivory
        {255, 252, 248},    // Light Cream Ivory IvoryThe expanded color palette above includes a range of colors that transition smoothly across a wide frequency range. You can continue adding more colors as needed to cover higher frequencies. The provided palette covers frequencies up to approximately 2000Hz, with each color representing a 50Hz increment. Feel free to modify or extend the palette to suit your requirements and achieve the desired color transition for your LED array.

};

void DetermineColour(Colour *colour, float freq, float mag)
{
    int numColours = sizeof(Colours) / sizeof(Colour);
    int jumps = 50;
    for (int i = 0; i < numColours; ++i)
    {
        int freqRange = i * jumps;
        if (freqRange <= freq || freqRange - jumps >= freq) continue;
        *colour = Colours[i];
    }
}

//int main()
//{
//    Colour initial = Colours[1];
//    Colour previous = Colours[0];
//    float weight = 0.75f;
//
//    Colour blendedColor = Blend(initial, previous, weight);
//    printf("Blended Colour: (%u, %u, %u)\n", blendedColor.r, blendedColor.g, blendedColor.b);
//
//    Colour normalizedColor = NormaliseColor(initial);
//    printf("Normalized Colour: (%u, %u, %u)\n", normalizedColor.r, normalizedColor.g, normalizedColor.b);
//
//    return 0;
//}
