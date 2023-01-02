# AudioClient
Utilises NAudio (V2.1.0) WSAPI to capture the default windows playback device, runs it through [FFT](https://en.wikipedia.org/wiki/Fast_Fourier_transform) 
and turns it into a byte array containing RGB values **(see byte array order below)** which are influenced by the max magnitude value (somewhat like volume)
which is then sent to the target ip address  

Project contains a network server to test the sent information locally but does not do anything relevant with this information apart from unpacking it to view values.

### Setup


Run the audio client on the machine that you are wanting to capture audio from. (Limitation here is that it requires to be running the windows OS)  
The two options when running the AudioClient are: 'client' and 'server', client will be sending the audio from the device to the target ip and port, server is for local testing and only receives and unpacks the data.

Create a UDP listen server and interpret the received data. [You can see my rough Raspberry Pi implementation here](https://github.com/CurtisDH/AudioClient/blob/main/run_leds.py)

**Arguments are supported but not gracefully**  
If no values are entered it defaults to client with the magnitude threshold set to 75 the target IP: 192.168.1.11  port: 5555  
Expects arguments in the following order:  
type: (client/server)  
<br />
IF SERVER:  
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
will then log the magnitude value, and adjusted magnitude value to the terminal



#### Byte array order

The byte array is in the following structure  
0 NUM_LEDS  
1 R  
2 G  
3 B  
4 Brightness  
5 Speed/Delay  

Delay is currently unused and will always be 0.

## Potentially useful information
Magnitude threshold: Lower values will respond to quieter sounds, higher values will ignore quieter sounds. Keep in mind the magnitude value is captured from the highest value in the FFT set, play around with this value to see what works best for you    
<br />    
On the same note, colour spectrum is based on the magnitude value, so a higher volume will result in less colours, there is definitely a sweet spot for the max colour distribution.  
<br />
Bluetooth devices such as UE boom's when paused for x period of time go into some sort of disconnected state, when this happens you can press any key into the terminal to force the program to re-attach to the default device **once audio playback has been resumed**

