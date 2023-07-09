
#include <stdio.h>
#include <string.h>
#include <pulse/pulseaudio.h>

#ifndef CAUDIOCLIENT_GETDEVICES_H
#define CAUDIOCLIENT_GETDEVICES_H
typedef struct pa_devicelist
{
    uint8_t initialized;
    char name[512];
    uint32_t index;
    char description[256];
} pa_devicelist_t;

int getDeviceList(pa_devicelist_t *pa_input_devicelist, pa_devicelist_t *pa_output_devicelist);


#endif //CAUDIOCLIENT_GETDEVICES_H
