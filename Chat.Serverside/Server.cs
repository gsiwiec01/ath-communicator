using System.Net;
using System.Net.Sockets;
using Chat.Serverside.Entities;
using Chat.Shared.Net.InputOutput;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Chat.Serverside;

public class Server
{
    private List<User> _users;
    
    private TcpListener _listener;

    public static readonly ILogger _logger;
    public static readonly IConfiguration _config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .Build();

    public Server()
    {
        _users = new List<User>();
        
        _listener = new TcpListener(
            IPAddress.Parse(_config.GetValue<string>("ipAddress")),
            _config.GetValue<int>("port")
        );
        _listener.Start();
        
        for (;;)
        {
            var user = new User(_listener.AcceptTcpClient());
            _users.Add(user);
            
            BroadcastConnection();
        }
    }

    private void BroadcastConnection()
    {
        foreach (var user in _users)
        {
            foreach (var usr in _users)
            {
                var packager = new PacketBuilder();
                
                packager.WriteOpCode(1);
                packager.WriteString(usr.Username);
                packager.WriteString(usr.Id.ToString());
                
                user.Socket.Client.Send(packager.GetPacket());
            }
        }
    }
}