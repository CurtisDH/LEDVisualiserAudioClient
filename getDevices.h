
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

void SetupAudioCapture(const pa_devicelist_t *outputDevices, int input, pa_simple **record_handle, int *error,
                       int SAMPLE_RATE, int CHANNELS);

int audioDeviceSelection(const uint8_t size, const pa_devicelist_t *outputDevices);

#endif //CAUDIOCLIENT_GETDEVICES_H
