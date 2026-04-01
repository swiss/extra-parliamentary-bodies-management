using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class ContactPointConfiguration : EntityBaseConfiguration<ContactPoint>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ContactPoint> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Property(cp => cp.OgdId)
            .ValueGeneratedOnAdd();

        builder.HasIndex(cp => cp.OgdId)
            .IsUnique();

        builder.Property(cp => cp.CompanyName)
            .HasMaxLength(150);

        builder.Property(cp => cp.Section)
            .HasMaxLength(150);

        builder.Property(cp => cp.Street)
            .HasMaxLength(100);

        builder.Property(cp => cp.Zip)
            .HasMaxLength(10);

        builder.Property(cp => cp.City)
            .HasMaxLength(100);

        builder.Property(cp => cp.Email)
            .HasMaxLength(150);

        builder.Property(cp => cp.Phone)
            .HasMaxLength(20);

        builder.Property(cp => cp.PoBox)
            .HasMaxLength(50);

        builder.Property(cp => cp.PersonalEmail)
            .HasMaxLength(150);

        builder.Property(cp => cp.PersonalPhone)
            .HasMaxLength(20);

        builder.Property(cp => cp.PersonalMobile)
            .HasMaxLength(20);

        builder.Property(cp => cp.Surname)
            .HasMaxLength(150);

        builder.Property(cp => cp.GivenName)
            .HasMaxLength(150);

        builder.Property(cp => cp.Title)
            .HasMaxLength(100);

        builder
            .HasOne(p => p.Gender)
            .WithMany()
            .HasForeignKey(cp => cp.GenderId);

        builder
            .HasOne(cp => cp.Language)
            .WithMany()
            .HasForeignKey(cp => cp.LanguageId);

        builder.Property(cp => cp.RowVersion).IsRowVersion();
    }
}
