using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    public partial class UpdateFunctionText : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.UpdateData(schema: "data", table: "functions", keyColumn: "id", keyValue: "c2e8d46d-d827-412e-997b-d8afadaf41a7", column: "text_fr", value: "Membre");
            migrationBuilder.UpdateData(schema: "data", table: "functions", keyColumn: "id", keyValue: "c2e8d46d-d827-412e-997b-d8afadaf41a7", column: "text_it", value: "Membro");

            migrationBuilder.UpdateData(schema: "data", table: "functions", keyColumn: "id", keyValue: "43b6ea02-0933-4e6e-83cb-62bf70405fb9", column: "text_fr", value: "Membre suppléant");
            migrationBuilder.UpdateData(schema: "data", table: "functions", keyColumn: "id", keyValue: "43b6ea02-0933-4e6e-83cb-62bf70405fb9", column: "text_it", value: "Membro supplente");

            migrationBuilder.UpdateData(schema: "data", table: "functions", keyColumn: "id", keyValue: "a282a0cd-4a7d-48b6-9b52-9b216e9454fe", column: "text_female_fr", value: "Président");
            migrationBuilder.UpdateData(schema: "data", table: "functions", keyColumn: "id", keyValue: "a282a0cd-4a7d-48b6-9b52-9b216e9454fe", column: "text_female_it", value: "Presidente");
            migrationBuilder.UpdateData(schema: "data", table: "functions", keyColumn: "id", keyValue: "a282a0cd-4a7d-48b6-9b52-9b216e9454fe", column: "text_fr", value: "Président");
            migrationBuilder.UpdateData(schema: "data", table: "functions", keyColumn: "id", keyValue: "a282a0cd-4a7d-48b6-9b52-9b216e9454fe", column: "text_it", value: "Presidente");

            migrationBuilder.UpdateData(schema: "data", table: "functions", keyColumn: "id", keyValue: "17f63cd3-f254-4e6e-bd84-37311b38041c", column: "text_female_fr", value: "Vice-président");
            migrationBuilder.UpdateData(schema: "data", table: "functions", keyColumn: "id", keyValue: "17f63cd3-f254-4e6e-bd84-37311b38041c", column: "text_female_it", value: "Vicepresidente");
            migrationBuilder.UpdateData(schema: "data", table: "functions", keyColumn: "id", keyValue: "17f63cd3-f254-4e6e-bd84-37311b38041c", column: "text_fr", value: "Vice-président");
            migrationBuilder.UpdateData(schema: "data", table: "functions", keyColumn: "id", keyValue: "17f63cd3-f254-4e6e-bd84-37311b38041c", column: "text_it", value: "Vice-presidente");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
