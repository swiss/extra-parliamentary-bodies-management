using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class WorklistTaskConfiguration : EntityBaseConfiguration<WorklistTask>
{
    protected override void ConfigureEntity(EntityTypeBuilder<WorklistTask> builder)
    {
        builder.Property(p => p.DueDate)
            .IsRequired();

        builder
            .HasOne(p => p.ParentTask)
            .WithMany(p => p.ChildTasks)
            .HasForeignKey(p => p.ParentTaskId);

        builder
            .HasOne(p => p.WorklistTaskType)
            .WithMany()
            .HasForeignKey(p => p.WorklistTaskTypeId);

        builder
            .HasOne(p => p.WorklistTaskState)
            .WithMany()
            .HasForeignKey(p => p.WorklistTaskStateId);

        builder
            .HasOne(p => p.Department)
            .WithMany()
            .HasForeignKey(p => p.DepartmentId);

        builder
            .HasOne(p => p.Office)
            .WithMany()
            .HasForeignKey(p => p.OfficeId);

        builder
            .HasOne(p => p.Committee)
            .WithMany()
            .HasForeignKey(p => p.CommitteeId);

        builder
            .HasOne(p => p.Membership)
            .WithMany()
            .HasForeignKey(p => p.MembershipId);

        builder
            .HasOne(p => p.Person)
            .WithMany()
            .HasForeignKey(p => p.PersonId);

        builder
            .HasOne(p => p.GeneralElectionCommittee)
            .WithMany()
            .HasForeignKey(p => p.GeneralElectionCommitteeId);

        builder
            .HasOne(p => p.MembershipCandidate)
            .WithMany()
            .HasForeignKey(p => p.MembershipCandidateId);

        builder
            .HasOne(p => p.TermOfOfficeDate)
            .WithMany()
            .HasForeignKey(p => p.TermOfOfficeDateId);
    }
}
