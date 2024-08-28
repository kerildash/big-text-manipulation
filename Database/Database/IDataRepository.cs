using System.Numerics;
using TextGeneration;

namespace Infrastructure.Database;

public interface IDataRepository
{
	//public AppDbContext _context { get; set; }
    void NewContext();
	Task CreateAsync(DataDto data);
    Task SaveAsync();
    Task<Int128> GetSumOfIntegerAsync();
    Task<double> GetMedianOfRealAsync();
}
