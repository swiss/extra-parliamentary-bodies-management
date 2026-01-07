using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class CantonConfiguration : MasterDataBaseConfiguration<Canton>
{
    protected override void ConfigureMasterData(EntityTypeBuilder<Canton> builder)
    {
    }
}
