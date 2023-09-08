using System;
using Microsoft.Win32;
using System.Windows;

namespace AthChat.Client.Services;

public class DialogService : IDialogService
{
    public void ShowNotification(string message, string caption = "")
    {
        MessageBox.Show(message, caption);
    }
}