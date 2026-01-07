using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class InterestFunctionConfiguration : MasterDataBaseConfiguration<InterestFunction>
{
    protected override void ConfigureMasterData(EntityTypeBuilder<InterestFunction> builder)
    {
    }
}
