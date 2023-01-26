# AudioClient
Utilises NAudio (V2.1.0) WSAPI to capture the default windows playback device, runs it through [FFT](https://en.wikipedia.org/wiki/Fast_Fourier_transform) 
and turns it into a byte array containing RGB values for the entire strip **(see byte array order below)** which are influenced by the frequency value, brightness is controlled by the magnitude (which is like volume) the entire led strip which has been processed on the client is then sent to the target ip address and needs to be processed  


### Setup


Run the audio client on the machine that you are wanting to capture audio from. (Limitation here is that it requires to be running the windows OS)  
The two options when running the AudioClient are: 'client' and 'server', client will be sending the audio from the device to the target ip and port, server is deprecated

Create a UDP listen server and interpret the received data. [You can see my rough Raspberry Pi python implementation here](https://github.com/CurtisDH/AudioClient/blob/main/run_leds.py)

**Arguments are supported but not gracefully  (WIP) need to add 2 additonal arguments**  
If no values are entered it defaults to client with the magnitude threshold set to 75 the target IP: 192.168.1.11  port: 5555  
Expects arguments in the following order:  
type: (client/server)  
<br />
IF SERVER  (DEPRECATED) Legacy code still exists here but will not function (WIP) refactoring:     
arg[1]: port (e.g. 5555)  
arg[2]: byte array size (set to 6 if no modifications have been made.)  
 <br />
**SERVER EXAMPLE starting program without manual entries via terminal**  
AudioClient.exe server 5555 6  
Starts the program as a server, waiting on port 5555, and expecting a byte array size of 6.  
<br />  
IF CLIENT:  
arg[1]: magnitude threshold  
arg[2]: targetIP (e.g. 192.168.1.11)   
arg[3]: port (e.g. 5555)   
Currently the additional arguments are not supported via command line, those being strip_size and speed (int as ms)
<br />
**CLIENT EXAMPLE starting program without manual entries via terminal**  
AudioClient.exe client 75 192.168.11 5555  
Starts the program as a client, magnitude threshold at 75, ip set to 192.168.11, and the target port being 5555
<br />
<br />
<br />
### Debugging Optional parameter
adding debug to the end of either, server, or client e.g  
AudioClient.exe client 75 192.168.11 5555 debug  
will then log the magnitude value, and adjusted magnitude value as well as other debug prompts.



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

