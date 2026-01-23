using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class FixEiamAssignmentRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE data.eiam_assignments SET ""role"" = 'Admin' WHERE ""role"" = '0';");
            migrationBuilder.Sql(@"UPDATE data.eiam_assignments SET ""role"" = 'Department' WHERE ""role"" = '1';");
            migrationBuilder.Sql(@"UPDATE data.eiam_assignments SET ""role"" = 'Office' WHERE ""role"" = '2';");

            migrationBuilder.Sql(@"
                UPDATE data.offices o
                SET eiam_assignment_id = e.id
                FROM data.eiam_assignments e
                WHERE e.role = 'Office' AND e.office_id = o.id;
            ");

            migrationBuilder.Sql(@"UPDATE data.committees SET eiam_assignment_id = NULL;");
            migrationBuilder.Sql(@"DELETE FROM data.eiam_assignments WHERE ""role"" = 'Secretariat';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
