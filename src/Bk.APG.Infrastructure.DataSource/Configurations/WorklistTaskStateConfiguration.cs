using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class WorklistTaskStateConfiguration : MasterDataBaseConfiguration<WorklistTaskState>
{
    protected override void ConfigureMasterData(EntityTypeBuilder<WorklistTaskState> builder)
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
