using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class ElectionOfficeConfiguration : MasterDataBaseConfiguration<ElectionOffice>
{
    protected override void ConfigureMasterData(EntityTypeBuilder<ElectionOffice> builder)
    {
    }
}
