using System;

namespace AthChat.Client.Model;

public class ChatMessage
{
    public string Message { get; set; }
    public string Author { get; set; }
    public DateTime Time { get; set; }
    public bool IsOriginNative { get; set; }
}