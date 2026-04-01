using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOgdIdFromGeneralMeasure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.DropIndex(
                name: "ix_general_language_measures_ogd_id",
                schema: "data",
                table: "general_language_measures");

            migrationBuilder.DropIndex(
                name: "ix_general_gender_measures_ogd_id",
                schema: "data",
                table: "general_gender_measures");

            migrationBuilder.DropColumn(
                name: "ogd_id",
                schema: "data",
                table: "general_language_measures");

            migrationBuilder.DropColumn(
                name: "ogd_id",
                schema: "data",
                table: "general_gender_measures");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.AddColumn<int>(
                name: "ogd_id",
                schema: "data",
                table: "general_language_measures",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "ogd_id",
                schema: "data",
                table: "general_gender_measures",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "ix_general_language_measures_ogd_id",
                schema: "data",
                table: "general_language_measures",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_general_gender_measures_ogd_id",
                schema: "data",
                table: "general_gender_measures",
                column: "ogd_id",
                unique: true);
        }
    }
}
