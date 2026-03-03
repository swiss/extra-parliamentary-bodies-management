using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class CountryConfiguration : MasterDataBaseConfiguration<Country>
{
    protected override void ConfigureMasterData(EntityTypeBuilder<Country> builder)
    {
    }
}
