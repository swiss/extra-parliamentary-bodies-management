using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;


public class TermOfOfficeConfiguration : MasterDataBaseConfiguration<TermOfOffice>
{
    protected override void ConfigureMasterData(EntityTypeBuilder<TermOfOffice> builder)
    {
    }
}
