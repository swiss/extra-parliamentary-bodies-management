using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class EntityAuditLogConfiguration : IEntityTypeConfiguration<EntityAuditLog>
{
    public void Configure(EntityTypeBuilder<EntityAuditLog> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.AuditData)
            .HasColumnType("jsonb");

        builder.Property(e => e.AuditUser)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.AuditAction)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.EntityType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.EntityPrimaryKey)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.EntitySnapshot)
            .HasColumnType("jsonb")
            .IsRequired();
    }
}
