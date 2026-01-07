using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class InterestLegalFormConfiguration : MasterDataBaseConfiguration<InterestLegalForm>
{
    protected override void ConfigureMasterData(EntityTypeBuilder<InterestLegalForm> builder)
    {
    }
}
