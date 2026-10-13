using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class AddRomanshCommitteeFunctions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.Sql(@$"
                update data.functions set text_rm = 'Commember', text_female_rm = 'Commembra'  where uri = 'www.todo.uri.C2E8D46D-D827-412E-997B-D8AFADAF41A7';
                update data.functions set text_rm = 'Commember substitut', text_female_rm = 'Commembra subsituta'  where uri = 'www.todo.uri.43B6EA02-0933-4E6E-83CB-62BF70405FB9';
                update data.functions set text_rm = 'President', text_female_rm = 'Presidenta'  where uri = 'www.todo.uri.A282A0CD-4A7D-48B6-9B52-9B216E9454FE';
                update data.functions set text_rm = 'Vicepresident', text_female_rm = 'Vicepresidenta'  where uri = 'www.todo.uri.17F63CD3-F254-4E6E-BD84-37311B38041C';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
