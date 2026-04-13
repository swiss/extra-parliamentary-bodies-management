using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    public partial class OtherElectionOffice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            // Keine -> Andere
            migrationBuilder.UpdateData(schema: "data", table: "election_offices", keyColumn: "id", keyValue: "b4ec71be-3174-4624-83cc-110da132d699", column: "text_de", value: "Andere");
            migrationBuilder.UpdateData(schema: "data", table: "election_offices", keyColumn: "id", keyValue: "b4ec71be-3174-4624-83cc-110da132d699", column: "text_fr", value: "Autres");
            migrationBuilder.UpdateData(schema: "data", table: "election_offices", keyColumn: "id", keyValue: "b4ec71be-3174-4624-83cc-110da132d699", column: "text_it", value: "Altri");
            migrationBuilder.UpdateData(schema: "data", table: "election_offices", keyColumn: "id", keyValue: "b4ec71be-3174-4624-83cc-110da132d699", column: "text_rm", value: "Auters");
            migrationBuilder.UpdateData(schema: "data", table: "election_offices", keyColumn: "id", keyValue: "b4ec71be-3174-4624-83cc-110da132d699", column: "sort", value: 2);
            migrationBuilder.UpdateData(schema: "data", table: "election_offices", keyColumn: "id", keyValue: "b4ec71be-3174-4624-83cc-110da132d699", column: "uri", value: "https://politics.ld.admin.ch/fch/apg/vocabulary/election-office/2");
            migrationBuilder.UpdateData(schema: "data", table: "election_offices", keyColumn: "id", keyValue: "b4ec71be-3174-4624-83cc-110da132d699", column: "is_deleted", value: false);
            migrationBuilder.UpdateData(schema: "data", table: "election_offices", keyColumn: "id", keyValue: "b4ec71be-3174-4624-83cc-110da132d699", column: "modified", value: DateTime.UtcNow);
            migrationBuilder.UpdateData(schema: "data", table: "election_offices", keyColumn: "id", keyValue: "b4ec71be-3174-4624-83cc-110da132d699", column: "modified_by", value: "system");

            // Fix URI for Bundesrat
            migrationBuilder.UpdateData(schema: "data", table: "election_offices", keyColumn: "id", keyValue: "29333d9c-77da-4674-b496-ecc626552267", column: "uri", value: "https://register.ld.admin.ch/termdat/38634");
            migrationBuilder.UpdateData(schema: "data", table: "election_offices", keyColumn: "id", keyValue: "29333d9c-77da-4674-b496-ecc626552267", column: "modified", value: DateTime.UtcNow);
            migrationBuilder.UpdateData(schema: "data", table: "election_offices", keyColumn: "id", keyValue: "29333d9c-77da-4674-b496-ecc626552267", column: "modified_by", value: "system");

            // Update sort for Departement
            migrationBuilder.UpdateData(schema: "data", table: "election_offices", keyColumn: "id", keyValue: "9890fce5-4331-4e96-8894-04b51acaedfb", column: "sort", value: 1);
            migrationBuilder.UpdateData(schema: "data", table: "election_offices", keyColumn: "id", keyValue: "9890fce5-4331-4e96-8894-04b51acaedfb", column: "modified", value: DateTime.UtcNow);
            migrationBuilder.UpdateData(schema: "data", table: "election_offices", keyColumn: "id", keyValue: "9890fce5-4331-4e96-8894-04b51acaedfb", column: "modified_by", value: "system");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
