using System;
using System.Windows.Input;

namespace Quiz.Core
{
	public class RelayCommand : ICommand
	{
		private Action<object> execute;
		private Predicate<object> canExecute;

		public RelayCommand(Action<object> execute, Predicate<object> canExecute)
		{
			this.execute = execute;
			this.canExecute = canExecute;
		}

		public event EventHandler CanExecuteChanged
		{
			add
			{
				if (canExecute != null) CommandManager.RequerySuggested += value;
			}
			remove
			{
				if (canExecute != null) CommandManager.RequerySuggested -= value;
			}
		}

		public bool CanExecute(object parameter)
		{

			return canExecute == null ? true : canExecute(parameter);
		}

		public void Execute(object parameter)
		{
			execute(parameter);
		}
	}
}
