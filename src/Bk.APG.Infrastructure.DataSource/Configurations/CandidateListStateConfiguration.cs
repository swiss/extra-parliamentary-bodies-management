using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class CandidateListStateConfiguration : MasterDataBaseConfiguration<CandidateListState>
{
    protected override void ConfigureMasterData(EntityTypeBuilder<CandidateListState> builder)
    {
        builder.Property(lp => lp.Created).HasDefaultValueSql("now()")
            .ValueGeneratedOnAdd()
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
        builder.Property(lp => lp.Modified).HasDefaultValueSql("now()")
            .ValueGeneratedOnAddOrUpdate()
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
    }
}
