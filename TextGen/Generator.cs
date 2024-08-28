namespace TextGeneration;

public class Generator
{
	private readonly Random random;
	public Generator()
	{
		random = new Random();
	}
	public string GenerateDataString(string delimiter = "||")
	{
		string dataString = GenerateDate(5).ToString("dd.MM.yyyy")
			+ delimiter + GenerateLetters(Alphabet.Latin)
			+ delimiter + GenerateLetters(Alphabet.Cyrillic)
			+ delimiter + GenerateInteger(1, 100_000_000).ToString()
			+ delimiter + GenerateFloat().ToString("n8") + delimiter;
		return dataString;
	}


	public DateTime GenerateDate(int period)
	{
		DateTime today = DateTime.Now;
		DateTime start = today.AddYears(-period);
		int range = (today - start).Days;
		return start.AddDays(random.Next(range));
	}
	public int GenerateInteger(int minValue, int maxValue)
	{
		return random.Next(minValue, maxValue);
	}

	public double GenerateFloat()
	{
		return random.NextDouble() * 19 + 1;
	}
	public string GenerateLetters(Alphabet alphabet, int length = 10)
	{
		string charSet = string.Empty;

		switch (alphabet)
		{
			case Alphabet.Latin:
				charSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
				break;
			case Alphabet.Cyrillic:
				charSet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя";
				break;
			default:
				throw new InvalidOperationException();
		}

		string stringOfLetters = string.Empty;

		for (int i = 0; i < length; i++)
		{
			char letter = charSet[random.Next(charSet.Length)];
			stringOfLetters += letter;
		}
		return stringOfLetters;
	}
}
