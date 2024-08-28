using Win32 = Microsoft.Win32;
using System.Windows;

using System.Windows.Forms;
using System.IO;

namespace Gui.Services
{
	public class DialogService
	{
		public bool OpenFile(string directory, out string filePath)
		{
			try
			{
				var openDialog = new Win32.OpenFileDialog();
				openDialog.InitialDirectory = directory;
				openDialog.Filter = "Text files(*.txt)|*.txt| All files(*.*) |*.*";
				if (openDialog.ShowDialog() == true)
				{
					filePath = openDialog.FileName;
					return true;
				}
				filePath = string.Empty;
				return false;
			}
			catch
			{
				throw;
			}
		}
		public bool OpenFolder(string directory, out string newDirectory)
		{
			try
			{
				var openDialog = new FolderBrowserDialog();
				openDialog.InitialDirectory = directory;
				DialogResult result = openDialog.ShowDialog();
				if (result == DialogResult.OK)
				{
					newDirectory = openDialog.SelectedPath;
					return true;
				}
				newDirectory = string.Empty;
				return false;
			}
			catch
			{
				throw;
			}
		}
		public bool ProgressDialog(string message)
		{
			try
			{
				var progressDialog = new ProgressDialogWindow();
				//progressDialog.DataContext = new ProgressDialogWindowViewModel();
				progressDialog.ShowDialog();


				if (progressDialog.DialogResult == true)
				{
					return true;
				}
				return false;
			}
			catch
			{
				throw;
			}
		}

		public void ShowMessage(string message)
		{
			{
				System.Windows.MessageBox.Show(message);
			}
		}
	}
}
