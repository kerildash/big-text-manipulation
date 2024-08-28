using System.IO;
using System.IO.Enumeration;
using System.Text;

namespace TextGeneration;

public class ToFileWriter(Generator generator) : IWriter
{
	private readonly Generator Generator = generator;
    public async Task Write(int stringsAmount, string directory)
	{
		string fileName = $"{DateTime.Now:dd.MM.yyyy hh-mm-ss-ffff}.txt";
		string filePath = Path.Combine(directory, fileName);

		using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8, 65536)) // 64KB buffer size
		{
			for (int i = 0; i < stringsAmount; i++)
			{
				await sw.WriteLineAsync(Generator.GenerateDataString());
			}
		}
	}

}
