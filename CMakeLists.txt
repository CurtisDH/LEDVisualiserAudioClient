cmake_minimum_required(VERSION 3.25)
project(CAudioClient C)

set(CMAKE_C_STANDARD 17)

add_executable(CAudioClient main.c "FFT Module/fft.c" "FFT Module/fft.h" "FetchDevices Module/getDevices.c" "FetchDevices Module/getDevices.h" "LED Module/led.c" "LED Module/led.h")
target_link_libraries(CAudioClient pulse pulse-simple m)