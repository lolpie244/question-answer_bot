using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BotSettings;

public class StageModel
{
	[Key, MaxLength(12)]
	public long user_id { get; set; }
	public string? StageName { get; set; }
}

public class InMemoryContext : DbContext
{
	public InMemoryContext() : base() { }

	public InMemoryContext(DbContextOptions<InMemoryContext> options) : base(options) { }

	public DbSet<StageModel> Stages { get; set; }
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseModel(InMemoryContextModel.Instance).UseInMemoryDatabase("db");
		base.OnConfiguring(optionsBuilder);
	}
}
