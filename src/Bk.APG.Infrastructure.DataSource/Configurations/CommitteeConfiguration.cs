using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class CommitteeConfiguration : EntityBaseConfiguration<Committee>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Committee> builder)
    {
        builder.Property(c => c.CommitteeNumber)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.OgdId)
            .ValueGeneratedOnAdd();

        builder.HasIndex(c => c.OgdId)
            .IsUnique();

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
            .HasOne(c => c.Department)
            .WithMany(d => d.Committees)
            .HasForeignKey(c => c.DepartmentId);

        builder
            .HasOne(c => c.Office)
            .WithMany(o => o.Committees)
            .HasForeignKey(c => c.OfficeId);

        builder
            .HasOne(c => c.CommitteeLevel)
            .WithMany(cl => cl.Committees)
            .HasForeignKey(c => c.CommitteeLevelId);

        builder
            .HasOne(c => c.CommitteeType)
            .WithMany(ct => ct.Committees)
            .HasForeignKey(c => c.CommitteeTypeId);

        builder
            .HasOne(c => c.TermOfOffice)
            .WithMany(too => too.Committees)
            .HasForeignKey(c => c.TermOfOfficeId);

        builder
            .HasOne(c => c.LegalForm)
            .WithMany(lf => lf.Committees)
            .HasForeignKey(c => c.LegalFormId);

        builder
            .HasOne(t => t.TermOfOfficeDate)
            .WithMany(tod => tod.Committees)
            .HasForeignKey(c => c.TermOfOfficeDateId);

        builder
            .HasMany(x => x.MembershipAdditionsInGeneralElection)
            .WithMany(x => x.Committees)
            .UsingEntity(x => x.ToTable("committee_membership_addition"));

        builder.Property(c => c.RowVersion).IsRowVersion();
    }
}
