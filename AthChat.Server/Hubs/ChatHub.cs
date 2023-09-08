using System.Collections.Concurrent;
using AthChat.Server.Dtos;
using AthChat.Server.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace AthChat.Server.Hubs;

public class ChatHub : Hub<IClient>
{
    private static ConcurrentDictionary<string, User> _chatClients = new();
    
    public async Task<List<User>> Login(string name, string password)
    {
        if (_chatClients.ContainsKey(name)) return null;
        
        Console.WriteLine($"++ {name} logged in");
        
        var users = new List<User>(_chatClients.Values);
        var newUser = new User { UserName = name, Id = 0 };
        
        var added = _chatClients.TryAdd(name, newUser);
        if (!added) return null;
        
        await Clients.Others.ParticipantLogin(newUser);
        return users;
    }
}