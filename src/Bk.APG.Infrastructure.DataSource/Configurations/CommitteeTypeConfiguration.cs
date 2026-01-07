using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class CommitteeTypeConfiguration : MasterDataBaseConfiguration<CommitteeType>
{
    protected override void ConfigureMasterData(EntityTypeBuilder<CommitteeType> builder)
    {
        builder.Property(p => p.RowVersion).IsRowVersion();
    }
}
