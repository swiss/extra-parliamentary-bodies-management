using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class InterestCommitteeConfiguration : MasterDataBaseConfiguration<InterestCommittee>
{
    protected override void ConfigureMasterData(EntityTypeBuilder<InterestCommittee> builder)
    {
    }
}
