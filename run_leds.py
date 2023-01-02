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
    num_leds: int
    R: int
    G: int
    B: int
    Brightness: int
    Delay: int


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
            num_leds = data[0]
            r = data[1]
            g = data[2]
            b = data[3]
            brightness = data[4]
            delay = data[5]
            print(f"Receiving Data: NUMLEDS:{num_leds}, R {r}, G {g}, B {b}, brightness: {brightness}, Delay: {delay}")

            instruct = Instruction(data[0], data[1], data[2], data[3], data[4], data[5])
            global colour
            colour = (instruct.R, instruct.G, instruct.B)
            pixels[0] = colour
        else:
            print("No data from ", address)
            break

    sock.close()



async def main():
    networkThread = threading.Thread(target=Initiate_Conn)
    number_of_threads = 1
    number_of_leds = 150
    for i in range(number_of_threads):
        ledShiftThread = threading.Thread(target=Shift_Leds, args=(i, i * number_of_leds, number_of_leds))
        ledShiftThread.start()

    networkThread.start()


def Shift_Leds(thread_num, start_pos, num_leds):
    if start_pos == 0:
        max = num_leds
    else:
        max = start_pos * 2
    print(f"starting thread: StartPOS:{start_pos} MAX: {max}")

    # TODO revisit
    while True:
        start = time.time()
        for i in range(num_leds - 1, 0, -1):

            if (i - 1) <= 149:
                if i == 149:
                    continue
                if i == 1:
                    pixels[i + 1] = pixels[0]
                    pixels[1] = pixels[0]
                    continue
                pixels[i + 1] = pixels[i]

        pixels.show()

        end = time.time()

        print(f"Thread {thread_num} timer:  {end - start}")


asyncio.run(main())
