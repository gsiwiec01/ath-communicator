using System.Net.Sockets;
using System.Text;
using Chat.Serverside.Net.InputOutput;
using Chat.Shared.Extensions;
using Chat.Shared.Net.InputOutput;

namespace Chat.Serverside.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }

    public TcpClient Socket { get; init; }
    private PacketReader _packetReader { get; set; }

    public User(TcpClient client)
    {
        Id = Guid.NewGuid();
        Socket = client;

        _packetReader = new PacketReader(Socket.GetStream());

        Console.WriteLine("Work");
        
        var opCode = _packetReader.ReadByte();
        Username = _packetReader.ReadMessage();

        var sb = new StringBuilder();
        sb.Append($"[{DateTime.Now}] User ");
        sb.AppendWithColor(Username, ConsoleColor.Green);
        sb.Append(" connected with id ");
        sb.AppendWithColor(Id.ToString(), ConsoleColor.Green);
        sb.Append(" from ");
        sb.AppendWithColor( Socket.Client.RemoteEndPoint.ToString(), ConsoleColor.Green);

        Console.WriteLine(sb.ToString());
    }
}