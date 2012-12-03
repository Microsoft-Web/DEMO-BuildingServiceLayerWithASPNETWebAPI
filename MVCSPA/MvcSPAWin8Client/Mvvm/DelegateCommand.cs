using System;
using System.Windows.Input;

namespace MvcSPAWin8Client.Mvvm
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return (_canExecute == null) || _canExecute.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _execute.Invoke(parameter);
        }
    }
}
