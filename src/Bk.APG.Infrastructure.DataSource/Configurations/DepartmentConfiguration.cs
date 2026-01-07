using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class DepartmentConfiguration : MasterDataBaseConfiguration<Department>
{
    protected override void ConfigureMasterData(EntityTypeBuilder<Department> builder)
    {
    }
}
