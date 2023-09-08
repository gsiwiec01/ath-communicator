using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AthChat.Client.Enums;
using AthChat.Server.Dtos;

namespace AthChat.Client.Services;

public interface IChatService
{
    event Action<User> ParticipantLoggedIn;
    event Action<string> ParticipantLoggedOut;
    event Action<string> ParticipantDisconnected;
    event Action<string> ParticipantReconnected;
    event Action<string, string, MessageType> NewTextMessage;
    event Action<string> ParticipantTyping;
    event Action ConnectionClosed;

    public Task ConnectAsync();
    Task<List<User>> LoginAsync(string name, string password);
    Task LogoutAsync();

    Task SendBroadcastMessageAsync(string msg);
    Task SendUnicastMessageAsync(string recepient, string msg);
    Task TypingAsync(string recepient);
}