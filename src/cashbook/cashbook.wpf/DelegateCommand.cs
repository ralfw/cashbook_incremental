using System;
using System.Windows.Input;

namespace cashbook.wpf
{
    public class DelegateCommand : ICommand
    {
        private readonly Action _onClick;
        private readonly Func<bool> _canExecute;

        public DelegateCommand(Action onClick, Func<bool> canExecute = null)
        {
            _onClick = onClick ?? (() => { });
            _canExecute = canExecute ?? (() => true);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public void Execute(object parameter)
        {
            _onClick();
        }

        public void CheckPossibleCanExecuteChange()
        {
            var e = CanExecuteChanged;
            if (e != null)
                e.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler CanExecuteChanged;
    }
}