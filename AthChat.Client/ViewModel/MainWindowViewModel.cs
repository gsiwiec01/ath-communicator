using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AthChat.Client.Commands;
using AthChat.Client.Enums;
using AthChat.Client.Model;
using AthChat.Client.Services;
using AthChat.Server.Dtos;

namespace AthChat.Client.ViewModel;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly IChatService _chatService;
    private readonly IDialogService _dialogService;
    private readonly TaskFactory _taskFactory;

    public MainWindowViewModel()
    {
        _chatService = new ChatService();
        _dialogService = new DialogService();

        _chatService.NewTextMessage += NewTextMessage;
        _chatService.ParticipantLoggedIn += ParticipantLogin;
        _chatService.ParticipantLoggedOut += ParticipantDisconnection;
        _chatService.ParticipantDisconnected += ParticipantDisconnection;
        _chatService.ParticipantReconnected += ParticipantReconnection;
        _chatService.ParticipantTyping += ParticipantTyping;
        _chatService.ConnectionClosed += Disconnected;

        _taskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
    }


    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged([CallerMemberName()] string? name = null)
    {
        if (name != null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    #region LoginViewData

    private string _userName = "";

    public string UserName
    {
        get => _userName;
        set
        {
            Console.Write(value);
            _userName = value;
            OnPropertyChanged();
        }
    }

    private string _password = "";

    public string Password
    {
        get => _password;
        set
        {
            Console.Write(value);
            _password = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region ChatViewData

    private ObservableCollection<Participant> _participants = new ObservableCollection<Participant>();

    public ObservableCollection<Participant> Participants
    {
        get => _participants;
        set
        {
            _participants = value;
            OnPropertyChanged();
        }
    }

    private Participant _selectedParticipant;

    public Participant SelectedParticipant
    {
        get => _selectedParticipant;
        set
        {
            _selectedParticipant = value;
            if (SelectedParticipant.HasSentNewMessage)
                SelectedParticipant.HasSentNewMessage = false;

            OnPropertyChanged();
        }
    }

    private UserNode _userMode;

    public UserNode UserMode
    {
        get => _userMode;
        set
        {
            _userMode = value;
            OnPropertyChanged();
        }
    }

    private string _textMessage;

    public string TextMessage
    {
        get => _textMessage;
        set
        {
            _textMessage = value;
            OnPropertyChanged();
        }
    }

    private bool _isConnected;

    public bool IsConnected
    {
        get => _isConnected;
        set
        {
            _isConnected = value;
            OnPropertyChanged();
        }
    }

    private bool _isLoggedIn;

    public bool IsLoggedIn
    {
        get => _isLoggedIn;
        set
        {
            _isLoggedIn = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region ConnectCommand

    private ICommand _connectCommand;
    public ICommand ConnectCommand => _connectCommand ?? (_connectCommand = new RelayCommandAsync(() => Connect()));

    private async Task<bool> Connect()
    {
        try
        {
            await _chatService.ConnectAsync();
            IsConnected = true;
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    #endregion

    #region LoginCommand

    private ICommand _loginCommand;

    public ICommand LoginCommand
    {
        get { return _loginCommand ??= new RelayCommandAsync(Login, (x) => CanLogin()); }
    }

    private async Task<bool> Login()
    {
        try
        {
            var users = await _chatService.LoginAsync(UserName, Password);
            if (users != null)
            {
                users.ForEach(x => Participants.Add(new Participant {Name = x.UserName}));
                UserMode = UserNode.Chat;
                IsLoggedIn = true;
                return true;
            }
            else
            {
                _dialogService.ShowNotification("Ten użytkownik jest już zalogowany");
                return false;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    private bool CanLogin()
    {
        return !string.IsNullOrEmpty(UserName)
               && UserName.Length >= 2
               && !string.IsNullOrEmpty(Password)
               && Password.Length >= 2
               && IsConnected;
    }

    #endregion

    #region Logout Command

    private ICommand _logoutCommand;

    public ICommand LogoutCommand
    {
        get
        {
            return _logoutCommand ?? (_logoutCommand =
                new RelayCommandAsync(() => Logout(), (o) => CanLogout()));
        }
    }

    private async Task<bool> Logout()
    {
        try
        {
            await _chatService.LogoutAsync();
            UserMode = UserNode.Login;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private bool CanLogout()
    {
        return IsConnected && IsLoggedIn;
    }

    #endregion

    #region TypingCommand

    private ICommand _typingCommand;

    public ICommand TypingCommand
    {
        get
        {
            return _typingCommand ?? (_typingCommand =
                new RelayCommandAsync(() => Typing(), (o) => CanUseTypingCommand()));
        }
    }

    private async Task<bool> Typing()
    {
        try
        {
            await _chatService.TypingAsync(SelectedParticipant.Name);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private bool CanUseTypingCommand()
    {
        return (SelectedParticipant != null && SelectedParticipant.IsLoggedIn);
    }

    #endregion

    #region SendTextMessageCommand

    private ICommand _sendTextMessageCommand;

    public ICommand SendTextMessageCommand
    {
        get
        {
            return _sendTextMessageCommand ?? (_sendTextMessageCommand =
                new RelayCommandAsync(() => SendTextMessage(), (o) => CanSendTextMessage()));
        }
    }

    private async Task<bool> SendTextMessage()
    {
        try
        {
            var recepient = _selectedParticipant.Name;
            await _chatService.SendUnicastMessageAsync(recepient, _textMessage);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
        finally
        {
            ChatMessage msg = new ChatMessage
            {
                Author = UserName,
                Message = _textMessage,
                Time = DateTime.Now,
                IsOriginNative = true
            };
            SelectedParticipant.Chatter.Add(msg);
            TextMessage = string.Empty;
        }
    }

    private bool CanSendTextMessage()
    {
        return (!string.IsNullOrEmpty(TextMessage) && IsConnected &&
                _selectedParticipant != null && _selectedParticipant.IsLoggedIn);
    }

    #endregion

    #region EventHandlers

    private void NewTextMessage(string name, string msg, MessageType mt)
    {
        if (mt != MessageType.Unicast) return;

        var cm = new ChatMessage {Author = name, Message = msg, Time = DateTime.Now};
        var sender = _participants.FirstOrDefault(u => string.Equals(u.Name, name));
        _taskFactory.StartNew(() => sender.Chatter.Add(cm)).Wait();

        if (!(SelectedParticipant != null && sender.Name.Equals(SelectedParticipant.Name)))
        {
            _taskFactory.StartNew(() => sender.HasSentNewMessage = true).Wait();
        }
    }

    private void ParticipantLogin(User user)
    {
        var ptp = Participants.FirstOrDefault(p => string.Equals(p.Name, user.UserName));
        if (!_isLoggedIn || ptp != null) return;
        
        _taskFactory.StartNew(() => Participants.Add(new Participant
        {
            Name = user.UserName
        })).Wait();
    }

    private void ParticipantDisconnection(string name)
    {
        var person = Participants.FirstOrDefault(p => string.Equals(p.Name, name));
        if (person != null) person.IsLoggedIn = false;
    }

    private void ParticipantReconnection(string name)
    {
        var person = Participants.FirstOrDefault(p => string.Equals(p.Name, name));
        if (person != null) person.IsLoggedIn = true;
    }

    private async void Disconnected()
    {
        var connectionTask = _chatService.ConnectAsync();
        await connectionTask.ContinueWith(t =>
        {
            if (t.IsFaulted) return;

            IsConnected = true;
            _chatService.LoginAsync(UserName, Password).Wait();
            IsLoggedIn = true;
        });
    }

    private void ParticipantTyping(string name)
    {
        var person = Participants.FirstOrDefault(p => string.Equals(p.Name, name));
        if (person == null || person.IsTyping) return;

        person.IsTyping = true;
        Observable.Timer(TimeSpan.FromMilliseconds(1500)).Subscribe(t => person.IsTyping = false);
    }

    #endregion
}