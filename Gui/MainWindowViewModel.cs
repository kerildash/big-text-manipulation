using Gui.Services;
using Infrastructure;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using System.Reflection;
using System.IO;
using TextGeneration;
using AsyncAwaitBestPractices.MVVM;
using Infrastructure.Database;
using System.Numerics;

namespace Gui;

public class MainWindowViewModel : INotifyPropertyChanged
{
	public MainWindowViewModel(DialogService dialog, FileManipulator manipulator, IWriter writer, IDataRepository repository)
	{
		//services
		_dialog = dialog;
		_manipulator = manipulator;
		_writer = writer;
		_repository = repository;

		//commands
		ExportFileToDbAsync = new AsyncCommand(OnExportFileToDbAsyncExecutedAsync, CanExportFileToDbAsyncExecute);
		OpenDirectory = new Command(OnOpenDirectoryExecuted, CanOpenDirectoryExecute);
		
		GenerateFilesAsync = new AsyncCommand(OnGenerateFilesAsyncExecutedAsync, CanGenerateFilesAsyncExecute);
		ConcatenateFilesAsync = new AsyncCommand(OnConcatenateFilesAsyncExecutedAsync, CanConcatenateFilesAsyncExecute);
		GetMedianOfRealsInDbAsync = new AsyncCommand(OnGetMedianOfRealsInDbAsyncExecutedAsync, CanGetMedianOfRealsInDbAsyncExecute);
		GetSumOfIntegersInDbAsync = new AsyncCommand(OnGetSumOfIntegersInDbAsyncExecutedAsync, CanGetSumOfIntegersInDbAsyncExecute);

		//trackable properties
		FilesToGenerate = 100;
		WorkingDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Files");
		SubstringToDelete = String.Empty;

	}

	private readonly IDataRepository _repository;
	private readonly IWriter _writer;
	private readonly DialogService _dialog;
	private readonly FileManipulator _manipulator;


	private string _substringToDelete;
	public string SubstringToDelete
	{
		get
		{
			return _substringToDelete;
		}
		set
		{
			_substringToDelete = value;
			OnPropertyChanged();
		}
	}

	private string _workingDirectory;
	public string WorkingDirectory
	{
		get
		{
			return _workingDirectory;
		}
		set
		{
			_workingDirectory = value;
			OnPropertyChanged();
		}
	}
	private int _filesToGenerate;
	public int FilesToGenerate
	{
		get
		{
			return _filesToGenerate;
		}
		set
		{
			_filesToGenerate = value;
			OnPropertyChanged();
		}
	}
	private string Substring { get; set; } = String.Empty;

	#region commands
	public IAsyncCommand GetSumOfIntegersInDbAsync { get; }
	public async Task OnGetSumOfIntegersInDbAsyncExecutedAsync()
	{
		try
		{
			Int128 sum = await _repository.GetSumOfIntegerAsync();
			_dialog.ShowMessage($"The sum of integer numbers in database is {sum}");
		}
		catch (Exception e)
		{
			_dialog.ShowMessage(e.ToString());
		}
	}
	public bool CanGetSumOfIntegersInDbAsyncExecute(object parameter) => true;

	public IAsyncCommand GetMedianOfRealsInDbAsync { get; }
	public async Task OnGetMedianOfRealsInDbAsyncExecutedAsync()
	{
		try
		{
			double median = await _repository.GetMedianOfRealAsync();
			_dialog.ShowMessage($"The median of decimal numbers in database is {median:n8}");
		}
		catch (Exception e)
		{
			_dialog.ShowMessage(e.ToString());
		}
	}
	public bool CanGetMedianOfRealsInDbAsyncExecute(object parameter) => true;
	public IAsyncCommand ExportFileToDbAsync { get; }
	public async Task OnExportFileToDbAsyncExecutedAsync()
	{
		try
		{
			string filePath = string.Empty;
			if (_dialog.OpenFile(WorkingDirectory, out filePath) == true)
			{
				ProgressDialogWindow progressDialog = new ProgressDialogWindow();
				progressDialog.DataContext = new ProgressDialogWindowViewModel(false, "Importing in progress.");

				progressDialog.Show();
				ProgressDialogWindowViewModel dialogContext = progressDialog.DataContext as ProgressDialogWindowViewModel;

				dialogContext.StatusMessage = $"Calculating the amount of rows.";
				_manipulator.Notify += (all, current) =>
				{
					dialogContext.Status = current * 100 / all;
					dialogContext.StatusMessage = $"{current}/{all} lines imported to database.";
				};
				
				await _manipulator.WriteFileDataToDbAsync(filePath);

				dialogContext.Status = 100;
				progressDialog.Close();
			}

		}
		catch (Exception e)
		{
			_dialog.ShowMessage(e.ToString());
		}
	}
	public bool CanExportFileToDbAsyncExecute(object parameter) => true;


	public ICommand OpenDirectory { get; }
	public void OnOpenDirectoryExecuted(object parameter)
	{
		try
		{
			string chosenDirectory = string.Empty;
			if (_dialog.OpenFolder(WorkingDirectory, out chosenDirectory) == true)
			{
				WorkingDirectory = chosenDirectory;
			}
		}
		catch (Exception e)
		{
			_dialog.ShowMessage(e.Message);
		}
	}
	public bool CanOpenDirectoryExecute(object parameter) => true;


	

	public IAsyncCommand GenerateFilesAsync { get; }
	public async Task OnGenerateFilesAsyncExecutedAsync()
	{
		try
		{

			ProgressDialogWindow progressDialog = new ProgressDialogWindow();
			progressDialog.DataContext = new ProgressDialogWindowViewModel(false, "Files are generating...");

			progressDialog.Show();
			ProgressDialogWindowViewModel dialogContext = progressDialog.DataContext as ProgressDialogWindowViewModel;
			for (int i = 0; i < FilesToGenerate; i++)
			{
				dialogContext.Status = i * 100 / FilesToGenerate;
				dialogContext.StatusMessage = $"{i}/{FilesToGenerate} files generated";
				await _writer.Write(stringsAmount: 100_000, directory: WorkingDirectory);
			}
			dialogContext.Status = 100;
			progressDialog.Close();

		}
		catch (Exception e)
		{
			_dialog.ShowMessage(e.Message);
		}
	}
	public bool CanGenerateFilesAsyncExecute(object parameter) => FilesToGenerate >= 1 ? true : false;


	public IAsyncCommand ConcatenateFilesAsync { get; }
	public async Task OnConcatenateFilesAsyncExecutedAsync()
	{
		try
		{
			ProgressDialogWindow progressDialog = new ProgressDialogWindow();
			progressDialog.DataContext = new ProgressDialogWindowViewModel(true, "Concatenating in  progress...");
			
			progressDialog.Show();
			ProgressDialogWindowViewModel dialogContext = progressDialog.DataContext as ProgressDialogWindowViewModel;

			//dialogContext.Status = 50;
			int deleted = await _manipulator.ConcatenateAllFilesAsync(WorkingDirectory, SubstringToDelete);
			progressDialog.Close();
			_dialog.ShowMessage($"{deleted} lines deleted");

		}
		catch (Exception e)
		{
			_dialog.ShowMessage(e.Message);
		}
	}
	public bool CanConcatenateFilesAsyncExecute(object parameter) =>  true;
	#endregion

	public event PropertyChangedEventHandler PropertyChanged;
	public void OnPropertyChanged([CallerMemberName] string prop = "")
	{
		if (PropertyChanged != null)
		{
			PropertyChanged(this, new PropertyChangedEventArgs(prop));
		}
	}
}
