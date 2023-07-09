#include <sys/socket.h>
#include <arpa/inet.h>
#include <stdio.h>
#include <string.h>
#include <sys/socket.h>
#include <unistd.h>
#include "../LED/led.h"
#include <malloc.h>

#ifndef CAUDIOCLIENT_NETWORK_H
#define CAUDIOCLIENT_NETWORK_H

void sendData(Led *LedArray, int stripSize);

#endif //CAUDIOCLIENT_NETWORK_H
