using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class TranslateFormLeterSenderFunctions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.Sql(@$"
                update data.form_letter_sender_functions set text_fr = 'Présidente de la Confédération', text_it = 'Presidente della Confederazione', text_rm = 'Presidenta da la Confederaziun' where text_de = 'Bundespräsidentin';
                update data.form_letter_sender_functions set text_fr = 'Président de la Confédération', text_it = 'Presidente della Confederazione', text_rm = 'President da la Confederaziun' where text_de = 'Bundespräsident';
                update data.form_letter_sender_functions set text_fr = 'Conseillère fédérale', text_it = 'Consigliera federale', text_rm = 'Cussegliera federala' where text_de = 'Bundesrätin';
                update data.form_letter_sender_functions set text_fr = 'Conseiller fédéral', text_it = 'Consigliere federale', text_rm = 'Cusseglier federal' where text_de = 'Bundesrat';
                update data.form_letter_sender_functions set text_fr = 'Cheffe de département', text_it = 'Capo di dipartimento', text_rm = 'Scheffa dal dapartament' where text_de = 'Departementsvorsteherin';
                update data.form_letter_sender_functions set text_fr = 'Chef de département', text_it = 'Capo di dipartimento', text_rm = 'Schef dal departament' where text_de = 'Departementsvorsteher';
                update data.form_letter_sender_functions set text_fr = 'Secrétaire générale', text_it = 'Segretaria generale', text_rm = 'Secretaria generala' where text_de = 'Generalsekretärin';
                update data.form_letter_sender_functions set text_fr = 'Secrétaire général', text_it = 'Segretario generale', text_rm = 'Secretari general' where text_de = 'Generalsekretär';
                update data.form_letter_sender_functions set text_fr = 'Secrétaire générale suppléante', text_it = 'Segretaria generale supplente', text_rm = 'Secretaria generala substituta' where text_de = 'Stv. Generalsekretärin';
                update data.form_letter_sender_functions set text_fr = 'Secrétaire général suppléant', text_it = 'Segretario generale supplente', text_rm = 'Secretari general substitut' where text_de = 'Stv. Generalsekretär';
                update data.form_letter_sender_functions set text_fr = 'Directrice', text_it = 'Direttrice', text_rm = 'Directura' where text_de = 'Direktorin';
                update data.form_letter_sender_functions set text_fr = 'Directeur', text_it = 'Direttore', text_rm = 'Directur' where text_de = 'Direktor';
                update data.form_letter_sender_functions set text_fr = 'Directrice suppléante', text_it = 'Direttrice supplente', text_rm = 'Directura substituta' where text_de = 'Stv. Direktorin';
                update data.form_letter_sender_functions set text_fr = 'Directeur suppléant', text_it = 'Direttore supplente', text_rm = 'Directur substitut' where text_de = 'Stv. Direktor';
                update data.form_letter_sender_functions set text_fr = 'Sous-directrice', text_it = 'Vicedirettrice', text_rm = 'Vicedirectura' where text_de = 'Vizedirektorin';
                update data.form_letter_sender_functions set text_fr = 'Sous-directeur', text_it = 'Vicedirettore', text_rm = 'Vicedirectur' where text_de = 'Vizedirektor';
             ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
