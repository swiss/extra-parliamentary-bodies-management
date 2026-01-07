using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;


public class SalutationConfiguration : MasterDataBaseConfiguration<Salutation>
{
    protected override void ConfigureMasterData(EntityTypeBuilder<Salutation> builder)
    {
        builder
            .HasOne(s => s.Gender)
            .WithMany()
            .HasForeignKey(s => s.GenderId);
    }
}
