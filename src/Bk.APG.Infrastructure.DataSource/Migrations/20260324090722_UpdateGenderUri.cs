using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    public partial class UpdateGenderUri : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(schema: "data", table: "genders", keyColumn: "id", keyValue: "d45a5c83-ddf8-4e78-9fd9-0cf2a8914d15", column: "uri", value: "https://register.ld.admin.ch/i14y/concept/sex/1");
            migrationBuilder.UpdateData(schema: "data", table: "genders", keyColumn: "id", keyValue: "aa36da2a-b1d5-4b1e-a659-3f488dbc4d1e", column: "uri", value: "https://register.ld.admin.ch/i14y/concept/sex/2");
            migrationBuilder.UpdateData(schema: "data", table: "genders", keyColumn: "id", keyValue: "cec2b585-fd3f-4ff9-b45d-2b50780fead9", column: "uri", value: "https://register.ld.admin.ch/i14y/concept/sex/3");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(schema: "data", table: "genders", keyColumn: "id", keyValue: "d45a5c83-ddf8-4e78-9fd9-0cf2a8914d15", column: "uri", value: "https://register.ld.admin.ch/i14y/concept/DV_SEXE/1");
            migrationBuilder.UpdateData(schema: "data", table: "genders", keyColumn: "id", keyValue: "aa36da2a-b1d5-4b1e-a659-3f488dbc4d1e", column: "uri", value: "https://register.ld.admin.ch/i14y/concept/DV_SEXE/2");
            migrationBuilder.UpdateData(schema: "data", table: "genders", keyColumn: "id", keyValue: "cec2b585-fd3f-4ff9-b45d-2b50780fead9", column: "uri", value: "https://register.ld.admin.ch/i14y/concept/DV_SEXE/3");
        }
    }
}
