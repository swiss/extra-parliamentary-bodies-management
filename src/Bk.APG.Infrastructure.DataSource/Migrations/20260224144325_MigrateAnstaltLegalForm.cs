using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class MigrateAnstaltLegalForm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE data.legal_forms
                SET
                    text_de = 'Institut des öffentlichen Rechts',
                    text_fr = 'Institut de droit public',
                    text_it = 'Istituto di diritto pubblico',
                    text_rm = 'Institut da dretg public',
                    legal_form_id = '0117',
                    uri = 'https://ld.admin.ch/ech/97/legalforms/0117',
                    sort = 10,
                    modified = now(),
                    modified_by = 'migration'
                WHERE id = 'e816ec7e-bd5b-4037-a83b-a53fe636bf53';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE data.legal_forms
                SET
                    text_de = 'Anstalt',
                    text_fr = 'Etablissement',
                    text_it = 'Istituto',
                    text_rm = '',
                    legal_form_id = '0000',
                    uri = 'https://ld.admin.ch/ech/97/legalforms/0000',
                    sort = 99,
                    modified = now(),
                    modified_by = 'migration'
                WHERE id = 'e816ec7e-bd5b-4037-a83b-a53fe636bf53';
            ");
        }
    }
}
