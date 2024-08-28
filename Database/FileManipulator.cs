using System.Globalization;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class FileManipulator(IDataRepository repository)
{
	private readonly IDataRepository _repository = repository;	
	public async Task<string> ConcatenateAllFilesAsync(string pathToFolder)
	{
		string[] files = Directory.GetFiles(pathToFolder);
		string resultingFilePath = @$"{pathToFolder}\{DateTime.Now:dd.MM.yyyy hh-mm-ss-ffff}.txt";

		using Stream destStream = File.OpenWrite(resultingFilePath);

		foreach (string srcFileName in files)
		{
			using Stream srcStream = File.OpenRead(srcFileName);
			await srcStream.CopyToAsync(destStream);
		}

		return resultingFilePath;
	}
	/// <summary>
	/// Concatenates all files in a specified folder and deletes 
	/// all strings containing <c>substringToSkip</c> in a resulting file.
	/// </summary>
	/// <param name="pathToFolder">A path to a directory where the files to concatenate are located</param>
	/// <param name="substringToSkip">All strings containing that substring will be deleted</param>
	/// <returns>A number of deleted lines.</returns>
	public async Task<int> ConcatenateAllFilesAsync(string pathToFolder, string substringToSkip)
	{
		if (string.IsNullOrEmpty(substringToSkip)) 
		{
			await ConcatenateAllFilesAsync(pathToFolder);
			return 0;
		} 
		string[] files = Directory.GetFiles(pathToFolder);
		string resultingFilePath = @$"{pathToFolder}\{DateTime.Now:dd.MM.yyyy hh-mm-ss-ffff}.txt";

		using Stream destStream = File.OpenWrite(resultingFilePath);
		using StreamWriter destStreamWriter = new StreamWriter(destStream);

		int linesDeleted = 0;
		string? line = string.Empty;

		foreach (string srcFileName in files)
		{
			using Stream srcStream = File.OpenRead(srcFileName);
			using StreamReader reader = new StreamReader(srcStream);

			while ((line = reader.ReadLine()) != null)
			{
				if (!line.Contains(substringToSkip))
				{
					await destStreamWriter.WriteLineAsync(line);
				}
				else
				{
					linesDeleted++;
				}
			}
			await reader.ReadLineAsync();
			await srcStream.CopyToAsync(destStream);
		}
		return linesDeleted;
	}

	public delegate void ProgressHandler(int all, int current);
	public event ProgressHandler? Notify;
	public async Task WriteFileDataToDbAsync(string filePath)
	{
		int lineCount = File.ReadLines(filePath).Count();
		int currentLine = 0; 
		string? line = string.Empty;
		using StreamReader reader = new StreamReader(File.OpenRead(filePath));
		
		try
		{
			while ((line = await reader.ReadLineAsync()) != null)
			{
				Notify.Invoke(lineCount, currentLine);
				DataDto data = TryParseLine(line);
				if (data == null) continue;
				await _repository.CreateAsync(data);
				if (currentLine % 20000 == 0)
				{
					await _repository.SaveAsync();
					_repository.NewContext();
				}
				currentLine++;
			}
		}
		catch
		{
			throw;
		}
		finally
		{
			await _repository.SaveAsync();
			reader.Close();
		}
	}
	public DataDto TryParseLine(string line, string delimiter = "||")
	{
		try
		{
			return ParseLine(line, delimiter);
		}
		catch
		{
			return null;
		}
	}
	public DataDto ParseLine(string line ,string delimiter = "||") 
	{
		try
		{
			string[] segments = line.Split(delimiter, StringSplitOptions.None);
			DataDto data = new DataDto
			{
				Id = Guid.NewGuid(),
				Date = DateTime.ParseExact(segments[0], "dd.MM.yyyy", CultureInfo.InvariantCulture),
				LatinString = segments[1],
				CyrillicString = segments[2],
				Integer = Int32.Parse(segments[3], CultureInfo.InvariantCulture),
				Real = Double.Parse(segments[4], CultureInfo.InvariantCulture)
			};
			return data;
		}
		catch
		{
			throw new ArgumentException("Content of the string does not meet the format");
		}
		
	}
}
