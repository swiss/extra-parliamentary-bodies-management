using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class FixAdminExternalId : Migration
    {
        private const string TemporaryExternalId = "APG_TMP_SWAP";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.UpdateData(
                schema: "data",
                table: "eiam_assignments",
                keyColumn: "id",
                keyValue: EiamAssignment.AdminId,
                column: "external_id",
                value: TemporaryExternalId);

            migrationBuilder.UpdateData(
                schema: "data",
                table: "eiam_assignments",
                keyColumn: "id",
                keyValue: EiamAssignment.ApgId,
                column: "external_id",
                value: "APG");

            migrationBuilder.UpdateData(
                schema: "data",
                table: "eiam_assignments",
                keyColumn: "id",
                keyValue: EiamAssignment.AdminId,
                column: "external_id",
                value: "Admin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
