using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class ElectionTypeConfiguration : MasterDataBaseConfiguration<ElectionType>
{
    protected override void ConfigureMasterData(EntityTypeBuilder<ElectionType> builder)
    {
    }
}
