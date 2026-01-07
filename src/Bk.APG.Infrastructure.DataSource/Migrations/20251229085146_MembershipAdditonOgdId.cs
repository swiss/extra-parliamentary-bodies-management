using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class MembershipAdditonOgdId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ogd_id",
                schema: "data",
                table: "appointment_decisions",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "ix_appointment_decisions_ogd_id",
                schema: "data",
                table: "appointment_decisions",
                column: "ogd_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_appointment_decisions_ogd_id",
                schema: "data",
                table: "appointment_decisions");

            migrationBuilder.DropColumn(
                name: "ogd_id",
                schema: "data",
                table: "appointment_decisions");
        }
    }
}
