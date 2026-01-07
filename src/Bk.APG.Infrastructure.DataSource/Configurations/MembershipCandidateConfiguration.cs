using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class MembershipCandidateConfiguration : EntityBaseConfiguration<MembershipCandidate>
{
    protected override void ConfigureEntity(EntityTypeBuilder<MembershipCandidate> builder)
    {
        builder.Property(m => m.Surname)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(m => m.GivenName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(m => m.BirthYear)
            .IsRequired();

        builder.Property(m => m.OldMembershipAddition)
            .HasMaxLength(2000);

        builder.Property(m => m.Remarks)
            .HasMaxLength(1000);

        builder.Property(m => m.RemarksStatus)
            .HasMaxLength(1000);

        builder
            .HasOne(m => m.Membership)
            .WithMany()
            .HasForeignKey(m => m.MembershipId);

        builder
            .HasOne(m => m.Person)
            .WithMany()
            .HasForeignKey(m => m.PersonId);

        builder
            .HasOne(m => m.Gender)
            .WithMany()
            .IsRequired()
            .HasForeignKey(m => m.GenderId);

        builder
            .HasOne(m => m.Language)
            .WithMany()
            .IsRequired()
            .HasForeignKey(m => m.LanguageId);

        builder
            .HasOne(m => m.GeneralElectionCommittee)
            .WithMany(m => m.MembershipCandidates)
            .HasForeignKey(m => m.GeneralElectionCommitteeId);

        builder
            .HasOne(m => m.ElectionOffice)
            .WithMany(m => m.MembershipCandidates)
            .HasForeignKey(m => m.ElectionOfficeId);

        builder
            .HasOne(m => m.ElectionType)
            .WithMany(m => m.MembershipCandidates)
            .HasForeignKey(m => m.ElectionTypeId);

        builder
            .HasOne(m => m.Function)
            .WithMany(m => m.MembershipCandidates)
            .HasForeignKey(m => m.FunctionId);

        builder
            .HasOne(m => m.MembershipAddition)
            .WithMany(m => m.MembershipCandidates)
            .HasForeignKey(m => m.MembershipAdditionId);

        builder.Property(m => m.RowVersion).IsRowVersion();
    }
}
