cmake_minimum_required(VERSION 3.25)
project(CAudioClient C)

set(CMAKE_C_STANDARD 17)

add_executable(CAudioClient main.c "FFT/fft.c" "FFT/fft.h" "FetchDevices/getDevices.c" "FetchDevices/getDevices.h" "LED/led.c" "LED/led.h" "LED/colour.c" "LED/colour.h" Network/network.c Network/network.h Configs/constants.h)
target_link_libraries(CAudioClient pulse pulse-simple m)