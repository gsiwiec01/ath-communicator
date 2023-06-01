using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Chat.Serverside.Net.InputOutput;
using Chat.Shared.Net.InputOutput;

namespace Chat.Clientside.Net;

public class Connection
{
    private readonly TcpClient _tcpClient = new();

    public PacketReader PacketReader { get; private set; }
    public event Action connectedEvent;
    
    public bool ConnectToServer(string ipAddress, int port, string username)
    {
        try
        {
            if (_tcpClient.Connected) return true;
            
            _tcpClient.Connect(ipAddress, port);
            
            PacketReader = new PacketReader(_tcpClient.GetStream());

            if (string.IsNullOrEmpty(username))
            {
                var packet = new PacketBuilder();

                packet.WriteOpCode(0);
                packet.WriteString(username);
                _tcpClient.Client.Send(packet.GetPacket());   
            }
        }
        catch (Exception e)
        {
            return false;
        }

        return true;
    }

    private async Task RoadPackets()
    {
        while (true)
        {
            var opCode = PacketReader.ReadByte();
            switch (opCode)
            {
                case 1:
                    connectedEvent!.Invoke();
                    break;
                
                default:
                    Console.WriteLine("Unknown opcode!");
                    break;
            }
        }
    }
}