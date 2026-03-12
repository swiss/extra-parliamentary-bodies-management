using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class AddCandidateStateReadyForFederalCouncilProposalForwarded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE data.candidate_list_states
                SET
                    text_de = 'Validiert',
                    text_fr = 'FR_Validiert',
                    text_it = 'IT_Validiert',
                    text_rm = 'RM_Validiert',
                    uri = 'www.todo.uri.membershipliststate/VALIDATED',
                    modified = now(),
                    modified_by = 'migration'
                WHERE id = '7ec5c44a-b6ae-45bc-8d91-5b8d2db7c897'
            ");

            migrationBuilder.Sql(@"
                 INSERT INTO data.candidate_list_states(id, created_by, description_de, description_fr, description_it, description_rm, modified_by, old_id, sort, text_de, text_fr, text_it, text_rm, uri )
                    VALUES('192e3c42-e67d-4d6d-8051-e5c2a1cbbccd', 'migration', '', '', '', '', 'migration', 0, 4, 'Für BRA weitergeleitet', 'FR_Für BRA weitergeleitet', 'IT_Für BRA weitergeleitet', 'RM_Für BRA weitergeleitet', 'www.todo.uri.membershipliststate/READYPROPOSALFEDCOUNCILFORWARDED')
            ");

        }

        /// <inheritdoc />S
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
