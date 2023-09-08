using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Documents;
using AthChat.Client.Enums;
using AthChat.Server.Dtos;
using Microsoft.AspNetCore.SignalR.Client;

namespace AthChat.Client.Services;

public class ChatService : IChatService
{
    public event Action<User> ParticipantLoggedIn;
    public event Action<string> ParticipantLoggedOut;
    public event Action<string> ParticipantDisconnected;
    public event Action<string> ParticipantReconnected;
    public event Action ConnectionClosed;
    public event Action<string, string, MessageType> NewTextMessage;
    public event Action<string> ParticipantTyping;

    private HubConnection _connection;
    private const string url = "http://localhost:5104/chatHub";

    public async Task ConnectAsync()
    {
        _connection = new HubConnectionBuilder().WithUrl(url).Build();

        _connection.On<User>("ParticipantLogin", x => ParticipantLoggedIn.Invoke(x));
        _connection.On<string>("ParticipantLogout", (x) => ParticipantLoggedOut?.Invoke(x));
        _connection.On<string>("ParticipantDisconnection", (x) => ParticipantDisconnected?.Invoke(x));
        _connection.On<string>("ParticipantReconnection", (x) => ParticipantReconnected?.Invoke(x));
        _connection.On<string, string>("BroadcastTextMessage", (x, y) => NewTextMessage?.Invoke(x, y, MessageType.Broadcast));
        _connection.On<string, string>("UnicastTextMessage", (x, y) => NewTextMessage?.Invoke(x, y, MessageType.Unicast));
        _connection.On<string>("ParticipantTyping", (x) => ParticipantTyping?.Invoke(x));
        
        await _connection.StartAsync();
    }

    public async Task<List<User>> LoginAsync(string name, string password)
    {
        return await _connection.InvokeAsync<List<User>>("Login", name, password);
    }

    public Task LogoutAsync()
    {
        throw new NotImplementedException();
    }

    public Task SendBroadcastMessageAsync(string msg)
    {
        throw new NotImplementedException();
    }

    public Task SendUnicastMessageAsync(string recepient, string msg)
    {
        throw new NotImplementedException();
    }

    public Task TypingAsync(string recepient)
    {
        throw new NotImplementedException();
    }
}