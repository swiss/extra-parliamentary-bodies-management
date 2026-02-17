using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingOccupations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"

            INSERT INTO data.occupations(""id"", ""text_de"", ""text_fr"", ""text_it"", ""text_rm"", ""text_female_de"", ""text_female_fr"", ""text_female_it"", ""text_female_rm"", ""created"", ""created_by"", ""modified"", ""modified_by"", ""description_de"", ""description_fr"", ""description_it"", ""description_rm"", ""sort"", ""uri"", ""old_id"", ""is_deleted"")
                VALUES
                ('3f7e8c5e-5c5e-4c57-9d2f-4f2b1a6f0a91', 'Fachreferent', 'Rapporteur spécialisé', 'Relatore specializzato', '', 'Fachreferentin', 'Rapporteuse spécialisée', 'Relatrice specializzata', '', now(), 'migration', now(), 'migration', '', '', '', '', 0, 'https://register.ld.admin.ch/i14y/ch-isco-19/47101032', 0, false),
                ('a9c1d7e2-8a43-4c0c-b0c2-6b7a9c5f2e3d', 'Geschäftsleitungsmitglied', 'Membre de la direction commerciale', 'Membro di direzione d''azienda', '', 'Geschäftsleitungsmitglied', 'Membre de la direction commerciale', 'Membro di direzione d''azienda', '', now(), 'migration', now(), 'migration', '', '', '', '', 0, 'https://register.ld.admin.ch/i14y/ch-isco-19/33101158', 0, false)
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
