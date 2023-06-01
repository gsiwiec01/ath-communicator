using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Chat.Clientside.Core;
using Chat.Clientside.Model;
using Chat.Clientside.Net;

namespace Chat.Clientside.ViewModel;

public class ConnectionViewModel
{
    public ObservableCollection<UserModel> Users = new();

    public string IpAddress { get; set; }
    public string Username { get; set; }

    public RelayCommand ConnectToServerCommand { get; set; }

    private readonly Connection _connection = new();
    private const string IpAddressPattern = @"[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}:\d+";

    public ConnectionViewModel()
    {
        _connection.connectedEvent += UserConnected;

        ConnectToServerCommand = new RelayCommand(
            x =>
            {
                var split = IpAddress.Split(":");
                if (!int.TryParse(split[1], out var port))
                    throw new Exception("Błąd parsowania portu!");

                _connection.ConnectToServer(split[0], port, Username);
            }
        );
    }

    private bool CheckConnectionData()
    {
        return !string.IsNullOrEmpty(IpAddress) &&
               Regex.IsMatch(IpAddress, IpAddressPattern) &&
               !string.IsNullOrEmpty(Username) &&
               Username.Length >= 3;
    }

    private void UserConnected()
    {
        var user = new UserModel
        {
            Username = _connection.PacketReader.ReadString(),
            Id = _connection.PacketReader.ReadString(),
        };
        
        if (Users.All(x => x.Id != user.Id))
            Application.Current.Dispatcher.Invoke(() => Users.Add(user)); 
    }
}