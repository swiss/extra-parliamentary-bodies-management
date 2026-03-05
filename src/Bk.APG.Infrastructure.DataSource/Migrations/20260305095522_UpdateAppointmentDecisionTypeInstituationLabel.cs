using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAppointmentDecisionTypeInstituationLabel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE data.appointment_decision_types
                SET
                    text_de = 'Einsetzungsverfügung',
                    text_fr = 'L’acte d’institution',
                    text_it = 'Decisione istitutiva',
                    text_rm = 'RM_Einsetzungsverfügung',
                    description_de = 'Einsetzungsverfügung',
                    description_fr = 'L’acte d’institution',
                    description_it = 'Decisione istitutiva',
                    description_rm = 'RM_Einsetzungsverfügung',
                    modified = now(),
                    modified_by = 'migration'
                WHERE id = '7a68f837-ea2c-42b9-b9c7-6506fa3e9c67';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE data.appointment_decision_types
                SET
                    text_de = 'Einsetzung',
                    text_fr = 'institution',
                    text_it = 'istituzione',
                    text_rm = 'RM_Einsetzung',
                    description_de = 'Einsetzung',
                    description_fr = 'institution',
                    description_it = 'istituzione',
                    description_rm = 'RM_Einsetzung',
                    modified = now(),
                    modified_by = 'migration'
                WHERE id = '7a68f837-ea2c-42b9-b9c7-6506fa3e9c67';
            ");
        }
    }
}
