import asyncio
import random
import threading
import time
import socket
from dataclasses import dataclass

import board
import neopixel

import concurrent.futures


@dataclass
class Instruction:
    R: int
    G: int
    B: int


pixel_pin = board.D18
num_pixels = 150
ORDER = neopixel.GRB
brightness = 3
pixels = neopixel.NeoPixel(
    pixel_pin, num_pixels, brightness=255, pixel_order=ORDER, auto_write=False
)

# clears the strip if its currently active
pixels.fill((0, 0, 0))

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

server_address = ('192.168.1.11', 5555)

sock.bind(server_address)

print("Awaiting data...")

colour = (0, 0, 0)


def Initiate_Conn():
    while True:
        data, address = sock.recvfrom(1024)
        if data:
            print("Processing data")
            Process_Byte_Array(data)
            pixels.show()
            print("Displaying")
        else:
            print("No data from ", address)
            break

    sock.close()


def Process_Byte_Array(data):
    start = time.time()
    size = int(len(data) / 3)
    print(size)
    for i in range(size):
        byteArrayIndex = i * 3
        pixels[i] = (data[byteArrayIndex], data[byteArrayIndex + 1], data[byteArrayIndex + 2])
    end = time.time()

    print(f"timer:  {end - start}")


async def main():
    networkThread = threading.Thread(target=Initiate_Conn)
    networkThread.start()


asyncio.run(main())
