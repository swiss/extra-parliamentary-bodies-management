using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class GeneralGenderMeasureConfiguration : EntityBaseConfiguration<GeneralGenderMeasure>
{
    protected override void ConfigureEntity(EntityTypeBuilder<GeneralGenderMeasure> builder)
    {
        builder.Property(x => x.Description)
            .IsRequired();

        builder
            .HasOne(x => x.Department)
            .WithOne(d => d.GeneralGenderMeasure)
            .HasForeignKey<GeneralGenderMeasure>(x => x.DepartmentId);
    }
}
