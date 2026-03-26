using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class CleanupMembershipAdditionSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // STEP 1: Ensure unique OGD IDs
            migrationBuilder.UpdateData(schema: "data", table: "membership_additions", keyColumn: "id", keyValue: "81da54ba-0dee-477d-8a9a-0859789c5d75", column: "ogd_id", value: 22);
            migrationBuilder.UpdateData(schema: "data", table: "membership_additions", keyColumn: "id", keyValue: "9dcbf649-5237-4060-94ad-fea728e9cd6c", column: "ogd_id", value: 23);
            migrationBuilder.UpdateData(schema: "data", table: "membership_additions", keyColumn: "id", keyValue: "ac40cb0b-8712-4a4a-864b-035e5eef680f", column: "ogd_id", value: 24);
            migrationBuilder.UpdateData(schema: "data", table: "membership_additions", keyColumn: "id", keyValue: "32ffc467-4e21-459e-aceb-0eb64dc71f40", column: "ogd_id", value: 25);
            migrationBuilder.UpdateData(schema: "data", table: "membership_additions", keyColumn: "id", keyValue: "99f0afb7-e34b-4afb-b253-4aac94dfb8eb", column: "ogd_id", value: 26);
            migrationBuilder.UpdateData(schema: "data", table: "membership_additions", keyColumn: "id", keyValue: "7c2c1ecb-7ffd-47eb-b817-82ded59df633", column: "ogd_id", value: 27);
            migrationBuilder.UpdateData(schema: "data", table: "membership_additions", keyColumn: "id", keyValue: "eb519b3c-5331-4648-935d-66deb3a8e1d2", column: "ogd_id", value: 28);
            migrationBuilder.UpdateData(schema: "data", table: "membership_additions", keyColumn: "id", keyValue: "80ef61f4-2eca-438a-9f30-eb82d71e4431", column: "ogd_id", value: 29);
            migrationBuilder.UpdateData(schema: "data", table: "membership_additions", keyColumn: "id", keyValue: "40e66991-0f00-432a-a32f-4f9038adda7b", column: "ogd_id", value: 30);
            migrationBuilder.UpdateData(schema: "data", table: "membership_additions", keyColumn: "id", keyValue: "1c414165-bdb0-4d01-9f8f-244e5271594e", column: "ogd_id", value: 31);

            // STEP 2: Apply schema changes based on entity configuration
            migrationBuilder.AlterColumn<string>(
                name: "uri",
                schema: "data",
                table: "membership_additions",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "text_rm",
                schema: "data",
                table: "membership_additions",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "text_it",
                schema: "data",
                table: "membership_additions",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "text_fr",
                schema: "data",
                table: "membership_additions",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "text_de",
                schema: "data",
                table: "membership_additions",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "sort",
                schema: "data",
                table: "membership_additions",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ogd_id",
                schema: "data",
                table: "membership_additions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "modified_by",
                schema: "data",
                table: "membership_additions",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "is_deleted",
                schema: "data",
                table: "membership_additions",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "description_rm",
                schema: "data",
                table: "membership_additions",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "description_it",
                schema: "data",
                table: "membership_additions",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "description_fr",
                schema: "data",
                table: "membership_additions",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "description_de",
                schema: "data",
                table: "membership_additions",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "created_by",
                schema: "data",
                table: "membership_additions",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "ix_membership_additions_ogd_id",
                schema: "data",
                table: "membership_additions",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_membership_additions_uri",
                schema: "data",
                table: "membership_additions",
                column: "uri",
                unique: true);

            // STEP 3: Reseed ogd_id sequence
            migrationBuilder.Sql(@"
                SELECT setval(
                    pg_get_serial_sequence('""data"".""membership_additions""', 'ogd_id'),
                    COALESCE((SELECT MAX(""ogd_id"") FROM ""data"".""membership_additions""), 0) + 1,
                    false
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_membership_additions_ogd_id",
                schema: "data",
                table: "membership_additions");

            migrationBuilder.DropIndex(
                name: "ix_membership_additions_uri",
                schema: "data",
                table: "membership_additions");

            migrationBuilder.AlterColumn<string>(
                name: "uri",
                schema: "data",
                table: "membership_additions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "text_rm",
                schema: "data",
                table: "membership_additions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "text_it",
                schema: "data",
                table: "membership_additions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "text_fr",
                schema: "data",
                table: "membership_additions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "text_de",
                schema: "data",
                table: "membership_additions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<int>(
                name: "sort",
                schema: "data",
                table: "membership_additions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ogd_id",
                schema: "data",
                table: "membership_additions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<string>(
                name: "modified_by",
                schema: "data",
                table: "membership_additions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<bool>(
                name: "is_deleted",
                schema: "data",
                table: "membership_additions",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "description_rm",
                schema: "data",
                table: "membership_additions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "description_it",
                schema: "data",
                table: "membership_additions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "description_fr",
                schema: "data",
                table: "membership_additions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "description_de",
                schema: "data",
                table: "membership_additions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "created_by",
                schema: "data",
                table: "membership_additions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);
        }
    }
}
