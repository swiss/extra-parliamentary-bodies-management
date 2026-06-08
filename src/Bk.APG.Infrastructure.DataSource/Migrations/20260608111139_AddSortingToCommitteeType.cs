using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class AddSortingToCommitteeType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.UpdateData(schema: "data", table: "committee_types", keyColumn: "id", keyValue: "f2e2af70-d1d4-42b5-b23a-793cbc220064", column: "sort", value: 1);
            migrationBuilder.UpdateData(schema: "data", table: "committee_types", keyColumn: "id", keyValue: "0a4b7f1d-d8bf-4932-bece-dd2a51cc2d59", column: "sort", value: 2);
            migrationBuilder.UpdateData(schema: "data", table: "committee_types", keyColumn: "id", keyValue: "0959d68e-9c09-4ab3-9434-a5a4689c0305", column: "sort", value: 3);
            migrationBuilder.UpdateData(schema: "data", table: "committee_types", keyColumn: "id", keyValue: "408865cf-2b92-4c19-ac66-ee8f4cea76e5", column: "sort", value: 4);
            migrationBuilder.UpdateData(schema: "data", table: "committee_types", keyColumn: "id", keyValue: "469134d7-fb82-42ac-8feb-c2df6b7d032b", column: "sort", value: 5);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
