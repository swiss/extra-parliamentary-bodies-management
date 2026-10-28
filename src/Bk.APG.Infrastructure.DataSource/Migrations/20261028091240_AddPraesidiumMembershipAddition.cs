using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class AddPraesidiumMembershipAddition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            // ogd_id is not left to the sequence: AddAdditionalInterestFunctionsMembershipAdditions inserted an explicit
            // ogd_id (45) without advancing it, so nextval() could return an existing value and violate the unique index
            // ix_membership_additions_ogd_id. Use MAX + 1 instead and reseed the sequence afterwards so later inserts are safe.
            migrationBuilder.Sql("""
                INSERT INTO data.membership_additions
                    ("id", "created", "created_by", "modified", "modified_by", "is_deleted",
                     "text_de", "text_fr", "text_it", "text_rm",
                     "description_de", "description_fr", "description_it", "description_rm",
                     "sort", "uri", "old_id", "ogd_id")
                SELECT
                    '5c3b8f2e-6a41-4d7e-9b2a-8e1f4c6d7a93', now(), 'migration', now(), 'migration', false,
                    'Präsidium', 'Présidence', 'Presidenza', '',
                    '', '', '', '', 0,
                    'www.todo.uri.5c3b8f2e-6a41-4d7e-9b2a-8e1f4c6d7a93', 0,
                    COALESCE(MAX("ogd_id"), 0) + 1
                FROM data.membership_additions;

                SELECT setval(
                    pg_get_serial_sequence('"data"."membership_additions"', 'ogd_id'),
                    COALESCE((SELECT MAX("ogd_id") FROM "data"."membership_additions"), 0) + 1,
                    false
                );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.Sql("""
                DELETE FROM data.membership_additions WHERE id = '5c3b8f2e-6a41-4d7e-9b2a-8e1f4c6d7a93';
                """);
        }
    }
}
