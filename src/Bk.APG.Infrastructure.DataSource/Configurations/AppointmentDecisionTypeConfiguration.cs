using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class AppointmentDecisionTypeConfiguration : MasterDataBaseConfiguration<AppointmentDecisionType>
{
    protected override void ConfigureMasterData(EntityTypeBuilder<AppointmentDecisionType> builder)
    {
    }
}
