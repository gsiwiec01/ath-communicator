using Chat.Clientside.Core;

namespace Chat.Clientside.ViewModel;

public class MainViewModel
{
    public RelayCommand ConnectToServerCommand { get; set; }

    private Net _net;

    public MainViewModel()
    {
        _net = new Net();
        ConnectToServerCommand = new RelayCommand(x => _net.ConnectToServer());
    }
}