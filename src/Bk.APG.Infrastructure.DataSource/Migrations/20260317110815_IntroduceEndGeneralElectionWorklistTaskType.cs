using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class IntroduceEndGeneralElectionWorklistTaskType : Migration
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
                    { WorklistTaskType.GeneralElectionEnd, true, "migration", "", "", "", "", "migration", 0, 14, "GEW_Ende", "FR_GEW_Ende", "IT_GEW_Ende", "RM_GEW_Ende", "https://intranet.apg.admin.ch/vocabulary/task-state-type/16" }
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
                keyValues: [WorklistTaskType.GeneralElectionEnd]);
        }
    }
}
