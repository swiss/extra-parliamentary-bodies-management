using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class AddFormLetterSenders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "form_letter_senders",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    surname = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    given_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    sender_function_id = table.Column<Guid>(type: "uuid", nullable: false),
                    department_id = table.Column<Guid>(type: "uuid", nullable: false),
                    office_id = table.Column<Guid>(type: "uuid", nullable: false),
                    street_german = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    street_french = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    street_italian = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    street_romansh = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    zip = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    city_german = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    city_french = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    city_italian = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    city_romansh = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    website = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    signature_file_reference_id = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_form_letter_senders", x => x.id);
                    table.ForeignKey(
                        name: "fk_form_letter_senders_departments_department_id",
                        column: x => x.department_id,
                        principalSchema: "data",
                        principalTable: "departments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_form_letter_senders_document_storages_signature_file_refere",
                        column: x => x.signature_file_reference_id,
                        principalSchema: "data",
                        principalTable: "document_storages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_form_letter_senders_form_letter_sender_functions_sender_fun",
                        column: x => x.sender_function_id,
                        principalSchema: "data",
                        principalTable: "form_letter_sender_functions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_form_letter_senders_offices_office_id",
                        column: x => x.office_id,
                        principalSchema: "data",
                        principalTable: "offices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_form_letter_senders_department_id",
                schema: "data",
                table: "form_letter_senders",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_form_letter_senders_office_id",
                schema: "data",
                table: "form_letter_senders",
                column: "office_id");

            migrationBuilder.CreateIndex(
                name: "ix_form_letter_senders_sender_function_id",
                schema: "data",
                table: "form_letter_senders",
                column: "sender_function_id");

            migrationBuilder.CreateIndex(
                name: "ix_form_letter_senders_signature_file_reference_id",
                schema: "data",
                table: "form_letter_senders",
                column: "signature_file_reference_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "form_letter_senders",
                schema: "data");
        }
    }
}
