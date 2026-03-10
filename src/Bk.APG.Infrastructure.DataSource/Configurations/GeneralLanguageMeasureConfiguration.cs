using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class GeneralLanguageMeasureConfiguration : EntityBaseConfiguration<GeneralLanguageMeasure>
{
    protected override void ConfigureEntity(EntityTypeBuilder<GeneralLanguageMeasure> builder)
    {
        builder.Property(x => x.Description)
            .IsRequired();

        builder
            .HasOne(x => x.Department)
            .WithOne(d => d.GeneralLanguageMeasure)
            .HasForeignKey<GeneralLanguageMeasure>(x => x.DepartmentId);
    }
}
