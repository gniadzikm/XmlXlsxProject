using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XmlXslxProject.UI.ViewModels
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _action;

        public RelayCommand(Action<T> action)
        {
            _action = action;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (parameter == null)
            {
                return;
            }

            _action((T)parameter);
        }
    }
}
