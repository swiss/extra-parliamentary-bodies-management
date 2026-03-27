using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class IntroduceGEPublicationDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                schema: "data",
                table: "worklist_tasks",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateOnly>(
                name: "planned_publication_date",
                schema: "data",
                table: "term_of_office_dates",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "publication_date",
                schema: "data",
                table: "term_of_office_dates",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.DropColumn(
                name: "is_deleted",
                schema: "data",
                table: "worklist_tasks");

            migrationBuilder.DropColumn(
                name: "planned_publication_date",
                schema: "data",
                table: "term_of_office_dates");

            migrationBuilder.DropColumn(
                name: "publication_date",
                schema: "data",
                table: "term_of_office_dates");
        }
    }
}
