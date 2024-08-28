namespace TextGeneration;

public interface IWriter
{
	Task Write(int stringsAmount, string directory);
}
