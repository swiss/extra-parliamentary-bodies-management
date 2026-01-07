using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class OfficeConfiguration : MasterDataBaseConfiguration<Office>
{
    protected override void ConfigureMasterData(EntityTypeBuilder<Office> builder)
    {
        builder.Property(p => p.DepartmentId).IsRequired();

        builder
            .HasOne(p => p.Department)
            .WithMany(p => p.Offices)
            .HasForeignKey(p => p.DepartmentId);
    }
}
