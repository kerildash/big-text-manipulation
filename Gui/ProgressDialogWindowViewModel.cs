using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Gui
{
	public class ProgressDialogWindowViewModel : INotifyPropertyChanged
	{
		public ProgressDialogWindowViewModel(bool isIndeterminate, string message = "Please wait...")
		{
			
			Message = message;
			StatusMessage = string.Empty;
			IsIndeterminate = isIndeterminate;
		}
		private bool _isIndeterminate;
		public bool IsIndeterminate
		{
			get
			{
				return _isIndeterminate;
			}
			set
			{
				_isIndeterminate = value;
				OnPropertyChanged();
			}
		}
		private string _statusMessage;
		public string StatusMessage
		{
			get
			{
				return _statusMessage;
			}
			set
			{
				_statusMessage = value;
				OnPropertyChanged();
			}
		}
		public string Message { get; init; }
		private int _status;
		public int Status
		{
			get
			{
				return _status;
			}
			set
			{
				_status = value;
				OnPropertyChanged();
			}
		}


		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged([CallerMemberName] string prop = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(prop));
			}
		}
	}
}
