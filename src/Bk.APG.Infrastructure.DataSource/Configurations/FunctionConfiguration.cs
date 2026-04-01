using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class FunctionConfiguration : MasterDataBaseConfiguration<Function>
{
    protected override void ConfigureMasterData(EntityTypeBuilder<Function> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Property(e => e.TextFemaleDe).IsRequired().HasMaxLength(250);
        builder.Property(e => e.TextFemaleFr).IsRequired().HasMaxLength(250);
        builder.Property(e => e.TextFemaleIt).IsRequired().HasMaxLength(250);
        builder.Property(e => e.TextFemaleRm).HasMaxLength(250);
    }
}
