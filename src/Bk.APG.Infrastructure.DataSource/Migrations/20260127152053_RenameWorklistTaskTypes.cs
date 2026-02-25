using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class RenameWorklistTaskTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"
                UPDATE data.worklist_task_types SET text_de = 'GEW Start' WHERE id = '{WorklistTaskType.GeneralElectionStart}';
                UPDATE data.worklist_task_types SET text_de = 'GEW Start weiterleiten' WHERE id = '{WorklistTaskType.GeneralElectionDispatch}';
                UPDATE data.worklist_task_types SET text_de = 'GEW Bereit für Bundesratsantrag' WHERE id = '{WorklistTaskType.ReadyForFederalCouncilProposal}';
                UPDATE data.worklist_task_types SET text_de = 'GEW Begründungspflicht', text_fr = 'RI justification obligatoire', text_it = 'ERI obbligo di fornire una motivazione', text_rm = 'RM_GEW Begründungspflicht' WHERE id = '{WorklistTaskType.GeneralElectionMissingJustifications}';
                UPDATE data.worklist_task_types SET text_de = 'GEW Interessenbindungen ergänzen', text_fr = 'FR_GEW Interessenbindungen ergänzen', text_it = 'IT_GEW Interessenbindungen ergänzen', text_rm = 'RM_GEW Interessenbindungen ergänzen' WHERE id = '{WorklistTaskType.GeneralElectionPersonInterests}';
                UPDATE data.worklist_task_types SET text_de = 'GEW Personendaten ergänzen', text_fr = 'FR_GEW Personendaten ergänzen', text_it = 'IT_GEW Personendaten ergänzen', text_rm = 'RM_GEW Personendaten ergänzen' WHERE id = '{WorklistTaskType.GeneralElectionPersonBaseData}';
                UPDATE data.worklist_task_types SET text_de = 'GEW Mitgliedschaft ergänzen', text_fr = 'FR_GEW Mitgliedschaft ergänzen', text_it = 'IT_GEW Mitgliedschaft ergänzen', text_rm = 'RM_GEW Mitgliedschaft ergänzen' WHERE id = '{WorklistTaskType.GeneralElectionMembershipValidation}';
                UPDATE data.worklist_task_types SET text_de = 'GEW Sekretariat fehlt', text_fr = 'FR_GEW Sekretariat fehlt', text_it = 'IT_GEW Sekretariat fehlt', text_rm = 'RM_GEW Sekretariat fehlt' WHERE id = '{WorklistTaskType.GeneralElectionMissingSecretariat}';
                UPDATE data.worklist_task_types SET text_de = 'GEW Datenschutzberater/-in fehlt', text_fr = 'FR_GEW Datenschutzberater/-in fehlt', text_it = 'IT_GEW Datenschutzberater/-in fehlt', text_rm = 'RM_GEW Datenschutzberater/-in fehlt' WHERE id = '{WorklistTaskType.GeneralElectionMissingDataProtectionOfficer}';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
