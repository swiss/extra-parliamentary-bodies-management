using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class IntroduceSelfOrganizedCommittees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.AddColumn<bool>(
                name: "self_organized",
                schema: "data",
                table: "general_election_committees",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "self_organized",
                schema: "data",
                table: "committees",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.DropColumn(
                name: "self_organized",
                schema: "data",
                table: "general_election_committees");

            migrationBuilder.DropColumn(
                name: "self_organized",
                schema: "data",
                table: "committees");
        }
    }
}
