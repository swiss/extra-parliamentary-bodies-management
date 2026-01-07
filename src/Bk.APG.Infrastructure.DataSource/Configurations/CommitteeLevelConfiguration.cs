using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class CommitteeLevelConfiguration : MasterDataBaseConfiguration<CommitteeLevel>
{
    protected override void ConfigureMasterData(EntityTypeBuilder<CommitteeLevel> builder)
    {
    }
}
