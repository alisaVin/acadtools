using System;
using System.Windows.Input;

namespace ACADTools.ViewModels
{
    /// <summary>
    /// Provides a type implementing ICommand
    /// </summary>
    public class RelayCommand : ICommand
    {
        readonly Action<object> execute; //function call
        readonly Func<object, bool> canExecute;

        /// <summary>
        /// Event indicating that the returned value of the predicte changed.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Creates a new instance of RelayCommand.
        /// </summary>
        /// <param name="execute">Action to execute.</param>
        /// <param name="canExecute">Predicate indicating if the action can be executed.</param>
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Executes the predicate passed as parameter to the constructor.
        /// </summary>
        /// <param name="parameter">Predicate parameter (may be null).</param>
        /// <returns>Result of the predictae execution.</returns>
        public void Execute(object parameter) => execute(parameter);

        /// <summary>
        /// Executes the action passed as parameter to the constructor.
        /// </summary>
        /// <param name="parameter">Action parameter (may be null).</param>
        public bool CanExecute(object parameter) => canExecute == null || canExecute(parameter);


    }
}
