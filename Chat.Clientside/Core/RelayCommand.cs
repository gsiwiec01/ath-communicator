using System;
using System.Windows.Input;

namespace Chat.Clientside.Core;

public class RelayCommand
{
    private Action<object> _execute;
    private Func<object, bool>? _camExecute;

    public RelayCommand(Action<object> execute, Func<object, bool> camExecute = null)
    {
        _execute = execute;
        _camExecute = camExecute;
    }
    
    public event EventHandler CamExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CamExecute(object parameter) => _camExecute == null || _camExecute(parameter);

    public void Execute(object parameter) => _execute(parameter);
}