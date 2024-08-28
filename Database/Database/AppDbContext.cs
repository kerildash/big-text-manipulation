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

		optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-LBS7186\SQLEXPRESS;Initial Catalog=textdb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");

	}

}