using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class AddValidationWorklistTaskTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
#pragma warning disable CA1814
            migrationBuilder.InsertData(
                schema: "data",
                table: "worklist_task_types",
                columns: new[] { "id", "can_be_created_manually", "created_by", "description_de", "description_fr", "description_it", "description_rm", "modified_by", "old_id", "sort", "text_de", "text_fr", "text_it", "text_rm", "uri" },
                values: new object[,]
                {
                    { WorklistTaskType.GeneralElectionPersonInterests, false, "migration", "", "", "", "", "migration", 0, 8, "GEW Finalisieren: Interessenbindungen vollständig erfassen", "FR_GEW Finalisieren: Interessenbindungen vollständig erfassen", "IT_GEW Finalisieren: Interessenbindungen vollständig erfassen", "RM_GEW Finalisieren: Interessenbindungen vollständig erfassen", "https://intranet.apg.admin.ch/vocabulary/task-state-type/9" },
                    { WorklistTaskType.GeneralElectionPersonBaseData, false, "migration", "", "", "", "", "migration", 0, 9, "GEW Finalisieren: Personendaten vollständig erfassen", "FR_GEW Finalisieren: Personendaten vollständig erfassen", "IT_GEW Finalisieren: Personendaten vollständig erfassen", "RM_GEW Finalisieren: Personendaten vollständig erfassen", "https://intranet.apg.admin.ch/vocabulary/task-state-type/10" },
                    { WorklistTaskType.GeneralElectionMembershipValidation, false, "migration", "", "", "", "", "migration", 0, 10, "GEW Finalisieren: Mitgliedschaft vollständig erfassen", "FR_GEW Finalisieren: Mitgliedschaft vollständig erfassen", "IT_GEW Finalisieren: Mitgliedschaft vollständig erfassen", "RM_GEW Finalisieren: Mitgliedschaft vollständig erfassen", "https://intranet.apg.admin.ch/vocabulary/task-state-type/11" },
                    { WorklistTaskType.GeneralElectionMissingSecretariat, false, "migration", "", "", "", "", "migration", 0, 11, "GEW Finalisieren: Sekretariat fehlt", "FR_GEW Finalisieren: Sekretariat fehlt", "IT_GEW Finalisieren: Sekretariat fehlt", "RM_GEW Finalisieren: Sekretariat fehlt", "https://intranet.apg.admin.ch/vocabulary/task-state-type/12" },
                    { WorklistTaskType.GeneralElectionMissingDataProtectionOfficer, false, "migration", "", "", "", "", "migration", 0, 12, "GEW Validierung: Datenschutzbeauftragter fehlt", "FR_GEW Validierung: Datenschutzbeauftragter fehlt", "IT_GEW Validierung: Datenschutzbeauftragter fehlt", "RM_GEW Validierung: Datenschutzbeauftragter fehlt", "https://intranet.apg.admin.ch/vocabulary/task-state-type/13" }
                });
#pragma warning restore CA1814
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "data",
                table: "worklist_task_types",
                keyColumn: "id",
                keyValues: [WorklistTaskType.GeneralElectionPersonInterests, WorklistTaskType.GeneralElectionPersonBaseData, WorklistTaskType.GeneralElectionMembershipValidation, WorklistTaskType.GeneralElectionMissingSecretariat, WorklistTaskType.GeneralElectionMissingDataProtectionOfficer]);
        }
    }
}
