using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class EiamAssignmentConfiguration : IEntityTypeConfiguration<EiamAssignment>
{
    public void Configure(EntityTypeBuilder<EiamAssignment> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.ExternalId).IsUnique();
        builder.Property(e => e.ExternalId).IsRequired().HasMaxLength(25);

        builder.Property(e => e.Role).HasConversion<string>().HasMaxLength(20);

        builder.HasOne(e => e.Parent).WithMany(e => e.Children).HasForeignKey(e => e.ParentId);

        builder.HasOne(e => e.Department).WithOne(d => d.EiamAssignment).HasForeignKey<EiamAssignment>(e => e.DepartmentId);
        builder.HasOne(e => e.Office).WithOne(o => o.EiamAssignment).HasForeignKey<EiamAssignment>(e => e.OfficeId);
        builder.HasOne(e => e.Committee).WithOne(c => c.EiamAssignment).HasForeignKey<EiamAssignment>(e => e.CommitteeId);

        builder.HasMany(e => e.ReceivedTasks).WithOne(w => w.AssignedTo).HasForeignKey(w => w.AssignedToId);
        builder.HasMany(e => e.DistributedTasks).WithOne(w => w.AssignedBy).HasForeignKey(w => w.AssignedById);
    }
}
