using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class DocumentStorageConfiguration : EntityBaseConfiguration<DocumentStorage>
{
    protected override void ConfigureEntity(EntityTypeBuilder<DocumentStorage> builder)
    {
        builder.Property(a => a.DocumentName)
            .HasMaxLength(500);

        builder.Property(a => a.DocumentStorageId)
            .HasMaxLength(100);
    }
}
