#include <stdlib.h>
#include "network.h"
#include "../Configs/constants.h"

void sendData(Led *LedArray, int stripSize)
{
    // this will basically just send the update packet so let's prepare the data to send
    uint8_t *byteArray = malloc(stripSize * LED_PARAMS);
    serializeLedData(LedArray, stripSize, byteArray);
    // should now be written into the byte array so all we need to do is send it, and then free memory
    // todo sockets and other network stuff

    // IPV4 - AF_INET
    // IPV6 - AF_INET6
    // todo probably will have to support ipv6 in the future but ceeb for now
    int sockfd = socket(AF_INET, SOCK_DGRAM, 0);
    if (sockfd == -1)
    {
        perror("socket");
        exit(1);
    }
    struct sockaddr_in dest_addr;
    memset(&dest_addr, 0, sizeof(dest_addr));
    dest_addr.sin_family = AF_INET;
    dest_addr.sin_port = htons(PORT);  // Destination port

    if (inet_pton(AF_INET, IP_ADDRESS, &(dest_addr.sin_addr)) != 1)
    {
        perror("inet_pton");
        exit(1);
    }

    if (sendto(sockfd, byteArray, stripSize * LED_PARAMS, 0, (struct sockaddr *) &dest_addr, sizeof(dest_addr)) == -1)
    {
        perror("sendto");
        exit(1);
    }
    free(byteArray);
    close(sockfd);
}
