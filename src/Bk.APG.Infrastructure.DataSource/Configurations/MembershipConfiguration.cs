using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class MembershipConfiguration : EntityBaseConfiguration<Membership>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Membership> builder)
    {
        builder.Property(c => c.OgdId)
            .ValueGeneratedOnAdd();

        builder.HasIndex(c => c.OgdId)
            .IsUnique();

        builder.Property(m => m.OldMembershipAddition)
            .HasMaxLength(2000);

        builder.Property(p => p.Remarks)
            .HasMaxLength(1000);

        builder.Property(p => p.RemarksStatus)
            .HasMaxLength(1000);

        builder
            .HasOne(m => m.Person)
            .WithMany(m => m.Memberships)
            .HasForeignKey(m => m.PersonId);

        builder
            .HasOne(m => m.ElectionOffice)
            .WithMany(m => m.Memberships)
            .HasForeignKey(m => m.ElectionOfficeId);

        builder
            .HasOne(m => m.ElectionType)
            .WithMany(m => m.Memberships)
            .HasForeignKey(m => m.ElectionTypeId);

        builder
            .HasOne(m => m.Function)
            .WithMany(m => m.Memberships)
            .HasForeignKey(m => m.FunctionId);

        builder
            .HasOne(m => m.MembershipAddition)
            .WithMany(m => m.Memberships)
            .HasForeignKey(m => m.MembershipAdditionId);

        builder.Property(m => m.RowVersion).IsRowVersion();
    }
}
