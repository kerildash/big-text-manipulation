using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Numerics;

namespace Infrastructure.Database;

public class DataRepository : IDataRepository
{
	public DataRepository(AppDbContext context)
	{
		_context = context;
		_context.ChangeTracker.AutoDetectChangesEnabled = false;
	}
	public AppDbContext _context { get; set; }

	/// <summary>
	/// Replaces the AppDbContext with the new one.
	/// Use this method to avoid the memory leak when
	/// inserting huge amount of instances to a database.
	/// </summary>
	/// <returns></returns>
	public void NewContext()
	{
		_context = new AppDbContext(_context.Options);
	}
    public async Task CreateAsync(DataDto data)
    {
        await _context.AddAsync(data);
    }

	public async Task<double> GetMedianOfRealAsync()
	{
		int count = _context.Data.Count();
		double[] sorted = await _context.Data.Select(data => data.Real).OrderBy(d => d).ToArrayAsync();
		if(count % 2 == 1)
		{
			return sorted[count / 2];
		}
		return (sorted[count / 2] + sorted[count / 2 - 1]) / 2d;
	}
	
	public async Task<Int128> GetSumOfIntegerAsync()
	{
		List<Int128> list = await _context.Data.Select(data => (Int128)data.Integer).ToListAsync();
        Int128 result = 0;
		foreach (Int128 num in list)
			result += num;
		return result;
	}

	public async Task SaveAsync()
	{
		await _context.SaveChangesAsync();//throw new NotImplementedException();
	}
}
