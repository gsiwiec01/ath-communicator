using System.Collections.ObjectModel;
using Chat.Clientside.Core;
using Chat.Clientside.Model;

namespace Chat.Clientside.ViewModel;

public class MainViewModel
{
    public ObservableCollection<UserModel> UserModels { get; set; } = new();
}