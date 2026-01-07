using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class MembershipCandidateLogMessageConfiguration : EntityBaseConfiguration<MembershipCandidateLogMessage>
{
    protected override void ConfigureEntity(EntityTypeBuilder<MembershipCandidateLogMessage> builder)
    {
        builder.Property(p => p.LogMessage)
                    .IsRequired()
                    .HasMaxLength(2000);

        builder
            .HasOne(p => p.Person)
            .WithMany()
            .HasForeignKey(p => p.PersonId);

        builder
            .HasOne(p => p.GeneralElectionCommittee)
            .WithMany()
            .IsRequired()
            .HasForeignKey(p => p.GeneralElectionCommitteeId);
    }
}
