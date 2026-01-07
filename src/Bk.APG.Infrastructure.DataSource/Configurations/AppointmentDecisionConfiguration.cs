using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class AppointmentDecisionConfiguration : EntityBaseConfiguration<AppointmentDecision>
{
    protected override void ConfigureEntity(EntityTypeBuilder<AppointmentDecision> builder)
    {
        builder.Property(cp => cp.OgdId)
            .ValueGeneratedOnAdd();

        builder.HasIndex(cp => cp.OgdId)
            .IsUnique();

        builder.Property(a => a.Text)
            .HasMaxLength(2000);

        builder.Property(a => a.Link)
            .HasMaxLength(250);

        builder.Property(a => a.AppointmentDecisionDate)
            .IsRequired();

        builder
            .HasOne(a => a.Committee)
            .WithMany(c => c.AppointmentDecisions)
            .HasForeignKey(a => a.CommitteeId);

        builder
            .HasOne(a => a.AppointmentDecisionType)
            .WithMany()
            .HasForeignKey(a => a.AppointmentDecisionTypeId);

        builder
            .HasOne(a => a.AppointmentDecisionLinkType)
            .WithMany()
            .HasForeignKey(a => a.AppointmentDecisionLinkTypeId);

        builder
            .HasOne(a => a.FileReferenceGerman)
            .WithMany()
            .HasForeignKey(a => a.FileReferenceGermanId);

        builder
            .HasOne(a => a.FileReferenceFrench)
            .WithMany()
            .HasForeignKey(a => a.FileReferenceFrenchId);

        builder
            .HasOne(a => a.FileReferenceItalian)
            .WithMany()
            .HasForeignKey(a => a.FileReferenceItalianId);

        builder
            .HasOne(a => a.FileReferenceRomansh)
            .WithMany()
            .HasForeignKey(a => a.FileReferenceRomanshId);

        builder
            .HasOne(a => a.OriginalDocument)
            .WithMany()
            .HasForeignKey(a => a.OriginalDocumentId);

        builder.Property(a => a.RowVersion).IsRowVersion();
    }
}
