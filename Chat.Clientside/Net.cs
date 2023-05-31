using System.Net.Sockets;

namespace Chat.Clientside;

public class Net
{
    private TcpClient _tcpClient = new();

    public void ConnectToServer()
    {
        if (!_tcpClient.Connected)
            _tcpClient.Connect("localhost", 7891);
    }
    
}