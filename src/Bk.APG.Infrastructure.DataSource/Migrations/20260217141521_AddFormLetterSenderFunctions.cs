using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class AddFormLetterSenderFunctions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.CreateTable(
                name: "form_letter_sender_functions",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ogd_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    text_de = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    text_fr = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    text_it = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    text_rm = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    description_de = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    description_fr = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    description_it = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    description_rm = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    sort = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    uri = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    old_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_form_letter_sender_functions", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_form_letter_sender_functions_ogd_id",
                schema: "data",
                table: "form_letter_sender_functions",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_form_letter_sender_functions_uri",
                schema: "data",
                table: "form_letter_sender_functions",
                column: "uri",
                unique: true);

            migrationBuilder.Sql(@$"
                INSERT INTO data.form_letter_sender_functions(""id"", ""created"", ""created_by"", ""modified"", ""modified_by"", ""is_deleted"", ""text_de"", ""text_fr"", ""text_it"", ""text_rm"", ""description_de"", ""description_fr"", ""description_it"", ""description_rm"", ""sort"", ""uri"", ""old_id"")
                    VALUES
                        ('22B1A6D4-87E5-409C-A164-7F893828FE27', now(), 'migration', now(), 'migration', false, 'Bundespräsidentin', 'FR_Bundespräsidentin', 'IT_Bundespräsidentin', 'RM_Bundespräsidentin', '', '', '', '', 1, 'www.todo.uri.22B1A6D4-87E5-409C-A164-7F893828FE27', 0),
                        ('EF37862A-F266-4F96-BEAB-E14F660177E3', now(), 'migration', now(), 'migration', false, 'Bundespräsident', 'FR_Bundespräsident', 'IT_Bundespräsident', 'RM_Bundespräsident', '', '', '', '', 2, 'www.todo.uri.EF37862A-F266-4F96-BEAB-E14F660177E3', 0),
                        ('A192A945-6614-4D92-810F-5DD7B7487974', now(), 'migration', now(), 'migration', false, 'Bundesrätin', 'FR_Bundesrätin.', 'IT_Bundesrätin.', 'RM_Bundesrätin', '', '', '', '', 3, 'www.todo.uri.A192A945-6614-4D92-810F-5DD7B7487974', 0),
                        ('EAED2882-F174-4EA6-A117-2D2A7F9A2863', now(), 'migration', now(), 'migration', false, 'Bundesrat', 'FR_Bundesrat', 'IT_Bundesrat', 'RM_Bundesrat', '', '', '', '', 4, 'www.todo.uri.EAED2882-F174-4EA6-A117-2D2A7F9A2863', 0),
                        ('AEA80E6C-BB70-46D0-B49A-E691AF2A9385', now(), 'migration', now(), 'migration', false, 'Departementsvorsteherin', 'FR_Departementsvorsteherin', 'IT_Departementsvorsteherin', 'RM_Departementsvorsteherin', '', '', '', '', 5, 'www.todo.uri.AEA80E6C-BB70-46D0-B49A-E691AF2A9385', 0),
                        ('93572A16-1A60-4944-91A0-A0736B775BF5', now(), 'migration', now(), 'migration', false, 'Departementsvorsteher', 'FR_Departementsvorsteher', 'IT_Departementsvorsteher', 'RM_Departementsvorsteher', '', '', '', '', 6, 'www.todo.uri.93572A16-1A60-4944-91A0-A0736B775BF5', 0),
                        ('291A40CC-2169-4657-8C61-9F412213DAED', now(), 'migration', now(), 'migration', false, 'Generalsekretärin', 'FR_Generalsekretärin', 'IT_Generalsekretärin', 'RM_Generalsekretärin', '', '', '', '', 7, 'www.todo.uri.291A40CC-2169-4657-8C61-9F412213DAED', 0),
                        ('D095D8B4-62BD-43AD-960E-88301C394B35', now(), 'migration', now(), 'migration', false, 'Generalsekretär', 'FR_Generalsekretär', 'IT_Generalsekretär', 'RM_Generalsekretär', '', '', '', '', 8, 'www.todo.uri.D095D8B4-62BD-43AD-960E-88301C394B35', 0),
                        ('E71607CE-E2FD-4268-8047-7807CCFD564B', now(), 'migration', now(), 'migration', false, 'Stv. Generalsekretärin', 'FR_Stv. Generalsekretärin', 'IT_Stv. Generalsekretärin', 'RM_Stv. Generalsekretärin', '', '', '', '', 9, 'www.todo.uri.E71607CE-E2FD-4268-8047-7807CCFD564B', 0),
                        ('6C24BFDA-B5C4-4E16-AE2D-E1264058BC6C', now(), 'migration', now(), 'migration', false, 'Stv. Generalsekretär', 'FR_Stv. Generalsekretär', 'IT_Stv. Generalsekretär', 'RM_Stv. Generalsekretär', '', '', '', '', 10, 'www.todo.uri.6C24BFDA-B5C4-4E16-AE2D-E1264058BC6C', 0),
                        ('5AAAE74B-F3F5-44F5-8C9B-B74D20D84D22', now(), 'migration', now(), 'migration', false, 'Direktorin', 'FR_Direktorin', 'IT_Direktorin', 'RM_Direktorin', '', '', '', '', 11, 'www.todo.uri.5AAAE74B-F3F5-44F5-8C9B-B74D20D84D22', 0),
                        ('BA6F3EBA-5B77-4F07-AC19-59F6F47427E2', now(), 'migration', now(), 'migration', false, 'Direktor', 'FR_Direktor', 'IT_Direktor', 'RM_Direktor', '', '', '', '', 12, 'www.todo.uri.BA6F3EBA-5B77-4F07-AC19-59F6F47427E2', 0),
                        ('A7D442E2-12C9-4945-B9F5-5C0D0482EEE9', now(), 'migration', now(), 'migration', false, 'Stv. Direktorin', 'FR_Stv. Direktorin', 'IT_Stv. Direktorin', 'RM_Stv. Direktorin', '', '', '', '', 13, 'www.todo.uri.A7D442E2-12C9-4945-B9F5-5C0D0482EEE9', 0),
                        ('4913137E-0E9B-414D-AD8F-3D55E8C36164', now(), 'migration', now(), 'migration', false, 'Stv. Direktor', 'FR_Stv. Direktor', 'IT_Stv. Direktor', 'RM_Stv. Direktor', '', '', '', '', 14, 'www.todo.uri.4913137E-0E9B-414D-AD8F-3D55E8C36164', 0),
                        ('9D70EEB6-57B8-4F94-9713-D7EE588D5FAB', now(), 'migration', now(), 'migration', false, 'Vizedirektorin', 'FR_Vizedirektorin', 'IT_Vizedirektorin', 'RM_Vizedirektorin', '', '', '', '', 15, 'www.todo.uri.9D70EEB6-57B8-4F94-9713-D7EE588D5FAB', 0),
                        ('D6352FE6-6D54-422B-9CD5-2ECFFBCDB8B0', now(), 'migration', now(), 'migration', false, 'Vizedirektor', 'FR_Vizedirektor', 'IT_Vizedirektor', 'RM_Vizedirektor', '', '', '', '', 16, 'www.todo.uri.D6352FE6-6D54-422B-9CD5-2ECFFBCDB8B0', 0)
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.DropTable(
                name: "form_letter_sender_functions",
                schema: "data");
        }
    }
}
