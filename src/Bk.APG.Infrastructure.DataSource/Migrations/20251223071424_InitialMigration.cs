using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "data");

            migrationBuilder.CreateTable(
                name: "apg_general_settings",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_ogd_export_activated = table.Column<bool>(type: "boolean", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_apg_general_settings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "appointment_decision_link_types",
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
                    table.PrimaryKey("pk_appointment_decision_link_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "appointment_decision_types",
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
                    table.PrimaryKey("pk_appointment_decision_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "candidate_list_states",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    created_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
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
                    table.PrimaryKey("pk_candidate_list_states", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cantons",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    region = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("pk_cantons", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "committee_levels",
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
                    table.PrimaryKey("pk_committee_levels", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "committee_types",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    female_threshold = table.Column<double>(type: "double precision", nullable: true),
                    male_threshold = table.Column<double>(type: "double precision", nullable: true),
                    german_minimal_threshold = table.Column<int>(type: "integer", nullable: true),
                    french_minimal_threshold = table.Column<int>(type: "integer", nullable: true),
                    italian_minimal_threshold = table.Column<int>(type: "integer", nullable: true),
                    romansh_minimal_threshold = table.Column<int>(type: "integer", nullable: true),
                    german_threshold_percentage = table.Column<double>(type: "double precision", nullable: true),
                    french_threshold_percentage = table.Column<double>(type: "double precision", nullable: true),
                    italian_threshold_percentage = table.Column<double>(type: "double precision", nullable: true),
                    romansh_threshold_percentage = table.Column<double>(type: "double precision", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
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
                    table.PrimaryKey("pk_committee_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "contact_point_types",
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
                    table.PrimaryKey("pk_contact_point_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "councils",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    created_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
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
                    table.PrimaryKey("pk_councils", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "departments",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    eiam_assignment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_big_department = table.Column<bool>(type: "boolean", nullable: false),
                    general_gender_measure_id = table.Column<Guid>(type: "uuid", nullable: true),
                    general_language_measure_id = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("pk_departments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "document_storages",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    document_storage_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_document_storages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "election_offices",
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
                    table.PrimaryKey("pk_election_offices", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "election_types",
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
                    table.PrimaryKey("pk_election_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "entity_audit_log",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    audit_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    audit_action = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    audit_user = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    audit_data = table.Column<string>(type: "jsonb", nullable: true),
                    entity_primary_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    entity_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    entity_snapshot = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_entity_audit_log", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "functions",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    text_female_de = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    text_female_fr = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    text_female_it = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    text_female_rm = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
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
                    table.PrimaryKey("pk_functions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "genders",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<string>(type: "text", nullable: false),
                    ogd_id = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    text_de = table.Column<string>(type: "text", nullable: false),
                    text_fr = table.Column<string>(type: "text", nullable: false),
                    text_it = table.Column<string>(type: "text", nullable: false),
                    text_rm = table.Column<string>(type: "text", nullable: false),
                    description_de = table.Column<string>(type: "text", nullable: false),
                    description_fr = table.Column<string>(type: "text", nullable: false),
                    description_it = table.Column<string>(type: "text", nullable: false),
                    description_rm = table.Column<string>(type: "text", nullable: false),
                    sort = table.Column<int>(type: "integer", nullable: false),
                    uri = table.Column<string>(type: "text", nullable: false),
                    old_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_genders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "interest_committees",
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
                    table.PrimaryKey("pk_interest_committees", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "interest_functions",
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
                    table.PrimaryKey("pk_interest_functions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "interest_legal_forms",
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
                    table.PrimaryKey("pk_interest_legal_forms", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "languages",
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
                    table.PrimaryKey("pk_languages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "legal_forms",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    legal_form_id = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
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
                    table.PrimaryKey("pk_legal_forms", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "legislature_periods",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    election_date = table.Column<DateOnly>(type: "date", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    created_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
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
                    table.PrimaryKey("pk_legislature_periods", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "membership_additions",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<string>(type: "text", nullable: false),
                    ogd_id = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    text_de = table.Column<string>(type: "text", nullable: false),
                    text_fr = table.Column<string>(type: "text", nullable: false),
                    text_it = table.Column<string>(type: "text", nullable: false),
                    text_rm = table.Column<string>(type: "text", nullable: false),
                    description_de = table.Column<string>(type: "text", nullable: false),
                    description_fr = table.Column<string>(type: "text", nullable: false),
                    description_it = table.Column<string>(type: "text", nullable: false),
                    description_rm = table.Column<string>(type: "text", nullable: false),
                    sort = table.Column<int>(type: "integer", nullable: false),
                    uri = table.Column<string>(type: "text", nullable: false),
                    old_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_membership_additions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "occupations",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    text_female_de = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    text_female_fr = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    text_female_it = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    text_female_rm = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
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
                    table.PrimaryKey("pk_occupations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "term_of_office_dates",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    begin_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    is_general_election = table.Column<bool>(type: "boolean", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    created_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
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
                    table.PrimaryKey("pk_term_of_office_dates", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "terms_of_office",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    duration_in_years = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("pk_terms_of_office", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "worklist_task_states",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    created_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
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
                    table.PrimaryKey("pk_worklist_task_states", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "worklist_task_types",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    can_be_created_manually = table.Column<bool>(type: "boolean", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    created_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
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
                    table.PrimaryKey("pk_worklist_task_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "addresses",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    street = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    po_box = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    country_code = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    zip = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    canton_id = table.Column<Guid>(type: "uuid", nullable: true),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    mobile = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    old_id = table.Column<int>(type: "integer", nullable: false),
                    verified_successfully = table.Column<bool>(type: "boolean", nullable: false),
                    verification_code = table.Column<int>(type: "integer", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_addresses", x => x.id);
                    table.ForeignKey(
                        name: "fk_addresses_cantons_canton_id",
                        column: x => x.canton_id,
                        principalSchema: "data",
                        principalTable: "cantons",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "general_gender_measures",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ogd_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false),
                    department_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_general_gender_measures", x => x.id);
                    table.ForeignKey(
                        name: "fk_general_gender_measures_departments_department_id",
                        column: x => x.department_id,
                        principalSchema: "data",
                        principalTable: "departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "general_language_measures",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ogd_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false),
                    department_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_general_language_measures", x => x.id);
                    table.ForeignKey(
                        name: "fk_general_language_measures_departments_department_id",
                        column: x => x.department_id,
                        principalSchema: "data",
                        principalTable: "departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "offices",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    eiam_assignment_id = table.Column<Guid>(type: "uuid", nullable: true),
                    department_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_central_federal_administration = table.Column<bool>(type: "boolean", nullable: false),
                    is_general_secretariat = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("pk_offices", x => x.id);
                    table.ForeignKey(
                        name: "fk_offices_departments_department_id",
                        column: x => x.department_id,
                        principalSchema: "data",
                        principalTable: "departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "salutations",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    gender_id = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("pk_salutations", x => x.id);
                    table.ForeignKey(
                        name: "fk_salutations_genders_gender_id",
                        column: x => x.gender_id,
                        principalSchema: "data",
                        principalTable: "genders",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "committees",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ogd_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    committee_number = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    eiam_assignment_id = table.Column<Guid>(type: "uuid", nullable: true),
                    begin_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    description_german = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    description_french = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    description_italian = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    description_romansh = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    department_id = table.Column<Guid>(type: "uuid", nullable: false),
                    office_id = table.Column<Guid>(type: "uuid", nullable: false),
                    committee_level_id = table.Column<Guid>(type: "uuid", nullable: false),
                    committee_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    legal_form_id = table.Column<Guid>(type: "uuid", nullable: true),
                    old_legal_form = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    legal_base = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    term_of_office_id = table.Column<Guid>(type: "uuid", nullable: false),
                    term_of_office_date_id = table.Column<Guid>(type: "uuid", nullable: false),
                    minimal_members = table.Column<int>(type: "integer", nullable: true),
                    maximal_members = table.Column<int>(type: "integer", nullable: true),
                    vacancies_general_election = table.Column<int>(type: "integer", nullable: true),
                    link_authority_website = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    remarks_base_data = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    remarks_base_data_admin = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    justification_members = table.Column<string>(type: "text", nullable: true),
                    justification_genders = table.Column<string>(type: "text", nullable: true),
                    measures_genders = table.Column<string>(type: "text", nullable: true),
                    justification_languages = table.Column<string>(type: "text", nullable: true),
                    measures_languages = table.Column<string>(type: "text", nullable: true),
                    release_general_election = table.Column<bool>(type: "boolean", nullable: true),
                    federal_law_establishment = table.Column<bool>(type: "boolean", nullable: true),
                    market_orientated = table.Column<bool>(type: "boolean", nullable: true),
                    supervision_duty = table.Column<bool>(type: "boolean", nullable: true),
                    additional_authority_members = table.Column<bool>(type: "boolean", nullable: false),
                    federal_institution = table.Column<bool>(type: "boolean", nullable: true),
                    link_homepage_german = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    link_homepage_french = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    link_homepage_italian = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    link_homepage_romansh = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_committees", x => x.id);
                    table.ForeignKey(
                        name: "fk_committees_committee_levels_committee_level_id",
                        column: x => x.committee_level_id,
                        principalSchema: "data",
                        principalTable: "committee_levels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_committees_committee_types_committee_type_id",
                        column: x => x.committee_type_id,
                        principalSchema: "data",
                        principalTable: "committee_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_committees_departments_department_id",
                        column: x => x.department_id,
                        principalSchema: "data",
                        principalTable: "departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_committees_legal_forms_legal_form_id",
                        column: x => x.legal_form_id,
                        principalSchema: "data",
                        principalTable: "legal_forms",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_committees_offices_office_id",
                        column: x => x.office_id,
                        principalSchema: "data",
                        principalTable: "offices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_committees_term_of_office_dates_term_of_office_date_id",
                        column: x => x.term_of_office_date_id,
                        principalSchema: "data",
                        principalTable: "term_of_office_dates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_committees_terms_of_office_term_of_office_id",
                        column: x => x.term_of_office_id,
                        principalSchema: "data",
                        principalTable: "terms_of_office",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "persons",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ogd_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    surname = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    given_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    birth_year = table.Column<int>(type: "integer", nullable: false),
                    occupation = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    federal_duty = table.Column<bool>(type: "boolean", nullable: false),
                    federal_assembly = table.Column<bool>(type: "boolean", nullable: false),
                    title = table.Column<string>(type: "text", nullable: true),
                    salutation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    salutation_text = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    language_id = table.Column<Guid>(type: "uuid", nullable: false),
                    correspondence_language_id = table.Column<Guid>(type: "uuid", nullable: false),
                    gender_id = table.Column<Guid>(type: "uuid", nullable: false),
                    office_address_id = table.Column<Guid>(type: "uuid", nullable: true),
                    private_address_id = table.Column<Guid>(type: "uuid", nullable: true),
                    correspondence_address_id = table.Column<Guid>(type: "uuid", nullable: true),
                    council_id = table.Column<Guid>(type: "uuid", nullable: true),
                    office_id = table.Column<Guid>(type: "uuid", nullable: true),
                    old_id = table.Column<int>(type: "integer", nullable: false),
                    remarks_person_data = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    remarks_person_data_admin = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    no_interest = table.Column<bool>(type: "boolean", nullable: false),
                    no_employment = table.Column<bool>(type: "boolean", nullable: false),
                    employer = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_persons", x => x.id);
                    table.ForeignKey(
                        name: "fk_persons_addresses_correspondence_address_id",
                        column: x => x.correspondence_address_id,
                        principalSchema: "data",
                        principalTable: "addresses",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_persons_addresses_office_address_id",
                        column: x => x.office_address_id,
                        principalSchema: "data",
                        principalTable: "addresses",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_persons_addresses_private_address_id",
                        column: x => x.private_address_id,
                        principalSchema: "data",
                        principalTable: "addresses",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_persons_councils_council_id",
                        column: x => x.council_id,
                        principalSchema: "data",
                        principalTable: "councils",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_persons_genders_gender_id",
                        column: x => x.gender_id,
                        principalSchema: "data",
                        principalTable: "genders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_persons_languages_correspondence_language_id",
                        column: x => x.correspondence_language_id,
                        principalSchema: "data",
                        principalTable: "languages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_persons_languages_language_id",
                        column: x => x.language_id,
                        principalSchema: "data",
                        principalTable: "languages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_persons_offices_office_id",
                        column: x => x.office_id,
                        principalSchema: "data",
                        principalTable: "offices",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_persons_salutations_salutation_id",
                        column: x => x.salutation_id,
                        principalSchema: "data",
                        principalTable: "salutations",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "appointment_decisions",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    committee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    appointment_decision_type_id = table.Column<Guid>(type: "uuid", nullable: true),
                    appointment_decision_link_type_id = table.Column<Guid>(type: "uuid", nullable: true),
                    appointment_decision_date = table.Column<DateOnly>(type: "date", nullable: false),
                    text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    link = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    original_document_id = table.Column<Guid>(type: "uuid", nullable: true),
                    file_reference_german_id = table.Column<Guid>(type: "uuid", nullable: true),
                    file_reference_french_id = table.Column<Guid>(type: "uuid", nullable: true),
                    file_reference_italian_id = table.Column<Guid>(type: "uuid", nullable: true),
                    file_reference_romansh_id = table.Column<Guid>(type: "uuid", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    old_id = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_appointment_decisions", x => x.id);
                    table.ForeignKey(
                        name: "fk_appointment_decisions_appointment_decision_link_types_appoi",
                        column: x => x.appointment_decision_link_type_id,
                        principalSchema: "data",
                        principalTable: "appointment_decision_link_types",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_appointment_decisions_appointment_decision_types_appointmen",
                        column: x => x.appointment_decision_type_id,
                        principalSchema: "data",
                        principalTable: "appointment_decision_types",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_appointment_decisions_committees_committee_id",
                        column: x => x.committee_id,
                        principalSchema: "data",
                        principalTable: "committees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_appointment_decisions_document_storages_file_reference_fren",
                        column: x => x.file_reference_french_id,
                        principalSchema: "data",
                        principalTable: "document_storages",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_appointment_decisions_document_storages_file_reference_germ",
                        column: x => x.file_reference_german_id,
                        principalSchema: "data",
                        principalTable: "document_storages",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_appointment_decisions_document_storages_file_reference_ital",
                        column: x => x.file_reference_italian_id,
                        principalSchema: "data",
                        principalTable: "document_storages",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_appointment_decisions_document_storages_file_reference_roma",
                        column: x => x.file_reference_romansh_id,
                        principalSchema: "data",
                        principalTable: "document_storages",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_appointment_decisions_document_storages_original_document_id",
                        column: x => x.original_document_id,
                        principalSchema: "data",
                        principalTable: "document_storages",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "committee_membership_addition",
                schema: "data",
                columns: table => new
                {
                    committees_id = table.Column<Guid>(type: "uuid", nullable: false),
                    membership_additions_in_general_election_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_committee_membership_addition", x => new { x.committees_id, x.membership_additions_in_general_election_id });
                    table.ForeignKey(
                        name: "fk_committee_membership_addition_committees_committees_id",
                        column: x => x.committees_id,
                        principalSchema: "data",
                        principalTable: "committees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_committee_membership_addition_membership_additions_membersh",
                        column: x => x.membership_additions_in_general_election_id,
                        principalSchema: "data",
                        principalTable: "membership_additions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "eiam_assignments",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_id = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    department_id = table.Column<Guid>(type: "uuid", nullable: true),
                    office_id = table.Column<Guid>(type: "uuid", nullable: true),
                    committee_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_eiam_assignments", x => x.id);
                    table.ForeignKey(
                        name: "fk_eiam_assignments_committees_committee_id",
                        column: x => x.committee_id,
                        principalSchema: "data",
                        principalTable: "committees",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_eiam_assignments_departments_department_id",
                        column: x => x.department_id,
                        principalSchema: "data",
                        principalTable: "departments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_eiam_assignments_eiam_assignments_parent_id",
                        column: x => x.parent_id,
                        principalSchema: "data",
                        principalTable: "eiam_assignments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_eiam_assignments_offices_office_id",
                        column: x => x.office_id,
                        principalSchema: "data",
                        principalTable: "offices",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "general_election_committees",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    begin_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    secretariat_ready_for_proposal_due_date = table.Column<DateOnly>(type: "date", nullable: true),
                    office_ready_for_proposal_due_date = table.Column<DateOnly>(type: "date", nullable: true),
                    description_german = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    description_french = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    description_italian = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    description_romansh = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    committee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    department_id = table.Column<Guid>(type: "uuid", nullable: false),
                    office_id = table.Column<Guid>(type: "uuid", nullable: false),
                    committee_level_id = table.Column<Guid>(type: "uuid", nullable: false),
                    committee_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    legal_form_id = table.Column<Guid>(type: "uuid", nullable: true),
                    old_legal_form = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    legal_base = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    term_of_office_id = table.Column<Guid>(type: "uuid", nullable: false),
                    term_of_office_date_id = table.Column<Guid>(type: "uuid", nullable: false),
                    minimal_members = table.Column<int>(type: "integer", nullable: true),
                    maximal_members = table.Column<int>(type: "integer", nullable: true),
                    vacancies_general_election = table.Column<int>(type: "integer", nullable: true),
                    link_authority_website = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    remarks_base_data = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    remarks_base_data_admin = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    justification_members = table.Column<string>(type: "text", nullable: true),
                    justification_genders = table.Column<string>(type: "text", nullable: true),
                    measures_genders = table.Column<string>(type: "text", nullable: true),
                    justification_languages = table.Column<string>(type: "text", nullable: true),
                    measures_languages = table.Column<string>(type: "text", nullable: true),
                    release_general_election = table.Column<bool>(type: "boolean", nullable: true),
                    federal_law_establishment = table.Column<bool>(type: "boolean", nullable: true),
                    market_orientated = table.Column<bool>(type: "boolean", nullable: true),
                    supervision_duty = table.Column<bool>(type: "boolean", nullable: true),
                    additional_authority_members = table.Column<bool>(type: "boolean", nullable: false),
                    federal_institution = table.Column<bool>(type: "boolean", nullable: true),
                    link_homepage_german = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    link_homepage_french = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    link_homepage_italian = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    link_homepage_romansh = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_validated = table.Column<bool>(type: "boolean", nullable: false),
                    selection_procedure = table.Column<string>(type: "text", nullable: true),
                    candidate_list_state_id = table.Column<Guid>(type: "uuid", nullable: true),
                    assigned_to_role = table.Column<string>(type: "text", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_general_election_committees", x => x.id);
                    table.ForeignKey(
                        name: "fk_general_election_committees_candidate_list_states_candidate",
                        column: x => x.candidate_list_state_id,
                        principalSchema: "data",
                        principalTable: "candidate_list_states",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_general_election_committees_committee_levels_committee_leve",
                        column: x => x.committee_level_id,
                        principalSchema: "data",
                        principalTable: "committee_levels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_general_election_committees_committee_types_committee_type_",
                        column: x => x.committee_type_id,
                        principalSchema: "data",
                        principalTable: "committee_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_general_election_committees_committees_committee_id",
                        column: x => x.committee_id,
                        principalSchema: "data",
                        principalTable: "committees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_general_election_committees_departments_department_id",
                        column: x => x.department_id,
                        principalSchema: "data",
                        principalTable: "departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_general_election_committees_legal_forms_legal_form_id",
                        column: x => x.legal_form_id,
                        principalSchema: "data",
                        principalTable: "legal_forms",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_general_election_committees_offices_office_id",
                        column: x => x.office_id,
                        principalSchema: "data",
                        principalTable: "offices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_general_election_committees_term_of_office_dates_term_of_of",
                        column: x => x.term_of_office_date_id,
                        principalSchema: "data",
                        principalTable: "term_of_office_dates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_general_election_committees_terms_of_office_term_of_office_",
                        column: x => x.term_of_office_id,
                        principalSchema: "data",
                        principalTable: "terms_of_office",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "interests",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ogd_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    interest_text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    interest_committee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    interest_function_id = table.Column<Guid>(type: "uuid", nullable: false),
                    interest_legal_form_id = table.Column<Guid>(type: "uuid", nullable: true),
                    legal_form_id = table.Column<Guid>(type: "uuid", nullable: true),
                    begin_date = table.Column<DateOnly>(type: "date", nullable: true),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    uid_organisation_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    old_id = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    verified_successfully = table.Column<bool>(type: "boolean", nullable: true),
                    uid_organisation_name_closest_match = table.Column<string>(type: "text", nullable: true),
                    uid_match_quality = table.Column<int>(type: "integer", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_interests", x => x.id);
                    table.ForeignKey(
                        name: "fk_interests_interest_committees_interest_committee_id",
                        column: x => x.interest_committee_id,
                        principalSchema: "data",
                        principalTable: "interest_committees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_interests_interest_functions_interest_function_id",
                        column: x => x.interest_function_id,
                        principalSchema: "data",
                        principalTable: "interest_functions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_interests_interest_legal_forms_interest_legal_form_id",
                        column: x => x.interest_legal_form_id,
                        principalSchema: "data",
                        principalTable: "interest_legal_forms",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_interests_legal_forms_legal_form_id",
                        column: x => x.legal_form_id,
                        principalSchema: "data",
                        principalTable: "legal_forms",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_interests_persons_person_id",
                        column: x => x.person_id,
                        principalSchema: "data",
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "memberships",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ogd_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    committee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    maximum_employment_level = table.Column<int>(type: "integer", nullable: true),
                    begin_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    election_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    function_id = table.Column<Guid>(type: "uuid", nullable: false),
                    election_office_id = table.Column<Guid>(type: "uuid", nullable: false),
                    old_membership_addition = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    membership_addition_id = table.Column<Guid>(type: "uuid", nullable: true),
                    justification_longer_duty = table.Column<string>(type: "text", nullable: true),
                    justification_shorter_duty = table.Column<string>(type: "text", nullable: true),
                    justification_member_in_federal_duty = table.Column<string>(type: "text", nullable: true),
                    justification_member_in_federal_assembly = table.Column<string>(type: "text", nullable: true),
                    requirements_profile = table.Column<string>(type: "text", nullable: true),
                    old_id = table.Column<int>(type: "integer", nullable: false),
                    remarks = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    remarks_status = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    in_correlation_with_federal_duty = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_memberships", x => x.id);
                    table.ForeignKey(
                        name: "fk_memberships_committees_committee_id",
                        column: x => x.committee_id,
                        principalSchema: "data",
                        principalTable: "committees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_memberships_election_offices_election_office_id",
                        column: x => x.election_office_id,
                        principalSchema: "data",
                        principalTable: "election_offices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_memberships_election_types_election_type_id",
                        column: x => x.election_type_id,
                        principalSchema: "data",
                        principalTable: "election_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_memberships_functions_function_id",
                        column: x => x.function_id,
                        principalSchema: "data",
                        principalTable: "functions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_memberships_membership_additions_membership_addition_id",
                        column: x => x.membership_addition_id,
                        principalSchema: "data",
                        principalTable: "membership_additions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_memberships_persons_person_id",
                        column: x => x.person_id,
                        principalSchema: "data",
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "person_legislature_period",
                schema: "data",
                columns: table => new
                {
                    legislature_periods_id = table.Column<Guid>(type: "uuid", nullable: false),
                    persons_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_person_legislature_period", x => new { x.legislature_periods_id, x.persons_id });
                    table.ForeignKey(
                        name: "fk_person_legislature_period_legislature_periods_legislature_p",
                        column: x => x.legislature_periods_id,
                        principalSchema: "data",
                        principalTable: "legislature_periods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_person_legislature_period_persons_persons_id",
                        column: x => x.persons_id,
                        principalSchema: "data",
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "person_occupations",
                schema: "data",
                columns: table => new
                {
                    occupations_id = table.Column<Guid>(type: "uuid", nullable: false),
                    persons_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_person_occupations", x => new { x.occupations_id, x.persons_id });
                    table.ForeignKey(
                        name: "fk_person_occupations_occupations_occupations_id",
                        column: x => x.occupations_id,
                        principalSchema: "data",
                        principalTable: "occupations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_person_occupations_persons_persons_id",
                        column: x => x.persons_id,
                        principalSchema: "data",
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contact_points",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ogd_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    committee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    contact_point_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    section = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    begin_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    street = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    po_box = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    zip = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    surname = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    given_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    language_id = table.Column<Guid>(type: "uuid", nullable: true),
                    gender_id = table.Column<Guid>(type: "uuid", nullable: true),
                    personal_phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    personal_mobile = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    personal_email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    release_person_data = table.Column<bool>(type: "boolean", nullable: false),
                    verified_successfully = table.Column<bool>(type: "boolean", nullable: false),
                    verification_code = table.Column<int>(type: "integer", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    old_id = table.Column<int>(type: "integer", nullable: false),
                    general_election_committee_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contact_points", x => x.id);
                    table.ForeignKey(
                        name: "fk_contact_points_committees_committee_id",
                        column: x => x.committee_id,
                        principalSchema: "data",
                        principalTable: "committees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_contact_points_contact_point_types_contact_point_type_id",
                        column: x => x.contact_point_type_id,
                        principalSchema: "data",
                        principalTable: "contact_point_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_contact_points_genders_gender_id",
                        column: x => x.gender_id,
                        principalSchema: "data",
                        principalTable: "genders",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_contact_points_general_election_committees_general_election",
                        column: x => x.general_election_committee_id,
                        principalSchema: "data",
                        principalTable: "general_election_committees",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_contact_points_languages_language_id",
                        column: x => x.language_id,
                        principalSchema: "data",
                        principalTable: "languages",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "membership_candidate_log_messages",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    general_election_committee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    log_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_membership_candidate_log_messages", x => x.id);
                    table.ForeignKey(
                        name: "fk_membership_candidate_log_messages_general_election_committe",
                        column: x => x.general_election_committee_id,
                        principalSchema: "data",
                        principalTable: "general_election_committees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_membership_candidate_log_messages_persons_person_id",
                        column: x => x.person_id,
                        principalSchema: "data",
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "membership_candidates",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    surname = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    given_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    birth_year = table.Column<int>(type: "integer", nullable: false),
                    membership_id = table.Column<Guid>(type: "uuid", nullable: true),
                    person_id = table.Column<Guid>(type: "uuid", nullable: true),
                    language_id = table.Column<Guid>(type: "uuid", nullable: false),
                    gender_id = table.Column<Guid>(type: "uuid", nullable: false),
                    general_election_committee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    maximum_employment_level = table.Column<int>(type: "integer", nullable: true),
                    begin_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    election_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    function_id = table.Column<Guid>(type: "uuid", nullable: false),
                    election_office_id = table.Column<Guid>(type: "uuid", nullable: false),
                    old_membership_addition = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    membership_addition_id = table.Column<Guid>(type: "uuid", nullable: true),
                    justification_longer_duty = table.Column<string>(type: "text", nullable: true),
                    justification_shorter_duty = table.Column<string>(type: "text", nullable: true),
                    justification_member_in_federal_duty = table.Column<string>(type: "text", nullable: true),
                    justification_member_in_federal_assembly = table.Column<string>(type: "text", nullable: true),
                    requirements_profile = table.Column<string>(type: "text", nullable: true),
                    remarks = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    remarks_status = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    in_correlation_with_federal_duty = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_selected = table.Column<bool>(type: "boolean", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_membership_candidates", x => x.id);
                    table.ForeignKey(
                        name: "fk_membership_candidates_election_offices_election_office_id",
                        column: x => x.election_office_id,
                        principalSchema: "data",
                        principalTable: "election_offices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_membership_candidates_election_types_election_type_id",
                        column: x => x.election_type_id,
                        principalSchema: "data",
                        principalTable: "election_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_membership_candidates_functions_function_id",
                        column: x => x.function_id,
                        principalSchema: "data",
                        principalTable: "functions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_membership_candidates_genders_gender_id",
                        column: x => x.gender_id,
                        principalSchema: "data",
                        principalTable: "genders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_membership_candidates_general_election_committees_general_e",
                        column: x => x.general_election_committee_id,
                        principalSchema: "data",
                        principalTable: "general_election_committees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_membership_candidates_languages_language_id",
                        column: x => x.language_id,
                        principalSchema: "data",
                        principalTable: "languages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_membership_candidates_membership_additions_membership_addit",
                        column: x => x.membership_addition_id,
                        principalSchema: "data",
                        principalTable: "membership_additions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_membership_candidates_memberships_membership_id",
                        column: x => x.membership_id,
                        principalSchema: "data",
                        principalTable: "memberships",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_membership_candidates_persons_person_id",
                        column: x => x.person_id,
                        principalSchema: "data",
                        principalTable: "persons",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "worklist_tasks",
                schema: "data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_task_id = table.Column<Guid>(type: "uuid", nullable: true),
                    assigned_to_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assigned_by_id = table.Column<Guid>(type: "uuid", nullable: false),
                    due_date = table.Column<DateOnly>(type: "date", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    worklist_task_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    worklist_task_state_id = table.Column<Guid>(type: "uuid", nullable: false),
                    department_id = table.Column<Guid>(type: "uuid", nullable: true),
                    office_id = table.Column<Guid>(type: "uuid", nullable: true),
                    committee_id = table.Column<Guid>(type: "uuid", nullable: true),
                    membership_id = table.Column<Guid>(type: "uuid", nullable: true),
                    person_id = table.Column<Guid>(type: "uuid", nullable: true),
                    general_election_committee_id = table.Column<Guid>(type: "uuid", nullable: true),
                    membership_candidate_id = table.Column<Guid>(type: "uuid", nullable: true),
                    term_of_office_date_id = table.Column<Guid>(type: "uuid", nullable: true),
                    row_version = table.Column<long>(type: "bigint", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_worklist_tasks", x => x.id);
                    table.ForeignKey(
                        name: "fk_worklist_tasks_committees_committee_id",
                        column: x => x.committee_id,
                        principalSchema: "data",
                        principalTable: "committees",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_worklist_tasks_departments_department_id",
                        column: x => x.department_id,
                        principalSchema: "data",
                        principalTable: "departments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_worklist_tasks_eiam_assignments_assigned_by_id",
                        column: x => x.assigned_by_id,
                        principalSchema: "data",
                        principalTable: "eiam_assignments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_worklist_tasks_eiam_assignments_assigned_to_id",
                        column: x => x.assigned_to_id,
                        principalSchema: "data",
                        principalTable: "eiam_assignments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_worklist_tasks_general_election_committees_general_election",
                        column: x => x.general_election_committee_id,
                        principalSchema: "data",
                        principalTable: "general_election_committees",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_worklist_tasks_membership_candidates_membership_candidate_id",
                        column: x => x.membership_candidate_id,
                        principalSchema: "data",
                        principalTable: "membership_candidates",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_worklist_tasks_memberships_membership_id",
                        column: x => x.membership_id,
                        principalSchema: "data",
                        principalTable: "memberships",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_worklist_tasks_offices_office_id",
                        column: x => x.office_id,
                        principalSchema: "data",
                        principalTable: "offices",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_worklist_tasks_persons_person_id",
                        column: x => x.person_id,
                        principalSchema: "data",
                        principalTable: "persons",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_worklist_tasks_term_of_office_dates_term_of_office_date_id",
                        column: x => x.term_of_office_date_id,
                        principalSchema: "data",
                        principalTable: "term_of_office_dates",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_worklist_tasks_worklist_task_states_worklist_task_state_id",
                        column: x => x.worklist_task_state_id,
                        principalSchema: "data",
                        principalTable: "worklist_task_states",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_worklist_tasks_worklist_task_types_worklist_task_type_id",
                        column: x => x.worklist_task_type_id,
                        principalSchema: "data",
                        principalTable: "worklist_task_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_worklist_tasks_worklist_tasks_parent_task_id",
                        column: x => x.parent_task_id,
                        principalSchema: "data",
                        principalTable: "worklist_tasks",
                        principalColumn: "id");
                });



            migrationBuilder.CreateIndex(
                name: "ix_addresses_canton_id",
                schema: "data",
                table: "addresses",
                column: "canton_id");

            migrationBuilder.CreateIndex(
                name: "ix_appointment_decision_link_types_ogd_id",
                schema: "data",
                table: "appointment_decision_link_types",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_appointment_decision_link_types_uri",
                schema: "data",
                table: "appointment_decision_link_types",
                column: "uri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_appointment_decision_types_ogd_id",
                schema: "data",
                table: "appointment_decision_types",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_appointment_decision_types_uri",
                schema: "data",
                table: "appointment_decision_types",
                column: "uri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_appointment_decisions_appointment_decision_link_type_id",
                schema: "data",
                table: "appointment_decisions",
                column: "appointment_decision_link_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_appointment_decisions_appointment_decision_type_id",
                schema: "data",
                table: "appointment_decisions",
                column: "appointment_decision_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_appointment_decisions_committee_id",
                schema: "data",
                table: "appointment_decisions",
                column: "committee_id");

            migrationBuilder.CreateIndex(
                name: "ix_appointment_decisions_file_reference_french_id",
                schema: "data",
                table: "appointment_decisions",
                column: "file_reference_french_id");

            migrationBuilder.CreateIndex(
                name: "ix_appointment_decisions_file_reference_german_id",
                schema: "data",
                table: "appointment_decisions",
                column: "file_reference_german_id");

            migrationBuilder.CreateIndex(
                name: "ix_appointment_decisions_file_reference_italian_id",
                schema: "data",
                table: "appointment_decisions",
                column: "file_reference_italian_id");

            migrationBuilder.CreateIndex(
                name: "ix_appointment_decisions_file_reference_romansh_id",
                schema: "data",
                table: "appointment_decisions",
                column: "file_reference_romansh_id");

            migrationBuilder.CreateIndex(
                name: "ix_appointment_decisions_original_document_id",
                schema: "data",
                table: "appointment_decisions",
                column: "original_document_id");

            migrationBuilder.CreateIndex(
                name: "ix_candidate_list_states_ogd_id",
                schema: "data",
                table: "candidate_list_states",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_candidate_list_states_uri",
                schema: "data",
                table: "candidate_list_states",
                column: "uri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_cantons_ogd_id",
                schema: "data",
                table: "cantons",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_cantons_uri",
                schema: "data",
                table: "cantons",
                column: "uri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_committee_levels_ogd_id",
                schema: "data",
                table: "committee_levels",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_committee_levels_uri",
                schema: "data",
                table: "committee_levels",
                column: "uri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_committee_membership_addition_membership_additions_in_gener",
                schema: "data",
                table: "committee_membership_addition",
                column: "membership_additions_in_general_election_id");

            migrationBuilder.CreateIndex(
                name: "ix_committee_types_ogd_id",
                schema: "data",
                table: "committee_types",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_committee_types_uri",
                schema: "data",
                table: "committee_types",
                column: "uri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_committees_committee_level_id",
                schema: "data",
                table: "committees",
                column: "committee_level_id");

            migrationBuilder.CreateIndex(
                name: "ix_committees_committee_type_id",
                schema: "data",
                table: "committees",
                column: "committee_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_committees_department_id",
                schema: "data",
                table: "committees",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_committees_legal_form_id",
                schema: "data",
                table: "committees",
                column: "legal_form_id");

            migrationBuilder.CreateIndex(
                name: "ix_committees_office_id",
                schema: "data",
                table: "committees",
                column: "office_id");

            migrationBuilder.CreateIndex(
                name: "ix_committees_ogd_id",
                schema: "data",
                table: "committees",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_committees_term_of_office_date_id",
                schema: "data",
                table: "committees",
                column: "term_of_office_date_id");

            migrationBuilder.CreateIndex(
                name: "ix_committees_term_of_office_id",
                schema: "data",
                table: "committees",
                column: "term_of_office_id");

            migrationBuilder.CreateIndex(
                name: "ix_contact_point_types_ogd_id",
                schema: "data",
                table: "contact_point_types",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_contact_point_types_uri",
                schema: "data",
                table: "contact_point_types",
                column: "uri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_contact_points_committee_id",
                schema: "data",
                table: "contact_points",
                column: "committee_id");

            migrationBuilder.CreateIndex(
                name: "ix_contact_points_contact_point_type_id",
                schema: "data",
                table: "contact_points",
                column: "contact_point_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_contact_points_gender_id",
                schema: "data",
                table: "contact_points",
                column: "gender_id");

            migrationBuilder.CreateIndex(
                name: "ix_contact_points_general_election_committee_id",
                schema: "data",
                table: "contact_points",
                column: "general_election_committee_id");

            migrationBuilder.CreateIndex(
                name: "ix_contact_points_language_id",
                schema: "data",
                table: "contact_points",
                column: "language_id");

            migrationBuilder.CreateIndex(
                name: "ix_contact_points_ogd_id",
                schema: "data",
                table: "contact_points",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_councils_ogd_id",
                schema: "data",
                table: "councils",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_councils_uri",
                schema: "data",
                table: "councils",
                column: "uri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_departments_ogd_id",
                schema: "data",
                table: "departments",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_departments_uri",
                schema: "data",
                table: "departments",
                column: "uri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_eiam_assignments_committee_id",
                schema: "data",
                table: "eiam_assignments",
                column: "committee_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_eiam_assignments_department_id",
                schema: "data",
                table: "eiam_assignments",
                column: "department_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_eiam_assignments_external_id",
                schema: "data",
                table: "eiam_assignments",
                column: "external_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_eiam_assignments_office_id",
                schema: "data",
                table: "eiam_assignments",
                column: "office_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_eiam_assignments_parent_id",
                schema: "data",
                table: "eiam_assignments",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_election_offices_ogd_id",
                schema: "data",
                table: "election_offices",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_election_offices_uri",
                schema: "data",
                table: "election_offices",
                column: "uri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_election_types_ogd_id",
                schema: "data",
                table: "election_types",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_election_types_uri",
                schema: "data",
                table: "election_types",
                column: "uri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_functions_ogd_id",
                schema: "data",
                table: "functions",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_functions_uri",
                schema: "data",
                table: "functions",
                column: "uri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_general_election_committees_candidate_list_state_id",
                schema: "data",
                table: "general_election_committees",
                column: "candidate_list_state_id");

            migrationBuilder.CreateIndex(
                name: "ix_general_election_committees_committee_id",
                schema: "data",
                table: "general_election_committees",
                column: "committee_id");

            migrationBuilder.CreateIndex(
                name: "ix_general_election_committees_committee_level_id",
                schema: "data",
                table: "general_election_committees",
                column: "committee_level_id");

            migrationBuilder.CreateIndex(
                name: "ix_general_election_committees_committee_type_id",
                schema: "data",
                table: "general_election_committees",
                column: "committee_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_general_election_committees_department_id",
                schema: "data",
                table: "general_election_committees",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_general_election_committees_legal_form_id",
                schema: "data",
                table: "general_election_committees",
                column: "legal_form_id");

            migrationBuilder.CreateIndex(
                name: "ix_general_election_committees_office_id",
                schema: "data",
                table: "general_election_committees",
                column: "office_id");

            migrationBuilder.CreateIndex(
                name: "ix_general_election_committees_term_of_office_date_id",
                schema: "data",
                table: "general_election_committees",
                column: "term_of_office_date_id");

            migrationBuilder.CreateIndex(
                name: "ix_general_election_committees_term_of_office_id",
                schema: "data",
                table: "general_election_committees",
                column: "term_of_office_id");

            migrationBuilder.CreateIndex(
                name: "ix_general_gender_measures_department_id",
                schema: "data",
                table: "general_gender_measures",
                column: "department_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_general_gender_measures_ogd_id",
                schema: "data",
                table: "general_gender_measures",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_general_language_measures_department_id",
                schema: "data",
                table: "general_language_measures",
                column: "department_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_general_language_measures_ogd_id",
                schema: "data",
                table: "general_language_measures",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_interest_committees_ogd_id",
                schema: "data",
                table: "interest_committees",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_interest_committees_uri",
                schema: "data",
                table: "interest_committees",
                column: "uri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_interest_functions_ogd_id",
                schema: "data",
                table: "interest_functions",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_interest_functions_uri",
                schema: "data",
                table: "interest_functions",
                column: "uri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_interest_legal_forms_ogd_id",
                schema: "data",
                table: "interest_legal_forms",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_interest_legal_forms_uri",
                schema: "data",
                table: "interest_legal_forms",
                column: "uri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_interests_interest_committee_id",
                schema: "data",
                table: "interests",
                column: "interest_committee_id");

            migrationBuilder.CreateIndex(
                name: "ix_interests_interest_function_id",
                schema: "data",
                table: "interests",
                column: "interest_function_id");

            migrationBuilder.CreateIndex(
                name: "ix_interests_interest_legal_form_id",
                schema: "data",
                table: "interests",
                column: "interest_legal_form_id");

            migrationBuilder.CreateIndex(
                name: "ix_interests_legal_form_id",
                schema: "data",
                table: "interests",
                column: "legal_form_id");

            migrationBuilder.CreateIndex(
                name: "ix_interests_person_id",
                schema: "data",
                table: "interests",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "ix_languages_ogd_id",
                schema: "data",
                table: "languages",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_languages_uri",
                schema: "data",
                table: "languages",
                column: "uri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_legal_forms_ogd_id",
                schema: "data",
                table: "legal_forms",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_legal_forms_uri",
                schema: "data",
                table: "legal_forms",
                column: "uri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_legislature_periods_ogd_id",
                schema: "data",
                table: "legislature_periods",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_legislature_periods_uri",
                schema: "data",
                table: "legislature_periods",
                column: "uri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_membership_candidate_log_messages_general_election_committe",
                schema: "data",
                table: "membership_candidate_log_messages",
                column: "general_election_committee_id");

            migrationBuilder.CreateIndex(
                name: "ix_membership_candidate_log_messages_person_id",
                schema: "data",
                table: "membership_candidate_log_messages",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "ix_membership_candidates_election_office_id",
                schema: "data",
                table: "membership_candidates",
                column: "election_office_id");

            migrationBuilder.CreateIndex(
                name: "ix_membership_candidates_election_type_id",
                schema: "data",
                table: "membership_candidates",
                column: "election_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_membership_candidates_function_id",
                schema: "data",
                table: "membership_candidates",
                column: "function_id");

            migrationBuilder.CreateIndex(
                name: "ix_membership_candidates_gender_id",
                schema: "data",
                table: "membership_candidates",
                column: "gender_id");

            migrationBuilder.CreateIndex(
                name: "ix_membership_candidates_general_election_committee_id",
                schema: "data",
                table: "membership_candidates",
                column: "general_election_committee_id");

            migrationBuilder.CreateIndex(
                name: "ix_membership_candidates_language_id",
                schema: "data",
                table: "membership_candidates",
                column: "language_id");

            migrationBuilder.CreateIndex(
                name: "ix_membership_candidates_membership_addition_id",
                schema: "data",
                table: "membership_candidates",
                column: "membership_addition_id");

            migrationBuilder.CreateIndex(
                name: "ix_membership_candidates_membership_id",
                schema: "data",
                table: "membership_candidates",
                column: "membership_id");

            migrationBuilder.CreateIndex(
                name: "ix_membership_candidates_person_id",
                schema: "data",
                table: "membership_candidates",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "ix_memberships_committee_id",
                schema: "data",
                table: "memberships",
                column: "committee_id");

            migrationBuilder.CreateIndex(
                name: "ix_memberships_election_office_id",
                schema: "data",
                table: "memberships",
                column: "election_office_id");

            migrationBuilder.CreateIndex(
                name: "ix_memberships_election_type_id",
                schema: "data",
                table: "memberships",
                column: "election_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_memberships_function_id",
                schema: "data",
                table: "memberships",
                column: "function_id");

            migrationBuilder.CreateIndex(
                name: "ix_memberships_membership_addition_id",
                schema: "data",
                table: "memberships",
                column: "membership_addition_id");

            migrationBuilder.CreateIndex(
                name: "ix_memberships_ogd_id",
                schema: "data",
                table: "memberships",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_memberships_person_id",
                schema: "data",
                table: "memberships",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "ix_occupations_ogd_id",
                schema: "data",
                table: "occupations",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_occupations_text_de",
                schema: "data",
                table: "occupations",
                column: "text_de");

            migrationBuilder.CreateIndex(
                name: "ix_occupations_text_female_de",
                schema: "data",
                table: "occupations",
                column: "text_female_de");

            migrationBuilder.CreateIndex(
                name: "ix_occupations_text_female_fr",
                schema: "data",
                table: "occupations",
                column: "text_female_fr");

            migrationBuilder.CreateIndex(
                name: "ix_occupations_text_female_it",
                schema: "data",
                table: "occupations",
                column: "text_female_it");

            migrationBuilder.CreateIndex(
                name: "ix_occupations_text_fr",
                schema: "data",
                table: "occupations",
                column: "text_fr");

            migrationBuilder.CreateIndex(
                name: "ix_occupations_text_it",
                schema: "data",
                table: "occupations",
                column: "text_it");

            migrationBuilder.CreateIndex(
                name: "ix_occupations_uri",
                schema: "data",
                table: "occupations",
                column: "uri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_offices_department_id",
                schema: "data",
                table: "offices",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_offices_ogd_id",
                schema: "data",
                table: "offices",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_offices_uri",
                schema: "data",
                table: "offices",
                column: "uri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_person_legislature_period_persons_id",
                schema: "data",
                table: "person_legislature_period",
                column: "persons_id");

            migrationBuilder.CreateIndex(
                name: "ix_person_occupations_persons_id",
                schema: "data",
                table: "person_occupations",
                column: "persons_id");

            migrationBuilder.CreateIndex(
                name: "ix_persons_correspondence_address_id",
                schema: "data",
                table: "persons",
                column: "correspondence_address_id");

            migrationBuilder.CreateIndex(
                name: "ix_persons_correspondence_language_id",
                schema: "data",
                table: "persons",
                column: "correspondence_language_id");

            migrationBuilder.CreateIndex(
                name: "ix_persons_council_id",
                schema: "data",
                table: "persons",
                column: "council_id");

            migrationBuilder.CreateIndex(
                name: "ix_persons_gender_id",
                schema: "data",
                table: "persons",
                column: "gender_id");

            migrationBuilder.CreateIndex(
                name: "ix_persons_language_id",
                schema: "data",
                table: "persons",
                column: "language_id");

            migrationBuilder.CreateIndex(
                name: "ix_persons_office_address_id",
                schema: "data",
                table: "persons",
                column: "office_address_id");

            migrationBuilder.CreateIndex(
                name: "ix_persons_office_id",
                schema: "data",
                table: "persons",
                column: "office_id");

            migrationBuilder.CreateIndex(
                name: "ix_persons_ogd_id",
                schema: "data",
                table: "persons",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_persons_private_address_id",
                schema: "data",
                table: "persons",
                column: "private_address_id");

            migrationBuilder.CreateIndex(
                name: "ix_persons_salutation_id",
                schema: "data",
                table: "persons",
                column: "salutation_id");

            migrationBuilder.CreateIndex(
                name: "ix_salutations_gender_id",
                schema: "data",
                table: "salutations",
                column: "gender_id");

            migrationBuilder.CreateIndex(
                name: "ix_salutations_ogd_id",
                schema: "data",
                table: "salutations",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_salutations_uri",
                schema: "data",
                table: "salutations",
                column: "uri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_term_of_office_dates_ogd_id",
                schema: "data",
                table: "term_of_office_dates",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_term_of_office_dates_uri",
                schema: "data",
                table: "term_of_office_dates",
                column: "uri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_terms_of_office_ogd_id",
                schema: "data",
                table: "terms_of_office",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_terms_of_office_uri",
                schema: "data",
                table: "terms_of_office",
                column: "uri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_worklist_task_states_ogd_id",
                schema: "data",
                table: "worklist_task_states",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_worklist_task_states_uri",
                schema: "data",
                table: "worklist_task_states",
                column: "uri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_worklist_task_types_ogd_id",
                schema: "data",
                table: "worklist_task_types",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_worklist_task_types_uri",
                schema: "data",
                table: "worklist_task_types",
                column: "uri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_worklist_tasks_assigned_by_id",
                schema: "data",
                table: "worklist_tasks",
                column: "assigned_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_worklist_tasks_assigned_to_id",
                schema: "data",
                table: "worklist_tasks",
                column: "assigned_to_id");

            migrationBuilder.CreateIndex(
                name: "ix_worklist_tasks_committee_id",
                schema: "data",
                table: "worklist_tasks",
                column: "committee_id");

            migrationBuilder.CreateIndex(
                name: "ix_worklist_tasks_department_id",
                schema: "data",
                table: "worklist_tasks",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_worklist_tasks_general_election_committee_id",
                schema: "data",
                table: "worklist_tasks",
                column: "general_election_committee_id");

            migrationBuilder.CreateIndex(
                name: "ix_worklist_tasks_membership_candidate_id",
                schema: "data",
                table: "worklist_tasks",
                column: "membership_candidate_id");

            migrationBuilder.CreateIndex(
                name: "ix_worklist_tasks_membership_id",
                schema: "data",
                table: "worklist_tasks",
                column: "membership_id");

            migrationBuilder.CreateIndex(
                name: "ix_worklist_tasks_office_id",
                schema: "data",
                table: "worklist_tasks",
                column: "office_id");

            migrationBuilder.CreateIndex(
                name: "ix_worklist_tasks_parent_task_id",
                schema: "data",
                table: "worklist_tasks",
                column: "parent_task_id");

            migrationBuilder.CreateIndex(
                name: "ix_worklist_tasks_person_id",
                schema: "data",
                table: "worklist_tasks",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "ix_worklist_tasks_term_of_office_date_id",
                schema: "data",
                table: "worklist_tasks",
                column: "term_of_office_date_id");

            migrationBuilder.CreateIndex(
                name: "ix_worklist_tasks_worklist_task_state_id",
                schema: "data",
                table: "worklist_tasks",
                column: "worklist_task_state_id");

            migrationBuilder.CreateIndex(
                name: "ix_worklist_tasks_worklist_task_type_id",
                schema: "data",
                table: "worklist_tasks",
                column: "worklist_task_type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "apg_general_settings",
                schema: "data");

            migrationBuilder.DropTable(
                name: "appointment_decisions",
                schema: "data");

            migrationBuilder.DropTable(
                name: "committee_membership_addition",
                schema: "data");

            migrationBuilder.DropTable(
                name: "contact_points",
                schema: "data");

            migrationBuilder.DropTable(
                name: "entity_audit_log",
                schema: "data");

            migrationBuilder.DropTable(
                name: "general_gender_measures",
                schema: "data");

            migrationBuilder.DropTable(
                name: "general_language_measures",
                schema: "data");

            migrationBuilder.DropTable(
                name: "interests",
                schema: "data");

            migrationBuilder.DropTable(
                name: "membership_candidate_log_messages",
                schema: "data");

            migrationBuilder.DropTable(
                name: "person_legislature_period",
                schema: "data");

            migrationBuilder.DropTable(
                name: "person_occupations",
                schema: "data");

            migrationBuilder.DropTable(
                name: "worklist_tasks",
                schema: "data");

            migrationBuilder.DropTable(
                name: "appointment_decision_link_types",
                schema: "data");

            migrationBuilder.DropTable(
                name: "appointment_decision_types",
                schema: "data");

            migrationBuilder.DropTable(
                name: "document_storages",
                schema: "data");

            migrationBuilder.DropTable(
                name: "contact_point_types",
                schema: "data");

            migrationBuilder.DropTable(
                name: "interest_committees",
                schema: "data");

            migrationBuilder.DropTable(
                name: "interest_functions",
                schema: "data");

            migrationBuilder.DropTable(
                name: "interest_legal_forms",
                schema: "data");

            migrationBuilder.DropTable(
                name: "legislature_periods",
                schema: "data");

            migrationBuilder.DropTable(
                name: "occupations",
                schema: "data");

            migrationBuilder.DropTable(
                name: "eiam_assignments",
                schema: "data");

            migrationBuilder.DropTable(
                name: "membership_candidates",
                schema: "data");

            migrationBuilder.DropTable(
                name: "worklist_task_states",
                schema: "data");

            migrationBuilder.DropTable(
                name: "worklist_task_types",
                schema: "data");

            migrationBuilder.DropTable(
                name: "general_election_committees",
                schema: "data");

            migrationBuilder.DropTable(
                name: "memberships",
                schema: "data");

            migrationBuilder.DropTable(
                name: "candidate_list_states",
                schema: "data");

            migrationBuilder.DropTable(
                name: "committees",
                schema: "data");

            migrationBuilder.DropTable(
                name: "election_offices",
                schema: "data");

            migrationBuilder.DropTable(
                name: "election_types",
                schema: "data");

            migrationBuilder.DropTable(
                name: "functions",
                schema: "data");

            migrationBuilder.DropTable(
                name: "membership_additions",
                schema: "data");

            migrationBuilder.DropTable(
                name: "persons",
                schema: "data");

            migrationBuilder.DropTable(
                name: "committee_levels",
                schema: "data");

            migrationBuilder.DropTable(
                name: "committee_types",
                schema: "data");

            migrationBuilder.DropTable(
                name: "legal_forms",
                schema: "data");

            migrationBuilder.DropTable(
                name: "term_of_office_dates",
                schema: "data");

            migrationBuilder.DropTable(
                name: "terms_of_office",
                schema: "data");

            migrationBuilder.DropTable(
                name: "addresses",
                schema: "data");

            migrationBuilder.DropTable(
                name: "councils",
                schema: "data");

            migrationBuilder.DropTable(
                name: "languages",
                schema: "data");

            migrationBuilder.DropTable(
                name: "offices",
                schema: "data");

            migrationBuilder.DropTable(
                name: "salutations",
                schema: "data");

            migrationBuilder.DropTable(
                name: "cantons",
                schema: "data");

            migrationBuilder.DropTable(
                name: "departments",
                schema: "data");

            migrationBuilder.DropTable(
                name: "genders",
                schema: "data");
        }
    }
}
