using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace RhenusCodingChallenge.Infrastructure.Database.Context
{
    public class EventStorageDbContext : DbContext
    {
        public DbSet<DomainEventStorageObject> DomainEventStorageDbSet { get; set; }

        public EventStorageDbContext(DbContextOptions<EventStorageDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
