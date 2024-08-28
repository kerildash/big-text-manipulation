using Microsoft.EntityFrameworkCore;

using System.Configuration;

namespace Infrastructure.Database;

public class AppDbContext : DbContext
{
    public DbContextOptions<AppDbContext> Options { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
		Options = options;
	}

	public DbSet<DataDto> Data { get; set; }
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

	{

		optionsBuilder.UseSqlServer(ConfigurationManager.AppSettings["DefaultConnection"]);

	}

}