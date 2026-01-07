using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class InterestConfiguration : EntityBaseConfiguration<Interest>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Interest> builder)
    {
        builder.Property(c => c.OgdId)
            .ValueGeneratedOnAdd();

        builder.Property(i => i.Text)
            .HasMaxLength(500);

        builder.Property(i => i.InterestText)
            .HasMaxLength(500);

        builder.Property(i => i.UidOrganisationId)
            .HasMaxLength(20);

        builder
            .HasOne(i => i.Person)
            .WithMany(i => i.Interests)
            .HasForeignKey(i => i.PersonId);

        builder.Property(i => i.RowVersion).IsRowVersion();
    }
}
