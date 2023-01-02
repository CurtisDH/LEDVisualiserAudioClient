using System.Net;
using System.Net.Sockets;

namespace AudioClient;

public class NetworkServer
{
    private IPEndPoint _sender;
    private readonly UdpClient _receiveSocket;
    byte[] data = new byte[6];
    private int ledCount;

    public NetworkServer(int port, int dataSizeInBytes)
    {
        data = new byte[dataSizeInBytes];
        var ipEndPoint = new IPEndPoint(IPAddress.Any, port);
        _receiveSocket = new UdpClient(ipEndPoint);
        _sender = new IPEndPoint(IPAddress.Any, 0);
    }

    public void Listen()
    {
        Console.WriteLine("Waiting for a client...");
        while (true)
        {
            data = _receiveSocket.Receive(ref _sender);

            var num_leds = data[0];
            var r = data[1];
            var g = data[2];
            var b = data[3];
            var brightness = data[4];
            var delay = data[5];

            Console.WriteLine($"NUM LEDS:{num_leds} R {r}, G {g}, B {b}, brightness: {brightness}, Delay: {delay}");
        }
    }
}