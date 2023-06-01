using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Chat.Serverside.Net.InputOutput;

public class PacketReader : BinaryReader
{
    private NetworkStream _stream;

    public PacketReader(NetworkStream stream) : base(stream)
    {
        _stream = stream;
    }

    public string ReadMessage()
    {
        var length = ReadInt32();
        var buffer = new byte[length];
        _stream.Read(buffer, 0, length);

        return Encoding.UTF8.GetString(buffer);
    }
}