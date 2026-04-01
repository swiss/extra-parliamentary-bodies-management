using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class FormLetterSenderConfiguration : EntityBaseConfiguration<FormLetterSender>
{
    protected override void ConfigureEntity(EntityTypeBuilder<FormLetterSender> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Surname)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.GivenName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.StreetGerman)
            .HasMaxLength(100);

        builder.Property(x => x.StreetFrench)
            .HasMaxLength(100);

        builder.Property(x => x.StreetItalian)
            .HasMaxLength(100);

        builder.Property(x => x.StreetRomansh)
            .HasMaxLength(100);

        builder.Property(x => x.Zip)
            .HasMaxLength(10);

        builder.Property(x => x.CityGerman)
            .HasMaxLength(100);

        builder.Property(x => x.CityFrench)
            .HasMaxLength(100);

        builder.Property(x => x.CityItalian)
            .HasMaxLength(100);

        builder.Property(x => x.CityRomansh)
            .HasMaxLength(100);

        builder.Property(x => x.Email)
            .HasMaxLength(150);

        builder.Property(x => x.Phone)
            .HasMaxLength(20);

        builder.Property(x => x.Website)
            .HasMaxLength(500);

        builder
            .HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => x.DepartmentId);

        builder
            .HasOne(x => x.Office)
            .WithMany()
            .HasForeignKey(x => x.OfficeId);

        builder
            .HasOne(x => x.SenderFunction)
            .WithMany()
            .HasForeignKey(x => x.SenderFunctionId);

        builder
            .HasOne(x => x.SignatureFileReference)
            .WithMany()
            .HasForeignKey(x => x.SignatureFileReferenceId);

        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}
