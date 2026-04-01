using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class LegislaturePeriodConfiguration : MasterDataBaseConfiguration<LegislaturePeriod>
{
    protected override void ConfigureMasterData(EntityTypeBuilder<LegislaturePeriod> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Property(lp => lp.Created).HasDefaultValueSql("now()")
            .ValueGeneratedOnAdd()
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
        builder.Property(lp => lp.Modified).HasDefaultValueSql("now()")
            .ValueGeneratedOnAddOrUpdate()
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        builder
            .HasMany(lp => lp.Persons)
            .WithMany(p => p.LegislaturePeriods)
            .UsingEntity(j => j.ToTable("person_legislature_period"));
    }
}
