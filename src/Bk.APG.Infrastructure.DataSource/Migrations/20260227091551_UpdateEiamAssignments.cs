using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEiamAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.Sql($@"UPDATE data.committees
                                    SET office_id = (SELECT id FROM data.offices
	                                    WHERE text_de='BAKOM')
                                    WHERE committee_number = '10763'");

            migrationBuilder.Sql($@"INSERT INTO data.eiam_assignments(id, external_id, role, parent_id, department_id, office_id, committee_id)
	                                    VALUES ('e96fa8ef-c403-4e18-bf46-3cc2a67fbae6', 30, 'Office', (SELECT id FROM data.eiam_assignments WHERE department_id = (SELECT id FROM data.departments WHERE text_de = 'UVEK')), null, (SELECT id FROM data.offices WHERE text_de = 'BAKOM'), null)");

            migrationBuilder.Sql($@"INSERT INTO data.eiam_assignments(id, external_id, role, parent_id, department_id, office_id, committee_id)
	                                    VALUES ('6a006227-896c-4823-8f1d-b410c22674f8', 31, 'Office', (SELECT id FROM data.eiam_assignments WHERE department_id = (SELECT id FROM data.departments WHERE text_de = 'UVEK')), null, (SELECT id FROM data.offices WHERE text_de = 'BAV'), null)");

            migrationBuilder.Sql($@"INSERT INTO data.eiam_assignments(id, external_id, role, parent_id, department_id, office_id, committee_id)
	                                    VALUES ('4c5ccee8-2375-4740-aa29-846d1c3e29e4', 32, 'Office', (SELECT id FROM data.eiam_assignments WHERE department_id = (SELECT id FROM data.departments WHERE text_de = 'UVEK')), null, (SELECT id FROM data.offices WHERE text_de = 'ASTRA'), null)");

            migrationBuilder.Sql($@"UPDATE data.eiam_assignments
                                        SET parent_id = '6a006227-896c-4823-8f1d-b410c22674f8'
                                    WHERE external_id = '10691'");

            migrationBuilder.Sql($@"UPDATE data.eiam_assignments
                                        SET parent_id = '4c5ccee8-2375-4740-aa29-846d1c3e29e4'
                                    WHERE external_id = '10726';");

            migrationBuilder.Sql($@"UPDATE data.eiam_assignments
                                        SET parent_id = 'e96fa8ef-c403-4e18-bf46-3cc2a67fbae6'
                                    WHERE external_id = '10763'");

            migrationBuilder.Sql($@"UPDATE data.eiam_assignments
                                        SET parent_id = (SELECT id FROM data.eiam_assignments WHERE office_id = (SELECT id FROM data.offices WHERE text_de = 'BAG'))
                                    WHERE external_id = '10780'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
