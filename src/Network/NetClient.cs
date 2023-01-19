using System.Net;
using System.Net.Sockets;

namespace AudioClient.Network;

public class NetClient
{
    private string ip = "";
    private int port = 0;
    private IPEndPoint ServerEndPoint;
    private UdpClient client;

    public NetClient(string ip, int port)
    {
        // Create an IPEndPoint to represent the server
        ServerEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

        // Create a UdpClient to send data to the server
        client = new UdpClient();
    }

    public void SendData(byte[] data)
    {
        // Send the data to the server
        client.Send(data, data.Length, ServerEndPoint);

        // Close the UdpClient
        client.Close();
    }
}