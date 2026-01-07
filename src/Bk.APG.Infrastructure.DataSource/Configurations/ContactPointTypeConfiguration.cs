using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class ContactPointTypeConfiguration : MasterDataBaseConfiguration<ContactPointType>
{
    protected override void ConfigureMasterData(EntityTypeBuilder<ContactPointType> builder)
    {
    }
}
