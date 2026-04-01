using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bk.APG.Infrastructure.DataSource.Configurations;

public class OccupationConfiguration : MasterDataBaseConfiguration<Occupation>
{
    protected override void ConfigureMasterData(EntityTypeBuilder<Occupation> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Property(e => e.TextFemaleDe).IsRequired().HasMaxLength(250);
        builder.Property(e => e.TextFemaleFr).IsRequired().HasMaxLength(250);
        builder.Property(e => e.TextFemaleIt).IsRequired().HasMaxLength(250);
        builder.Property(e => e.TextFemaleRm).HasMaxLength(250);

        builder
            .HasMany(o => o.Persons)
            .WithMany(p => p.Occupations)
            .UsingEntity<PersonOccupation>(j => j
                .HasOne(po => po.Person)
                .WithMany()
                .HasForeignKey(po => po.PersonsId)
                .HasConstraintName("fk_person_occupations_persons_persons_id"),
                j => j
                .HasOne(po => po.Occupation)
                .WithMany()
                .HasForeignKey(po => po.OccupationsId)
                .HasConstraintName("fk_person_occupations_occupations_occupations_id"),
                j =>
                {
                    j.ToTable("person_occupations");
                    j.HasKey(po => new { po.OccupationsId, po.PersonsId }).HasName("pk_person_occupations");
                    j.HasIndex(po => po.PersonsId).HasDatabaseName("ix_person_occupations_persons_id");
                });

        builder.HasIndex(o => o.TextDe)
            .IsUnique(false);
        builder.HasIndex(o => o.TextFr)
            .IsUnique(false);
        builder.HasIndex(o => o.TextIt)
            .IsUnique(false);
        builder.HasIndex(o => o.TextFemaleDe)
            .IsUnique(false);
        builder.HasIndex(o => o.TextFemaleFr)
            .IsUnique(false);
        builder.HasIndex(o => o.TextFemaleIt)
            .IsUnique(false);
    }
}
