using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RhenusCodingChallenge.Infrastructure.Database.Configurations
{
    public class DomainEventStorageObjectEntityTypeConfiguration : IEntityTypeConfiguration<DomainEventStorageObject>
    {
        public void Configure(EntityTypeBuilder<DomainEventStorageObject> builder)
        {
            builder.Property(x => x.AggregateId)
                .IsRequired();

            builder.Property(x => x.Version)
                .IsRequired();

            builder.Property(x => x.EventData)
                .IsRequired();

            builder.Property(x => x.Timestamp)
                .IsRequired();

            builder.HasKey(x => new { x.AggregateId, x.Version });
        }
    }
}
