using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class MembershipAdditionConfiguration : MasterDataBaseConfiguration<MembershipAddition>
{
    protected override void ConfigureMasterData(EntityTypeBuilder<MembershipAddition> builder)
    {

    }
}
