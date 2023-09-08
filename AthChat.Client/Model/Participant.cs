using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AthChat.Client.Model;

public class Participant : INotifyPropertyChanged
{
    public string Name { get; set; }
    public ObservableCollection<ChatMessage> Chatter = new();

    public event PropertyChangedEventHandler PropertyChanged;
    
    public void OnPropertyChanged([CallerMemberName()] string? name = null)
    {
        if (name != null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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

    private bool _hasSentNewMessage;
    public bool HasSentNewMessage
    {
        get => _hasSentNewMessage;
        set
        {
            _hasSentNewMessage = value;
            OnPropertyChanged();
        }
    }

    private bool _isTyping;
    public bool IsTyping
    {
        get => _isTyping;
        set
        {
            _isTyping = value;
            OnPropertyChanged();
        }
    }
}