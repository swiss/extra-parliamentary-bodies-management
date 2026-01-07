using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class AppointmentDecisionLinkTypeConfiguration : MasterDataBaseConfiguration<AppointmentDecisionLinkType>
{
    protected override void ConfigureMasterData(EntityTypeBuilder<AppointmentDecisionLinkType> builder)
    {
    }
}
