cmake_minimum_required(VERSION 3.25)
project(CAudioClient C)

set(CMAKE_C_STANDARD 17)

add_executable(CAudioClient main.c fft.c fft.h getDevices.c getDevices.h led.c led.h)
target_link_libraries(CAudioClient pulse pulse-simple m)