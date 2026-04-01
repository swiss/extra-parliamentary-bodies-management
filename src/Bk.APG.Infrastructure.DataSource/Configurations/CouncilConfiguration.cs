using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class CouncilConfiguration : MasterDataBaseConfiguration<Council>
{
    protected override void ConfigureMasterData(EntityTypeBuilder<Council> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Property(lp => lp.Created).HasDefaultValueSql("now()")
            .ValueGeneratedOnAdd()
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
        builder.Property(lp => lp.Modified).HasDefaultValueSql("now()")
            .ValueGeneratedOnAddOrUpdate()
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
    }
}
