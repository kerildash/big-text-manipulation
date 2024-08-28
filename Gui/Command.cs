using System.Windows.Input;

namespace Gui
{
	class Command : ICommand
	{
		private Action<object> _Execute;
		private Func<object, bool> _CanExecute;

		public Command(Action<object> Execute, Func<object, bool> CanExecute = null)
		{
			_Execute = Execute ?? throw new ArgumentNullException(nameof(Execute));
			_CanExecute = CanExecute;
		}

		public event EventHandler CanExecuteChanged
		{
			add => CommandManager.RequerySuggested += value;
			remove => CommandManager.RequerySuggested -= value;
		}

		public void Execute(object parameter) => _Execute(parameter);
		public bool CanExecute(object parameter) => _CanExecute?.Invoke(parameter) ?? true;
	}
}