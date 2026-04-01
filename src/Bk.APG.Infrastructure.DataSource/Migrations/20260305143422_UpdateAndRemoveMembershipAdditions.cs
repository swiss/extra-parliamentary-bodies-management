using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAndRemoveMembershipAdditions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.Sql(@"
                UPDATE data.membership_additions
                SET
                    is_deleted = true,
                    modified = now(),
                    modified_by = 'migration'
                WHERE id = '91240fa6-4480-4e87-b8cd-97380c571daf';
            ");

            migrationBuilder.Sql(@"
                UPDATE data.membership_additions
                SET
                    is_deleted = true,
                    modified = now(),
                    modified_by = 'migration'
                WHERE id = '485e464e-2159-4848-af9a-0bad11ec47e6';
            ");

            migrationBuilder.Sql(@"
                UPDATE data.membership_additions
                SET
                    text_de = 'Fachvertretung / Spezialist:innen',
                    modified = now(),
                    modified_by = 'migration'
                WHERE id = '934915b3-831a-4d66-a890-263839c4ae68';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.Sql(@"
                UPDATE data.membership_additions
                SET
                    is_deleted = false,
                    modified = now(),
                    modified_by = 'migration'
                WHERE id = '91240fa6-4480-4e87-b8cd-97380c571daf';
            ");

            migrationBuilder.Sql(@"
                UPDATE data.membership_additions
                SET
                    is_deleted = false,
                    modified = now(),
                    modified_by = 'migration'
                WHERE id = '485e464e-2159-4848-af9a-0bad11ec47e6';
            ");

            migrationBuilder.Sql(@"
                UPDATE data.membership_additions
                SET
                    text_de = 'Fachvertretung / Spezialistinnen und Spezialisten',
                    modified = now(),
                    modified_by = 'migration'
                WHERE id = '934915b3-831a-4d66-a890-263839c4ae68';
            ");
        }
    }
}
