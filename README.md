# AudioClient
Utilises NAudio (V2.1.0) WSAPI to capture the default windows playback device, runs it through [FFT](https://en.wikipedia.org/wiki/Fast_Fourier_transform) 
and turns it into a byte array containing RGB values for the entire strip **(see byte array order below)** which are influenced by the frequency value, brightness is controlled by the magnitude (which is like volume) the entire led strip which has been processed on the client is then sent to the target ip address and needs to be processed  


### Setup


Run the audio client on the machine that you are wanting to capture audio from. (Limitation here is that it requires to be running the windows OS)  
The two options when running the AudioClient are: 'client' and 'server', client will be sending the audio from the device to the target ip and port, server is deprecated

Create a UDP listen server and interpret the received data. [You can see my rough Raspberry Pi python implementation here](https://github.com/CurtisDH/AudioClient/blob/main/run_leds.py)
<br />
<br />
<br />
**Starting the client using runtime arguments**  
The following commands can be used when running via command prompt to change the default start settings:  
-threshold&nbsp;&nbsp;&nbsp;(Double type expected)  How loud a sound has to be before it gets picked up and displayed    
-ip&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;(string type expected)  The target IP of the device you want to send the data to  
-port&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;(integer type expected) The target port of ... as above.  
-strip&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;(integer type expected) This is the size of your LED strip    
-speed&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;(integer type expected) This is the update rate of LED strip in milliseconds       
-split&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;(true/false string response expected) Splits the visualiser down the middle into two visual streams  
Example using command prompt to run the exe:  
AudioClient.exe -port 5555 -ip 192.168.1.11 -strip 150 -speed 11 -threshold 10
<br />
<br />


#### Byte array order  
The newer implementation uses the client to process the entire strip, and sends the whole strip as a byte array.  
In order to process the byte array and turn it into meaningful data you need to loop over the received data in offset increments of 3, the first value i.e. 1 of 3 is red, 2 is blue, 3 is green. All that is required after processing this is to populate the LED strip array and display it see my example python script [here](https://github.com/CurtisDH/AudioClient/blob/main/run_leds.py): 

## Potentially useful information
Magnitude threshold: Lower values will respond to quieter sounds, higher values will ignore quieter sounds. Keep in mind the magnitude value is captured from the highest value in the FFT set, play around with this value to see what works best for you    
<br />    
On the same note, colour spectrum is based on the frequency, and the brightness is controlled by the volume (magnitude).  
(WIP) Smooth colour blending and alternating between different sets of colours.
<br />
Changing default device during playback requires a reconnect. The currently implemented solution is to interact with the terminal by pressing any key which will trigger a reset, ultimately targeting the default playback device.

