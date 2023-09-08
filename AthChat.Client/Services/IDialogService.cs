namespace AthChat.Client.Services;

public interface IDialogService
{
    public void ShowNotification(string message, string caption = "");
}