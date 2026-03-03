using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class AddressConfiguration : EntityBaseConfiguration<Address>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Address> builder)
    {
        builder.Property(a => a.CompanyName)
            .HasMaxLength(150);

        builder.Property(a => a.Street)
            .HasMaxLength(100);

        builder.Property(a => a.Zip)
            .HasMaxLength(10);

        builder.Property(a => a.City)
            .HasMaxLength(100);

        builder.Property(a => a.CountryCode)
            .HasMaxLength(5);

        builder.Property(a => a.Email)
            .HasMaxLength(150);

        builder.Property(a => a.Phone)
            .HasMaxLength(20);

        builder.Property(a => a.Mobile)
            .HasMaxLength(20);

        builder.Property(a => a.PoBox)
            .HasMaxLength(50);

        builder
            .HasOne(a => a.Canton)
            .WithMany()
            .HasForeignKey(a => a.CantonId);

        builder
            .HasOne(a => a.Country)
            .WithMany()
            .HasForeignKey(a => a.CountryId);
    }
}
