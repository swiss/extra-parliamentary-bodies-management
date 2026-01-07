using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public abstract class MasterDataBaseConfiguration<T> : IEntityTypeConfiguration<T> where T : MasterDataBase
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(c => c.OgdId)
            .ValueGeneratedOnAdd();

        builder.HasIndex(c => c.OgdId)
            .IsUnique();

        builder.Property(e => e.Uri).IsRequired().HasMaxLength(255);
        builder.HasIndex(e => e.Uri).IsUnique();

        builder.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.Property(e => e.TextDe).IsRequired().HasMaxLength(250);
        builder.Property(e => e.TextFr).IsRequired().HasMaxLength(250);
        builder.Property(e => e.TextIt).IsRequired().HasMaxLength(250);
        builder.Property(e => e.TextRm).HasMaxLength(250);
        builder.Property(e => e.DescriptionDe).IsRequired().HasMaxLength(1000);
        builder.Property(e => e.DescriptionFr).IsRequired().HasMaxLength(1000);
        builder.Property(e => e.DescriptionIt).IsRequired().HasMaxLength(1000);
        builder.Property(e => e.DescriptionRm).HasMaxLength(1000);
        builder.Property(e => e.Created).IsRequired();
        builder.Property(e => e.CreatedBy).IsRequired().HasMaxLength(250);
        builder.Property(e => e.Modified).IsRequired();
        builder.Property(e => e.ModifiedBy).IsRequired().HasMaxLength(250);
        builder.Property(e => e.Sort).IsRequired().HasDefaultValue(0);

        ConfigureMasterData(builder);
    }

    protected abstract void ConfigureMasterData(EntityTypeBuilder<T> builder);
}
