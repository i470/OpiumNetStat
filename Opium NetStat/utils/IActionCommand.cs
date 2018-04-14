using System;
using System.Windows.Input;

namespace  Opium_NetStat.utils
{
    
    public interface IActionCommand<T> : IActionCommand
    {
        
        void OverrideAction(Action<T> action);
    }

    
    public interface IActionCommand : ICommand
    {
        
        bool Overridden { get; set; }

        
        void RaiseCanExecuteChanged();
    }
}