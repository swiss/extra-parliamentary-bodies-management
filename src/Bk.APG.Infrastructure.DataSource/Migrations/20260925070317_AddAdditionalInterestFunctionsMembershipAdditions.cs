using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class AddAdditionalInterestFunctionsMembershipAdditions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.Sql("""
                INSERT INTO data.interest_functions
                    ("id", "created", "created_by", "modified", "modified_by", "is_deleted",
                     "text_de", "text_fr", "text_it", "text_rm",
                     "description_de", "description_fr", "description_it", "description_rm",
                     "sort", "uri", "old_id")
                VALUES
                    ('c4f4f1c4-6d97-4b5d-8aa5-2e381d4a8b31', now(), 'migration', now(), 'migration', false,
                     'Revisor/Revisorin', 'Réviseur/Réviseuse', 'Revisore/Revisora', '',
                     '', '', '', '', 20,
                     'https://politics.ld.admin.ch/fch/apg/vocabulary/interest-function/20', 0),
                    ('6f9f74b4-b5cb-443b-a1d1-9fc8c34fbc52', now(), 'migration', now(), 'migration', false,
                     'Auditor/Auditorin', 'Auditeur/Auditrice', 'Uditore/Uditrice', '',
                     '', '', '', '', 21,
                     'https://politics.ld.admin.ch/fch/apg/vocabulary/interest-function/21', 0);
                """);

            migrationBuilder.Sql("""
                INSERT INTO data.membership_additions
                    ("id", "created", "created_by", "modified", "modified_by", "is_deleted",
                     "text_de", "text_fr", "text_it", "text_rm",
                     "description_de", "description_fr", "description_it", "description_rm",
                     "sort", "uri", "old_id", "ogd_id")
                VALUES
                    ('f1e3e2fb-7a43-4f2d-9a9c-6b688b3b1e65', now(), 'migration', now(), 'migration', false,
                     'Vertretung des Bundes', 'Représentation de la Confédération', 'Rappresentanza della Confederazione', '',
                     '', '', '', '', 0,
                     'www.todo.uri.f1e3e2fb-7a43-4f2d-9a9c-6b688b3b1e65', 0, 45);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);
        }
    }
}
