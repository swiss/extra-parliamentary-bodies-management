using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class PersonConfiguration : EntityBaseConfiguration<Person>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Person> builder)
    {
        builder.Property(c => c.OgdId)
            .ValueGeneratedOnAdd();

        builder.HasIndex(c => c.OgdId)
            .IsUnique();

        builder.Property(p => p.Surname)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.GivenName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.BirthYear)
            .IsRequired();

        builder.Property(p => p.RemarksPersonData)
            .HasMaxLength(1_000);

        builder.Property(p => p.RemarksPersonDataAdmin)
            .HasMaxLength(1_000);

        builder.Property(p => p.Occupation)
            .HasMaxLength(150);

        builder.Property(p => p.Employer)
            .HasMaxLength(150);

        builder.Property(p => p.SalutationText)
            .HasMaxLength(200);

        builder
            .HasOne(p => p.Salutation)
            .WithMany()
            .HasForeignKey(p => p.SalutationId);

        builder
            .HasOne(p => p.Gender)
            .WithMany()
            .IsRequired()
            .HasForeignKey(p => p.GenderId);

        builder
            .HasOne(p => p.Language)
            .WithMany()
            .IsRequired()
            .HasForeignKey(p => p.LanguageId);

        builder
            .HasOne(p => p.CorrespondenceLanguage)
            .WithMany()
            .IsRequired()
            .HasForeignKey(p => p.CorrespondenceLanguageId);

        builder
            .HasOne(p => p.PrivateAddress)
            .WithMany()
            .HasForeignKey(p => p.PrivateAddressId);

        builder
            .HasOne(p => p.OfficeAddress)
            .WithMany()
            .HasForeignKey(p => p.OfficeAddressId);

        builder
            .HasOne(p => p.CorrespondenceAddress)
            .WithMany()
            .HasForeignKey(p => p.CorrespondenceAddressId);

        builder
            .HasOne(p => p.Council)
            .WithMany()
            .HasForeignKey(p => p.CouncilId);

        builder
            .HasOne(p => p.Office)
            .WithMany()
            .HasForeignKey(p => p.OfficeId);

        builder.Property(p => p.CorrespondenceAddressId)
           .HasDefaultValue(null);

        builder.Property(p => p.RowVersion).IsRowVersion();
    }
}
