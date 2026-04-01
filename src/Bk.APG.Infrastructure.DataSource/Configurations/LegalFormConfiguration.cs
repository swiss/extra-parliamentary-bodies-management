using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class LegalFormConfiguration : MasterDataBaseConfiguration<LegalForm>
{
    protected override void ConfigureMasterData(EntityTypeBuilder<LegalForm> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Property(l => l.LegalFormId)
            .IsRequired()
            .HasMaxLength(10);
    }
}
