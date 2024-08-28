using System.ComponentModel.DataAnnotations;

namespace Infrastructure;

public class DataDto
{
	
	public Guid Id { get; set; }
	public required DateTime Date { get; init; }
	[StringLength(10)]
	public required string LatinString { get; init; }
	[StringLength(10)]
	public required string CyrillicString { get; init; }
	public required int Integer { get; init; }
	public required double Real { get; init; }
}
