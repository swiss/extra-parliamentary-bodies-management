using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class AddGeneralMeasureWorklistTaskTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

#pragma warning disable CA1814
            migrationBuilder.InsertData(
                schema: "data",
                table: "worklist_task_types",
                columns: new[] { "id", "can_be_created_manually", "created_by", "description_de", "description_fr", "description_it", "description_rm", "modified_by", "old_id", "sort", "text_de", "text_fr", "text_it", "text_rm", "uri" },
                values: new object[,]
                {
                    { WorklistTaskType.GeneralMeasureCheck, false, "migration", "", "", "", "", "migration", 0, 13, "Allgemeine Massnahme prüfen", "FR_Allgemeine Massnahme prüfen", "IT_Allgemeine Massnahme prüfen", "RM_Allgemeine Massnahme prüfen", "https://intranet.apg.admin.ch/vocabulary/task-state-type/14" },
                    { WorklistTaskType.GeneralMeasureValidate, false, "migration", "", "", "", "", "migration", 0, 14, "Allgemeine Massnahme validieren", "FR_Allgemeine Massnahme validieren", "IT_Allgemeine Massnahme validieren", "RM_Allgemeine Massnahme validieren", "https://intranet.apg.admin.ch/vocabulary/task-state-type/15" }
                });
#pragma warning restore CA1814
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.DeleteData(
                schema: "data",
                table: "worklist_task_types",
                keyColumn: "id",
                keyValues: [WorklistTaskType.GeneralMeasureCheck, WorklistTaskType.GeneralMeasureValidate]);
        }
    }
}
