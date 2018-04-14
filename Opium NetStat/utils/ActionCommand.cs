using System;

namespace  Opium_NetStat.utils
{
    
    public class ActionCommand<T> : IActionCommand<T>
    {
        private Action<T> _execute = obj => { };
        private readonly Func<T, bool> _canExecute = obj => true;
        public bool Overridden { get; set; }
        
        public ActionCommand()
        {

        }

        public void OverrideAction(Action<T> action)
        {
            _execute = action;
            Overridden = true;
        }
        
        public ActionCommand(Action<T> execute)
        {
            _execute = execute;
        }

        
        public ActionCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

       public bool CanExecute(object parameter)
        {
            return _canExecute((T)parameter);
        }

         public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                _execute((T)parameter);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

     
        public event EventHandler CanExecuteChanged;
    }
}
