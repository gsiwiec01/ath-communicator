using System.Text;

namespace Chat.Shared.Net.InputOutput;

public class PacketBuilder
{
    private readonly MemoryStream _memory = new();

    public void WriteOpCode(byte opCode)
    {
        _memory.WriteByte(opCode);
    }
    
    public void WriteString(string str)
    {
        _memory.Write(BitConverter.GetBytes(str.Length));
        _memory.Write(Encoding.UTF8.GetBytes(str));
    }

    public byte[] GetPacket() => _memory.ToArray();
}