using System;
using System.Windows.Input;

namespace SteamGameNotes
{
    public class DelegateCommand : ICommand
    {
        public Action CommandAction { get; set; }

        public void Execute(object parameter)
        {
            CommandAction();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
