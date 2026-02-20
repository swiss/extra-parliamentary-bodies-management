using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class FormLetterSenderFunctionConfiguration : MasterDataBaseConfiguration<FormLetterSenderFunction>
{
    protected override void ConfigureMasterData(EntityTypeBuilder<FormLetterSenderFunction> builder)
    {
    }
}
