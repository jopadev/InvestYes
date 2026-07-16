using InvestYes.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvestYes.Infrastructure.Persistence
{
    public class InvestYesContext : DbContext
    {
        public InvestYesContext(
            DbContextOptions<InvestYesContext> options)
            : base(options)
        {
        }

        public DbSet<Asset> Assets => Set<Asset>();
        public DbSet<User> Users => Set<User>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(InvestYesContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}


