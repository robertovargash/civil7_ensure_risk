using System;
using System.Windows.Input;

namespace EnsureRisk.Classess
{
    public class RelayyCommand : ICommand
    {
        #region Fields

        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        #endregion // Fields

        #region Constructors

        public RelayyCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public RelayyCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }
        #endregion // Constructors

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        #endregion // ICommand Members
    }
    public class AnotherCommandImplementation : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public AnotherCommandImplementation(Action<object> execute) : this(execute, null)
        {
        }

        public AnotherCommandImplementation(Action<object> execute, Func<object, bool> canExecute)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));

            _execute = execute;
            _canExecute = canExecute ?? (x => true);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public void Refresh()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }

}
