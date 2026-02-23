using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingOccupations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var assembly = typeof(AddMissingOccupations).Assembly;

            using var stream = assembly.GetManifestResourceStream(
                "Bk.APG.Infrastructure.DataSource.Migrations.Scripts.MissingOccupations.sql");

            using var reader = new StreamReader(stream);
            var sql = reader.ReadToEnd();

            migrationBuilder.Sql(sql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
