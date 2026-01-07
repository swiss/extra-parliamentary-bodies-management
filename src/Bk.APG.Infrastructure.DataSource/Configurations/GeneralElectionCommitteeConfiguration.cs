using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class GeneralElectionCommitteeConfiguration : EntityBaseConfiguration<GeneralElectionCommittee>
{
    protected override void ConfigureEntity(EntityTypeBuilder<GeneralElectionCommittee> builder)
    {
        builder.Property(c => c.DescriptionGerman)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.DescriptionFrench)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.DescriptionItalian)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.DescriptionRomansh)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.OldLegalForm)
            .HasMaxLength(100);

        builder.Property(c => c.LegalBase)
            .HasMaxLength(2000);

        builder.Property(c => c.LinkAuthorityWebsite)
            .HasMaxLength(500);

        builder.Property(c => c.LinkHomepageGerman)
            .HasMaxLength(500);

        builder.Property(c => c.LinkHomepageFrench)
            .HasMaxLength(500);

        builder.Property(c => c.LinkHomepageItalian)
            .HasMaxLength(500);

        builder.Property(c => c.LinkHomepageRomansh)
            .HasMaxLength(500);

        builder.Property(c => c.RemarksBaseData)
            .HasMaxLength(1000);

        builder.Property(c => c.RemarksBaseDataAdmin)
            .HasMaxLength(1000);

        builder
            .HasOne(m => m.Committee)
            .WithMany(m => m.GeneralElectionCommittees)
            .HasForeignKey(m => m.CommitteeId);

        builder
            .HasOne(c => c.Department)
            .WithMany(d => d.GeneralElectionCommittees)
            .HasForeignKey(c => c.DepartmentId);

        builder
            .HasOne(c => c.Office)
            .WithMany(o => o.GeneralElectionCommittees)
            .HasForeignKey(c => c.OfficeId);

        builder
            .HasOne(c => c.CommitteeLevel)
            .WithMany(cl => cl.GeneralElectionCommittees)
            .HasForeignKey(c => c.CommitteeLevelId);

        builder
            .HasOne(c => c.CommitteeType)
            .WithMany(ct => ct.GeneralElectionCommittees)
            .HasForeignKey(c => c.CommitteeTypeId);

        builder
            .HasOne(c => c.TermOfOffice)
            .WithMany(too => too.GeneralElectionCommittees)
            .HasForeignKey(c => c.TermOfOfficeId);

        builder
            .HasOne(c => c.LegalForm)
            .WithMany(lf => lf.GeneralElectionCommittees)
            .HasForeignKey(c => c.LegalFormId);

        builder
            .HasOne(t => t.TermOfOfficeDate)
            .WithMany(tod => tod.GeneralElectionCommittees)
            .HasForeignKey(c => c.TermOfOfficeDateId);

        builder
            .HasOne(c => c.CandidateListState)
            .WithMany(cls => cls.GeneralElectionCommittees)
            .HasForeignKey(c => c.CandidateListStateId);

        builder.Property(c => c.RowVersion).IsRowVersion();
    }
}
