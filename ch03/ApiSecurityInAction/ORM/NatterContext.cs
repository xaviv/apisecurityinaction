using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApiSecurityInAction.ORM
{
	public class NatterContext : DbContext
	{
		public NatterContext(DbContextOptions<NatterContext> options) : base(options)
		{

		}

		public DbSet<Space> Spaces { get; set; }
		public DbSet<Message> Messages { get; set; }
		public DbSet<IdentityUser> Users { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Define aggregated primary key
			modelBuilder.Entity<Message>().HasKey(k => new { k.SpaceId, k.MessageId });
		}
	}
}