using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class IntroduceCountryMasterData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "country_id",
                schema: "data",
                table: "addresses",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "countries",
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
                    table.PrimaryKey("pk_countries", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_addresses_country_id",
                schema: "data",
                table: "addresses",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "ix_countries_ogd_id",
                schema: "data",
                table: "countries",
                column: "ogd_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_countries_uri",
                schema: "data",
                table: "countries",
                column: "uri",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_addresses_countries_country_id",
                schema: "data",
                table: "addresses",
                column: "country_id",
                principalSchema: "data",
                principalTable: "countries",
                principalColumn: "id");

            migrationBuilder.Sql(@$"

            INSERT INTO data.countries (""id"", ""created"", ""created_by"", ""modified"", ""modified_by"", ""ogd_id"", ""is_deleted"", ""text_de"", ""text_fr"", ""text_it"", ""text_rm"", ""description_de"", ""description_fr"", ""description_it"", ""description_rm"", ""sort"", ""uri"", ""old_id"")
                VALUES
                    ('f3c9c18b-9596-487d-b921-7f122a3d1226', now(), 'CountrySyncService', now(), 'CountrySyncService',316, false, 'CY', 'CY', 'CY', '', 'Zypern', 'Chypre', 'Cipro', '', 0, 'https://ld.admin.ch/country/CYP', 0),
                    ('e6b847d5-5df1-4945-851c-97247065bbb8', now(), 'CountrySyncService', now(), 'CountrySyncService',317, false, 'ES', 'ES', 'ES', '', 'Spanien', 'Espagne', 'Spagna', '', 0, 'https://ld.admin.ch/country/ESP', 0),
                    ('32958cf9-6b35-4ad8-8c6b-ae0435371022', now(), 'CountrySyncService', now(), 'CountrySyncService',318, false, 'IE', 'IE', 'IE', '', 'Irland', 'Irlande', 'Irlanda', '', 0, 'https://ld.admin.ch/country/IRL', 0),
                    ('45c365da-9b10-4350-b711-5f2d193e5e24', now(), 'CountrySyncService', now(), 'CountrySyncService',334, false, 'KR', 'KR', 'KR', '', 'Südkorea', 'Corée du Sud', 'Corea del Sud', '', 0, 'https://ld.admin.ch/country/KOR', 0),
                    ('70fff16f-d793-4e1b-855f-fe9beb27ff0f', now(), 'CountrySyncService', now(), 'CountrySyncService',380, false, 'SM', 'SM', 'SM', '', 'San Marino', 'Saint-Marin', 'San Marino', '', 0, 'https://ld.admin.ch/country/SMR', 0),
                    ('7de268ba-906b-402d-8cf8-da7b12b9d8cc', now(), 'CountrySyncService', now(), 'CountrySyncService',420, false, 'Europäische Union', 'Union européenne', 'Unione europea', '', 'Europäische Union', 'Union européenne', 'Unione europea', '', 0, 'https://ld.admin.ch/country/EUR', 0),
                    ('c4097256-d66b-47c5-978d-082fb5054209', now(), 'CountrySyncService', now(), 'CountrySyncService',285, false, 'LI', 'LI', 'LI', '', 'Liechtenstein', 'Liechtenstein', 'Liechtenstein', '', 0, 'https://ld.admin.ch/country/LIE', 0),
                    ('8368f092-2614-400b-8eff-9f186f5d8799', now(), 'CountrySyncService', now(), 'CountrySyncService',286, false, 'CH', 'CH', 'CH', '', 'Schweiz', 'Suisse', 'Svizzera', '', 0, 'https://ld.admin.ch/country/CHE', 0),
                    ('1c3b6922-cbc3-4e44-a651-e02d07408933', now(), 'CountrySyncService', now(), 'CountrySyncService',287, false, 'DE', 'DE', 'DE', '', 'Deutschland', 'Allemagne', 'Germania', '', 0, 'https://ld.admin.ch/country/DEU', 0),
                    ('fa845e9c-d154-493b-9e28-237e888514e4', now(), 'CountrySyncService', now(), 'CountrySyncService',288, false, 'IT', 'IT', 'IT', '', 'Italien', 'Italie', 'Italia', '', 0, 'https://ld.admin.ch/country/ITA', 0),
                    ('deea7b2e-7202-43a0-aff6-db5a53fcc2c2', now(), 'CountrySyncService', now(), 'CountrySyncService',289, false, 'BI', 'BI', 'BI', '', 'Burundi', 'Burundi', 'Burundi', '', 0, 'https://ld.admin.ch/country/BDI', 0),
                    ('b672c877-0d9d-4106-8a1d-546793c65aa3', now(), 'CountrySyncService', now(), 'CountrySyncService',290, false, 'BJ', 'BJ', 'BJ', '', 'Benin', 'Bénin', 'Benin', '', 0, 'https://ld.admin.ch/country/BEN', 0),
                    ('e0f95966-3e88-4f62-90c8-c37a881dafa8', now(), 'CountrySyncService', now(), 'CountrySyncService',291, false, 'CF', 'CF', 'CF', '', 'Zentralafrikanische Republik', 'République centrafricaine', 'Repubblica centrafricana', '', 0, 'https://ld.admin.ch/country/CAF', 0),
                    ('185fb0d4-c910-4a54-8616-86fb75900632', now(), 'CountrySyncService', now(), 'CountrySyncService',292, false, 'CD', 'CD', 'CD', '', 'Demokratische Republik Kongo', 'République démocratique du Congo', 'Repubblica democratica del Congo', '', 0, 'https://ld.admin.ch/country/COD', 0),
                    ('e130203d-4eef-4cd2-9586-64c30f10aca2', now(), 'CountrySyncService', now(), 'CountrySyncService',293, false, 'BH', 'BH', 'BH', '', 'Bahrain', 'Bahreïn', 'Bahrein', '', 0, 'https://ld.admin.ch/country/BHR', 0),
                    ('e6fd661b-63c8-480a-8782-ac95a7da5e98', now(), 'CountrySyncService', now(), 'CountrySyncService',294, false, 'KW', 'KW', 'KW', '', 'Kuwait', 'Koweït', 'Kuwait', '', 0, 'https://ld.admin.ch/country/KWT', 0),
                    ('fb5ef7ba-8fe9-497e-b380-47c932258f24', now(), 'CountrySyncService', now(), 'CountrySyncService',295, false, 'OM', 'OM', 'OM', '', 'Oman', 'Oman', 'Oman', '', 0, 'https://ld.admin.ch/country/OMN', 0),
                    ('89df08ec-b96b-45e0-a66c-fd22d72c2e09', now(), 'CountrySyncService', now(), 'CountrySyncService',296, false, 'QA', 'QA', 'QA', '', 'Katar', 'Qatar', 'Qatar', '', 0, 'https://ld.admin.ch/country/QAT', 0),
                    ('72ec9e2e-431f-43ea-8f1f-c8ac867b4613', now(), 'CountrySyncService', now(), 'CountrySyncService',297, false, 'AF', 'AF', 'AF', '', 'Afghanistan', 'Afghanistan', 'Afghanistan', '', 0, 'https://ld.admin.ch/country/AFG', 0),
                    ('677e8346-5692-45b5-bfb0-17919cfb80f6', now(), 'CountrySyncService', now(), 'CountrySyncService',298, false, 'TV', 'TV', 'TV', '', 'Tuvalu', 'Tuvalu', 'Tuvalu', '', 0, 'https://ld.admin.ch/country/TUV', 0),
                    ('85ef2c1b-3126-44d3-acd7-25ca19360be2', now(), 'CountrySyncService', now(), 'CountrySyncService',299, false, 'WS', 'WS', 'WS', '', 'Samoa', 'Samoa', 'Samoa', '', 0, 'https://ld.admin.ch/country/WSM', 0),
                    ('c63c4534-c61e-4105-9c42-d93834e9820c', now(), 'CountrySyncService', now(), 'CountrySyncService',300, false, 'SB', 'SB', 'SB', '', 'Salomonen', 'Îles Salomon', 'Isole Salomone', '', 0, 'https://ld.admin.ch/country/SLB', 0),
                    ('336767af-f02e-4fed-a191-0b8ebcc08a3f', now(), 'CountrySyncService', now(), 'CountrySyncService',301, false, 'BE', 'BE', 'BE', '', 'Belgien', 'Belgique', 'Belgio', '', 0, 'https://ld.admin.ch/country/BEL', 0),
                    ('441b83a7-6cff-4ffb-bd86-c8a73bc25668', now(), 'CountrySyncService', now(), 'CountrySyncService',302, false, 'DK', 'DK', 'DK', '', 'Dänemark', 'Danemark', 'Danimarca', '', 0, 'https://ld.admin.ch/country/DNK', 0),
                    ('761e3326-d839-44af-8913-86304979a996', now(), 'CountrySyncService', now(), 'CountrySyncService',303, false, 'FR', 'FR', 'FR', '', 'Frankreich', 'France', 'Francia', '', 0, 'https://ld.admin.ch/country/FRA', 0),
                    ('bb561672-6d00-4ec1-8161-02f89325f422', now(), 'CountrySyncService', now(), 'CountrySyncService',304, false, 'UK', 'UK', 'UK', '', 'Vereinigtes Königreich', 'Royaume-Uni', 'Regno Unito', '', 0, 'https://ld.admin.ch/country/GBR', 0),
                    ('a1dbebc5-1b05-4288-82e7-f6cc1c024995', now(), 'CountrySyncService', now(), 'CountrySyncService',305, false, 'GR', 'GR', 'GR', '', 'Griechenland', 'Grèce', 'Grecia', '', 0, 'https://ld.admin.ch/country/GRC', 0),
                    ('ebe5540c-4509-4433-8053-b4db99e01cbc', now(), 'CountrySyncService', now(), 'CountrySyncService',306, false, 'HR', 'HR', 'HR', '', 'Kroatien', 'Croatie', 'Croazia', '', 0, 'https://ld.admin.ch/country/HRV', 0),
                    ('72fac4ad-fe95-430c-b583-b19b9c12fc45', now(), 'CountrySyncService', now(), 'CountrySyncService',307, false, 'HU', 'HU', 'HU', '', 'Ungarn', 'Hongrie', 'Ungheria', '', 0, 'https://ld.admin.ch/country/HUN', 0),
                    ('6c7f1f1a-2ce5-4b9c-b838-0f2e9712005c', now(), 'CountrySyncService', now(), 'CountrySyncService',308, false, 'LT', 'LT', 'LT', '', 'Litauen', 'Lituanie', 'Lituania', '', 0, 'https://ld.admin.ch/country/LTU', 0),
                    ('596f43fb-5d15-4cc7-9eb6-5fdbbfe9a14e', now(), 'CountrySyncService', now(), 'CountrySyncService',309, false, 'MX', 'MX', 'MX', '', 'Mexiko', 'Mexique', 'Messico', '', 0, 'https://ld.admin.ch/country/MEX', 0),
                    ('d4c5a840-1895-4166-8e59-ebed22e471e3', now(), 'CountrySyncService', now(), 'CountrySyncService',310, false, 'NL', 'NL', 'NL', '', 'Niederlande', 'Pays-Bas', 'Paesi Bassi', '', 0, 'https://ld.admin.ch/country/NLD', 0),
                    ('9c636857-03f1-4b7c-a267-4c6d8252a390', now(), 'CountrySyncService', now(), 'CountrySyncService',311, false, 'PL', 'PL', 'PL', '', 'Polen', 'Pologne', 'Polonia', '', 0, 'https://ld.admin.ch/country/POL', 0),
                    ('18fc025a-6cc4-4f2f-af60-9b84bafbf827', now(), 'CountrySyncService', now(), 'CountrySyncService',312, false, 'PT', 'PT', 'PT', '', 'Portugal', 'Portugal', 'Portogallo', '', 0, 'https://ld.admin.ch/country/PRT', 0),
                    ('3e260d05-8c61-4e80-bf67-c90fed6488fb', now(), 'CountrySyncService', now(), 'CountrySyncService',313, false, 'SE', 'SE', 'SE', '', 'Schweden', 'Suède', 'Svezia', '', 0, 'https://ld.admin.ch/country/SWE', 0),
                    ('dd61ff12-95da-4fd3-86c0-2d6fca5544eb', now(), 'CountrySyncService', now(), 'CountrySyncService',314, false, 'AT', 'AT', 'AT', '', 'Österreich', 'Autriche', 'Austria', '', 0, 'https://ld.admin.ch/country/AUT', 0),
                    ('037844f4-ac9f-477f-9978-af8c1cdfa1a8', now(), 'CountrySyncService', now(), 'CountrySyncService',315, false, 'BG', 'BG', 'BG', '', 'Bulgarien', 'Bulgarie', 'Bulgaria', '', 0, 'https://ld.admin.ch/country/BGR', 0),
                    ('9a40ff01-2a62-48b0-9499-667c3d7a8af0', now(), 'CountrySyncService', now(), 'CountrySyncService',319, false, 'MA', 'MA', 'MA', '', 'Marokko', 'Maroc', 'Marocco', '', 0, 'https://ld.admin.ch/country/MAR', 0),
                    ('146efe6f-d8bb-4ff6-9a72-40619f600e89', now(), 'CountrySyncService', now(), 'CountrySyncService',322, false, 'CZ', 'CZ', 'CZ', '', 'Tschechien', 'Tchéquie', 'Cechia', '', 0, 'https://ld.admin.ch/country/CZE', 0),
                    ('8f14c04e-6cd8-45d1-aabf-180a392e120c', now(), 'CountrySyncService', now(), 'CountrySyncService',325, false, 'LV', 'LV', 'LV', '', 'Lettland', 'Lettonie', 'Lettonia', '', 0, 'https://ld.admin.ch/country/LVA', 0),
                    ('4238a827-c275-4573-a801-86703a65cd0d', now(), 'CountrySyncService', now(), 'CountrySyncService',328, false, 'LU', 'LU', 'LU', '', 'Luxemburg', 'Luxembourg', 'Lussemburgo', '', 0, 'https://ld.admin.ch/country/LUX', 0),
                    ('da39790d-8eea-47c4-b8a7-e8174b344aea', now(), 'CountrySyncService', now(), 'CountrySyncService',331, false, 'SN', 'SN', 'SN', '', 'Senegal', 'Sénégal', 'Senegal', '', 0, 'https://ld.admin.ch/country/SEN', 0),
                    ('9971c049-f239-4c2e-a4d0-82c18c4d8454', now(), 'CountrySyncService', now(), 'CountrySyncService',337, false, 'JO', 'JO', 'JO', '', 'Jordanien', 'Jordanie', 'Giordania', '', 0, 'https://ld.admin.ch/country/JOR', 0),
                    ('a3d17945-7d7c-4bec-9362-dc4767789a83', now(), 'CountrySyncService', now(), 'CountrySyncService',340, false, 'TN', 'TN', 'TN', '', 'Tunesien', 'Tunisie', 'Tunisia', '', 0, 'https://ld.admin.ch/country/TUN', 0),
                    ('ed85ae0d-d0f0-4109-86d8-8b870adf32ef', now(), 'CountrySyncService', now(), 'CountrySyncService',343, false, 'AU', 'AU', 'AU', '', 'Australien', 'Australie', 'Australia', '', 0, 'https://ld.admin.ch/country/AUS', 0),
                    ('1a7aa893-27c7-4a76-899a-68f30c3f21d4', now(), 'CountrySyncService', now(), 'CountrySyncService',346, false, 'TM', 'TM', 'TM', '', 'Turkmenistan', 'Turkménistan', 'Turkmenistan', '', 0, 'https://ld.admin.ch/country/TKM', 0),
                    ('f3203c73-f259-4db9-a91d-b06749185a97', now(), 'CountrySyncService', now(), 'CountrySyncService',349, false, 'EG', 'EG', 'EG', '', 'Ägypten', 'Égypte', 'Egitto', '', 0, 'https://ld.admin.ch/country/EGY', 0),
                    ('3c6fae76-1863-430d-aa84-2f012c8daaa8', now(), 'CountrySyncService', now(), 'CountrySyncService',352, false, 'BD', 'BD', 'BD', '', 'Bangladesch', 'Bangladesh', 'Bangladesh', '', 0, 'https://ld.admin.ch/country/BGD', 0),
                    ('7d1c9f19-3527-469f-9702-fbb4d4b7497c', now(), 'CountrySyncService', now(), 'CountrySyncService',355, false, 'BZ', 'BZ', 'BZ', '', 'Belize', 'Belize', 'Belize', '', 0, 'https://ld.admin.ch/country/BLZ', 0),
                    ('8aa0174d-0c37-4e64-94bb-a7dac2ef02b0', now(), 'CountrySyncService', now(), 'CountrySyncService',358, false, 'CG', 'CG', 'CG', '', 'Kongo', 'Congo', 'Congo', '', 0, 'https://ld.admin.ch/country/COG', 0),
                    ('1578f94d-1324-4d7b-8bd8-77bbad294588', now(), 'CountrySyncService', now(), 'CountrySyncService',361, false, 'KE', 'KE', 'KE', '', 'Kenia', 'Kenya', 'Kenya', '', 0, 'https://ld.admin.ch/country/KEN', 0),
                    ('f63cbfcf-b617-4139-8344-adc4428fb791', now(), 'CountrySyncService', now(), 'CountrySyncService',364, false, 'SR', 'SR', 'SR', '', 'Suriname', 'Suriname', 'Suriname', '', 0, 'https://ld.admin.ch/country/SUR', 0),
                    ('9d54e300-bd42-40ac-bd3b-f7262d9fbb29', now(), 'CountrySyncService', now(), 'CountrySyncService',367, false, 'TZ', 'TZ', 'TZ', '', 'Tansania', 'Tanzanie', 'Tanzania', '', 0, 'https://ld.admin.ch/country/TZA', 0),
                    ('210949b5-887a-4c3a-8252-c140db9337ee', now(), 'CountrySyncService', now(), 'CountrySyncService',370, false, 'ZW', 'ZW', 'ZW', '', 'Simbabwe', 'Zimbabwe', 'Zimbabwe', '', 0, 'https://ld.admin.ch/country/ZWE', 0),
                    ('1ca8f599-1b36-4b7e-a0ef-006b46700d82', now(), 'CountrySyncService', now(), 'CountrySyncService',373, false, 'SL', 'SL', 'SL', '', 'Sierra Leone', 'Sierra Leone', 'Sierra Leone', '', 0, 'https://ld.admin.ch/country/SLE', 0),
                    ('83c03e17-f0af-4782-8059-1cbcc8855ec8', now(), 'CountrySyncService', now(), 'CountrySyncService',376, false, 'PA', 'PA', 'PA', '', 'Panama', 'Panama', 'Panama', '', 0, 'https://ld.admin.ch/country/PAN', 0),
                    ('7332589b-7ca2-4513-9699-7280484f4aa6', now(), 'CountrySyncService', now(), 'CountrySyncService',379, false, 'RU', 'RU', 'RU', '', 'Russland', 'Russie', 'Russia', '', 0, 'https://ld.admin.ch/country/RUS', 0),
                    ('c660f0f7-63cc-4495-818c-f99945ea9bc3', now(), 'CountrySyncService', now(), 'CountrySyncService',382, false, 'TR', 'TR', 'TR', '', 'Türkei', 'Turquie', 'Turchia', '', 0, 'https://ld.admin.ch/country/TUR', 0),
                    ('6c65e8ca-62d1-4eb2-a559-314af6af75a7', now(), 'CountrySyncService', now(), 'CountrySyncService',385, false, 'CN', 'CN', 'CN', '', 'China', 'Chine', 'Cina', '', 0, 'https://ld.admin.ch/country/CHN', 0),
                    ('1b3b01fb-2c66-4f26-9949-e7221c45ae32', now(), 'CountrySyncService', now(), 'CountrySyncService',388, false, 'IL', 'IL', 'IL', '', 'Israel', 'Israël', 'Israele', '', 0, 'https://ld.admin.ch/country/ISR', 0),
                    ('f56e4d5b-825e-4408-a5a4-c9db4ab3ccb3', now(), 'CountrySyncService', now(), 'CountrySyncService',391, false, 'BA', 'BA', 'BA', '', 'Bosnien und Herzegowina', 'Bosnie-Herzégovine', 'Bosnia-Erzegovina', '', 0, 'https://ld.admin.ch/country/BIH', 0),
                    ('e871dda5-06af-4f35-8c77-1a3d404cec4d', now(), 'CountrySyncService', now(), 'CountrySyncService',394, false, 'KN', 'KN', 'KN', '', 'St. Kitts und Nevis', 'Saint-Christophe-et-Niévès', 'Saint Kitts e Nevis', '', 0, 'https://ld.admin.ch/country/KNA', 0),
                    ('d7fa34fb-85c3-4391-b039-793fe8ca8fcf', now(), 'CountrySyncService', now(), 'CountrySyncService',397, false, 'VU', 'VU', 'VU', '', 'Vanuatu', 'Vanuatu', 'Vanuatu', '', 0, 'https://ld.admin.ch/country/VUT', 0),
                    ('30867557-fc72-44c9-9af9-eb7af3d8e852', now(), 'CountrySyncService', now(), 'CountrySyncService',400, false, 'TG', 'TG', 'TG', '', 'Togo', 'Togo', 'Togo', '', 0, 'https://ld.admin.ch/country/TGO', 0),
                    ('206fa5cb-88c6-4f97-a4e8-29d282bb670f', now(), 'CountrySyncService', now(), 'CountrySyncService',403, false, 'PG', 'PG', 'PG', '', 'Papua-Neuguinea', 'Papouasie - Nouvelle-Guinée', 'Papua Nuova Guinea', '', 0, 'https://ld.admin.ch/country/PNG', 0),
                    ('dbdc11d7-9492-4e5c-8872-4cbd5ac268dc', now(), 'CountrySyncService', now(), 'CountrySyncService',406, false, 'BR', 'BR', 'BR', '', 'Brasilien', 'Brésil', 'Brasile', '', 0, 'https://ld.admin.ch/country/BRA', 0),
                    ('27197dfe-9299-4183-8905-6c6694e81482', now(), 'CountrySyncService', now(), 'CountrySyncService',409, false, 'NE', 'NE', 'NE', '', 'Niger', 'Niger', 'Niger', '', 0, 'https://ld.admin.ch/country/NER', 0),
                    ('64637b1c-777e-4f16-9573-f6fda93127b9', now(), 'CountrySyncService', now(), 'CountrySyncService',412, false, 'TW', 'TW', 'TW', '', 'Taiwan', 'Taïwan', 'Taiwan', '', 0, 'https://ld.admin.ch/country/TWN', 0),
                    ('3dc6210a-ce69-4cb3-8909-f6a11514e66e', now(), 'CountrySyncService', now(), 'CountrySyncService',415, false, 'LK', 'LK', 'LK', '', 'Sri Lanka', 'Sri Lanka', 'Sri Lanka', '', 0, 'https://ld.admin.ch/country/LKA', 0),
                    ('5c802b7a-4a70-42e3-b33f-daad6819830e', now(), 'CountrySyncService', now(), 'CountrySyncService',418, false, 'AM', 'AM', 'AM', '', 'Armenien', 'Arménie', 'Armenia', '', 0, 'https://ld.admin.ch/country/ARM', 0),
                    ('3741b091-4d3e-49c8-8c08-ad4ac916938f', now(), 'CountrySyncService', now(), 'CountrySyncService',421, false, 'AL', 'AL', 'AL', '', 'Albanien', 'Albanie', 'Albania', '', 0, 'https://ld.admin.ch/country/ALB', 0),
                    ('1de55a37-b34a-4955-8b56-bca3739d2c03', now(), 'CountrySyncService', now(), 'CountrySyncService',424, false, 'UA', 'UA', 'UA', '', 'Ukraine', 'Ukraine', 'Ucraina', '', 0, 'https://ld.admin.ch/country/UKR', 0),
                    ('aacb021c-19ba-4440-aa57-b75c33e0435e', now(), 'CountrySyncService', now(), 'CountrySyncService',427, false, 'AO', 'AO', 'AO', '', 'Angola', 'Angola', 'Angola', '', 0, 'https://ld.admin.ch/country/AGO', 0),
                    ('2270e449-08a1-404c-afe4-172e26ca3ddd', now(), 'CountrySyncService', now(), 'CountrySyncService',430, false, 'DO', 'DO', 'DO', '', 'Dominikanische Republik', 'République dominicaine', 'Repubblica dominicana', '', 0, 'https://ld.admin.ch/country/DOM', 0),
                    ('639bc6a0-ad67-4e5f-a037-bf71b3e3954f', now(), 'CountrySyncService', now(), 'CountrySyncService',433, false, 'CR', 'CR', 'CR', '', 'Costa Rica', 'Costa Rica', 'Costa Rica', '', 0, 'https://ld.admin.ch/country/CRI', 0),
                    ('34b1b53e-a471-4fc6-b7df-1e728907ffab', now(), 'CountrySyncService', now(), 'CountrySyncService',436, false, 'GD', 'GD', 'GD', '', 'Grenada', 'Grenade', 'Grenada', '', 0, 'https://ld.admin.ch/country/GRD', 0),
                    ('5658d30e-2f5c-4326-85b6-b630498fcb30', now(), 'CountrySyncService', now(), 'CountrySyncService',439, false, 'NI', 'NI', 'NI', '', 'Nicaragua', 'Nicaragua', 'Nicaragua', '', 0, 'https://ld.admin.ch/country/NIC', 0),
                    ('c899c202-5d28-49fc-b43c-f7ee1d41c738', now(), 'CountrySyncService', now(), 'CountrySyncService',320, false, 'MT', 'MT', 'MT', '', 'Malta', 'Malte', 'Malta', '', 0, 'https://ld.admin.ch/country/MLT', 0),
                    ('0531d5db-7244-45c9-a155-bfd65e726615', now(), 'CountrySyncService', now(), 'CountrySyncService',323, false, 'EE', 'EE', 'EE', '', 'Estland', 'Estonie', 'Estonia', '', 0, 'https://ld.admin.ch/country/EST', 0),
                    ('6d3645bf-c7e3-493f-b722-ba043dcd9307', now(), 'CountrySyncService', now(), 'CountrySyncService',326, false, 'SK', 'SK', 'SK', '', 'Slowakei', 'Slovaquie', 'Slovacchia', '', 0, 'https://ld.admin.ch/country/SVK', 0),
                    ('a1716a87-5db2-406c-89dd-a5c3e2c1dbed', now(), 'CountrySyncService', now(), 'CountrySyncService',329, false, 'RO', 'RO', 'RO', '', 'Rumänien', 'Roumanie', 'Romania', '', 0, 'https://ld.admin.ch/country/ROU', 0),
                    ('f6f4e4d5-d846-493b-a9be-3e7aa45ab709', now(), 'CountrySyncService', now(), 'CountrySyncService',332, false, 'SC', 'SC', 'SC', '', 'Seychellen', 'Seychelles', 'Seychelles', '', 0, 'https://ld.admin.ch/country/SYC', 0),
                    ('842a6b09-4c24-4e62-afd2-e75b8c29c570', now(), 'CountrySyncService', now(), 'CountrySyncService',335, false, 'LB', 'LB', 'LB', '', 'Libanon', 'Liban', 'Libano', '', 0, 'https://ld.admin.ch/country/LBN', 0),
                    ('63f823d1-2aa2-46b9-9723-4a2ef172f7e3', now(), 'CountrySyncService', now(), 'CountrySyncService',338, false, 'KM', 'KM', 'KM', '', 'Komoren', 'Comores', 'Comore', '', 0, 'https://ld.admin.ch/country/COM', 0),
                    ('8231626b-74b8-4f0a-8284-de0523bb1c94', now(), 'CountrySyncService', now(), 'CountrySyncService',341, false, 'MK', 'MK', 'MK', '', 'Nordmazedonien', 'Macédoine du Nord', 'Macedonia del Nord', '', 0, 'https://ld.admin.ch/country/MKD', 0),
                    ('7c5045cd-3540-45dc-9b0c-d77a74a3292c', now(), 'CountrySyncService', now(), 'CountrySyncService',344, false, 'GN', 'GN', 'GN', '', 'Guinea', 'Guinée', 'Guinea', '', 0, 'https://ld.admin.ch/country/GIN', 0),
                    ('3466f8c4-fa3b-4ad6-a1b2-3cbda91c833a', now(), 'CountrySyncService', now(), 'CountrySyncService',347, false, 'IR', 'IR', 'IR', '', 'Iran', 'Iran', 'Iran', '', 0, 'https://ld.admin.ch/country/IRN', 0),
                    ('ca5bd6d2-8fbf-4e99-9a83-fae993aedd62', now(), 'CountrySyncService', now(), 'CountrySyncService',350, false, 'ID', 'ID', 'ID', '', 'Indonesien', 'Indonésie', 'Indonesia', '', 0, 'https://ld.admin.ch/country/IDN', 0),
                    ('de0c0c15-1fac-4805-9c6b-e6ee339e1449', now(), 'CountrySyncService', now(), 'CountrySyncService',353, false, 'UY', 'UY', 'UY', '', 'Uruguay', 'Uruguay', 'Uruguay', '', 0, 'https://ld.admin.ch/country/URY', 0),
                    ('fdfe67fd-fc59-4f92-8939-51888b9cd01d', now(), 'CountrySyncService', now(), 'CountrySyncService',356, false, 'BB', 'BB', 'BB', '', 'Barbados', 'Barbade', 'Barbados', '', 0, 'https://ld.admin.ch/country/BRB', 0),
                    ('049d9489-8727-4234-a980-bd93d680acdb', now(), 'CountrySyncService', now(), 'CountrySyncService',359, false, 'GY', 'GY', 'GY', '', 'Guyana', 'Guyana', 'Guyana', '', 0, 'https://ld.admin.ch/country/GUY', 0),
                    ('7969ca61-f52e-4852-bb6c-232d0a134ec1', now(), 'CountrySyncService', now(), 'CountrySyncService',362, false, 'MG', 'MG', 'MG', '', 'Madagaskar', 'Madagascar', 'Madagascar', '', 0, 'https://ld.admin.ch/country/MDG', 0),
                    ('02cc8645-9d76-4d33-83dd-6fd871c611fe', now(), 'CountrySyncService', now(), 'CountrySyncService',365, false, 'SZ', 'SZ', 'SZ', '', 'Eswatini', 'Eswatini', 'Eswatini', '', 0, 'https://ld.admin.ch/country/SWZ', 0),
                    ('c3aae676-a759-459c-815c-fd0b2bddeebf', now(), 'CountrySyncService', now(), 'CountrySyncService',368, false, 'UG', 'UG', 'UG', '', 'Uganda', 'Ouganda', 'Uganda', '', 0, 'https://ld.admin.ch/country/UGA', 0),
                    ('809f6a0d-6454-4e44-9274-9ea72e8c63a5', now(), 'CountrySyncService', now(), 'CountrySyncService',371, false, 'KG', 'KG', 'KG', '', 'Kirgisistan', 'Kirghizstan', 'Kirghizistan', '', 0, 'https://ld.admin.ch/country/KGZ', 0),
                    ('d214af6d-b0c0-47f6-94ed-b977d1558a6a', now(), 'CountrySyncService', now(), 'CountrySyncService',374, false, 'GM', 'GM', 'GM', '', 'Gambia', 'Gambie', 'Gambia', '', 0, 'https://ld.admin.ch/country/GMB', 0),
                    ('8f483a95-78a1-4735-9189-2d8bb981c79b', now(), 'CountrySyncService', now(), 'CountrySyncService',377, false, 'AD', 'AD', 'AD', '', 'Andorra', 'Andorre', 'Andorra', '', 0, 'https://ld.admin.ch/country/AND', 0),
                    ('4823c287-6938-46de-b5ff-d3dc72899083', now(), 'CountrySyncService', now(), 'CountrySyncService',383, false, 'IS', 'IS', 'IS', '', 'Island', 'Islande', 'Islanda', '', 0, 'https://ld.admin.ch/country/ISL', 0),
                    ('c6011639-1a27-4cb1-9693-7535388f47fd', now(), 'CountrySyncService', now(), 'CountrySyncService',386, false, 'NP', 'NP', 'NP', '', 'Nepal', 'Népal', 'Nepal', '', 0, 'https://ld.admin.ch/country/NPL', 0),
                    ('d5b6445a-e507-43fd-8344-61a8f59eaa19', now(), 'CountrySyncService', now(), 'CountrySyncService',389, false, 'JP', 'JP', 'JP', '', 'Japan', 'Japon', 'Giappone', '', 0, 'https://ld.admin.ch/country/JPN', 0),
                    ('98e2fb43-01de-4438-848d-4a32ee3f089b', now(), 'CountrySyncService', now(), 'CountrySyncService',392, false, 'IN', 'IN', 'IN', '', 'Indien', 'Inde', 'India', '', 0, 'https://ld.admin.ch/country/IND', 0),
                    ('256bd318-7397-4902-9f4c-ebd72c4fed0d', now(), 'CountrySyncService', now(), 'CountrySyncService',395, false, 'MC', 'MC', 'MC', '', 'Monaco', 'Monaco', 'Monaco', '', 0, 'https://ld.admin.ch/country/MCO', 0),
                    ('3882b1df-d1fb-414c-8c16-4b826d966c59', now(), 'CountrySyncService', now(), 'CountrySyncService',398, false, 'LA', 'LA', 'LA', '', 'Laos', 'Laos', 'Laos', '', 0, 'https://ld.admin.ch/country/LAO', 0),
                    ('c176d080-6787-44c5-aad6-f2dcce23b58d', now(), 'CountrySyncService', now(), 'CountrySyncService',401, false, 'BN', 'BN', 'BN', '', 'Brunei', 'Brunei', 'Brunei', '', 0, 'https://ld.admin.ch/country/BRN', 0),
                    ('dba30dc0-1812-461e-b144-f527f35a13ce', now(), 'CountrySyncService', now(), 'CountrySyncService',404, false, 'TJ', 'TJ', 'TJ', '', 'Tadschikistan', 'Tadjikistan', 'Tagikistan', '', 0, 'https://ld.admin.ch/country/TJK', 0),
                    ('9775a10f-1dec-4c77-8556-ce73428cfba3', now(), 'CountrySyncService', now(), 'CountrySyncService',407, false, 'NZ', 'NZ', 'NZ', '', 'Neuseeland', 'Nouvelle-Zélande', 'Nuova Zelanda', '', 0, 'https://ld.admin.ch/country/NZL', 0),
                    ('047cbdf6-11df-4487-8257-a2446503ed9c', now(), 'CountrySyncService', now(), 'CountrySyncService',410, false, 'UZ', 'UZ', 'UZ', '', 'Usbekistan', 'Ouzbékistan', 'Uzbekistan', '', 0, 'https://ld.admin.ch/country/UZB', 0),
                    ('58ac270f-a98d-4530-918f-901bcc1713ef', now(), 'CountrySyncService', now(), 'CountrySyncService',413, false, 'GT', 'GT', 'GT', '', 'Guatemala', 'Guatemala', 'Guatemala', '', 0, 'https://ld.admin.ch/country/GTM', 0),
                    ('ec3e8c16-51f0-4ebd-86c2-2bedad4e4793', now(), 'CountrySyncService', now(), 'CountrySyncService',416, false, 'FM', 'FM', 'FM', '', 'Mikronesien', 'Micronésie', 'Micronesia', '', 0, 'https://ld.admin.ch/country/FSM', 0),
                    ('76989d73-3cbd-4e69-9755-31ce08d8c69d', now(), 'CountrySyncService', now(), 'CountrySyncService',419, false, 'DM', 'DM', 'DM', '', 'Dominica', 'Dominique', 'Dominica', '', 0, 'https://ld.admin.ch/country/DMA', 0),
                    ('c53bd9e8-bc61-4f90-8dfe-2b0fa6a11a16', now(), 'CountrySyncService', now(), 'CountrySyncService',422, false, 'BF', 'BF', 'BF', '', 'Burkina Faso', 'Burkina Faso', 'Burkina Faso', '', 0, 'https://ld.admin.ch/country/BFA', 0),
                    ('36d4b7a8-2dc1-4102-8693-c9647663d9bc', now(), 'CountrySyncService', now(), 'CountrySyncService',425, false, 'SG', 'SG', 'SG', '', 'Singapur', 'Singapour', 'Singapore', '', 0, 'https://ld.admin.ch/country/SGP', 0),
                    ('2d15f459-cb9b-4890-875a-631d18695de0', now(), 'CountrySyncService', now(), 'CountrySyncService',428, false, 'AE', 'AE', 'AE', '', 'Vereinigte Arabische Emirate', 'Émirats arabes unis', 'Emirati arabi uniti', '', 0, 'https://ld.admin.ch/country/ARE', 0),
                    ('979b2b2c-892d-49e7-875d-bd8e30102289', now(), 'CountrySyncService', now(), 'CountrySyncService',431, false, 'AG', 'AG', 'AG', '', 'Antigua und Barbuda', 'Antigua-et-Barbuda', 'Antigua e Barbuda', '', 0, 'https://ld.admin.ch/country/ATG', 0),
                    ('794ab513-8f23-4524-93f5-a4e80332eefe', now(), 'CountrySyncService', now(), 'CountrySyncService',434, false, 'CU', 'CU', 'CU', '', 'Kuba', 'Cuba', 'Cuba', '', 0, 'https://ld.admin.ch/country/CUB', 0),
                    ('dece0ee2-a943-4ff3-aa78-8dbf3584acc9', now(), 'CountrySyncService', now(), 'CountrySyncService',437, false, 'HT', 'HT', 'HT', '', 'Haiti', 'Haïti', 'Haiti', '', 0, 'https://ld.admin.ch/country/HTI', 0),
                    ('fcc16844-7fe4-42cd-90db-85db2d400a0f', now(), 'CountrySyncService', now(), 'CountrySyncService',440, false, 'PE', 'PE', 'PE', '', 'Peru', 'Pérou', 'Perù', '', 0, 'https://ld.admin.ch/country/PER', 0),
                    ('f5d3c679-1190-431e-b830-11ed85ef19f7', now(), 'CountrySyncService', now(), 'CountrySyncService',321, false, 'MY', 'MY', 'MY', '', 'Malaysia', 'Malaisie', 'Malaysia', '', 0, 'https://ld.admin.ch/country/MYS', 0),
                    ('d74f5be7-7cd1-4e25-bba1-82b829aa10b5', now(), 'CountrySyncService', now(), 'CountrySyncService',324, false, 'FI', 'FI', 'FI', '', 'Finnland', 'Finlande', 'Finlandia', '', 0, 'https://ld.admin.ch/country/FIN', 0),
                    ('7cda6b84-5b3a-4c5d-bf4e-97dafac068bc', now(), 'CountrySyncService', now(), 'CountrySyncService',327, false, 'SI', 'SI', 'SI', '', 'Slowenien', 'Slovénie', 'Slovenia', '', 0, 'https://ld.admin.ch/country/SVN', 0),
                    ('a0067d34-ad5b-436a-8a21-09d24a930cb4', now(), 'CountrySyncService', now(), 'CountrySyncService',330, false, 'VN', 'VN', 'VN', '', 'Vietnam', 'ViÃªt Nam', 'Vietnam', '', 0, 'https://ld.admin.ch/country/VNM', 0),
                    ('5ff93162-bd20-411b-a9d8-01cda1cc8f90', now(), 'CountrySyncService', now(), 'CountrySyncService',333, false, 'ZA', 'ZA', 'ZA', '', 'Südafrika', 'Afrique du Sud', 'Sud Africa', '', 0, 'https://ld.admin.ch/country/ZAF', 0),
                    ('209c3e13-17b1-4ff7-807c-093283c5ace0', now(), 'CountrySyncService', now(), 'CountrySyncService',336, false, 'SA', 'SA', 'SA', '', 'Saudi-Arabien', 'Arabie saoudite', 'Arabia Saudita', '', 0, 'https://ld.admin.ch/country/SAU', 0),
                    ('5ca91e21-8895-4423-9119-2cf8af3e5415', now(), 'CountrySyncService', now(), 'CountrySyncService',339, false, 'PK', 'PK', 'PK', '', 'Pakistan', 'Pakistan', 'Pakistan', '', 0, 'https://ld.admin.ch/country/PAK', 0),
                    ('5ba442b9-b792-48f7-bb91-a0b54260813f', now(), 'CountrySyncService', now(), 'CountrySyncService',342, false, 'GQ', 'GQ', 'GQ', '', 'Äquatorialguinea', 'Guinée équatoriale', 'Guinea equatoriale', '', 0, 'https://ld.admin.ch/country/GNQ', 0),
                    ('28ddb86b-7a5e-49b9-a572-0882c06eac69', now(), 'CountrySyncService', now(), 'CountrySyncService',345, false, 'DZ', 'DZ', 'DZ', '', 'Algerien', 'Algérie', 'Algeria', '', 0, 'https://ld.admin.ch/country/DZA', 0),
                    ('79277542-dc5a-470a-a69b-df90c70cdfef', now(), 'CountrySyncService', now(), 'CountrySyncService',348, false, 'AR', 'AR', 'AR', '', 'Argentinien', 'Argentine', 'Argentina', '', 0, 'https://ld.admin.ch/country/ARG', 0),
                    ('03595e58-217e-4231-948c-a16d3ead0805', now(), 'CountrySyncService', now(), 'CountrySyncService',351, false, 'CL', 'CL', 'CL', '', 'Chile', 'Chili', 'Cile', '', 0, 'https://ld.admin.ch/country/CHL', 0),
                    ('da004231-ccdb-4d52-8c33-6f24e010fa8e', now(), 'CountrySyncService', now(), 'CountrySyncService',354, false, 'MU', 'MU', 'MU', '', 'Mauritius', 'Maurice', 'Maurizio', '', 0, 'https://ld.admin.ch/country/MUS', 0),
                    ('740fcfce-82df-4fa9-90e3-8242351ae14a', now(), 'CountrySyncService', now(), 'CountrySyncService',357, false, 'CI', 'CI', 'CI', '', 'Côte d’Ivoire', 'Côte d’Ivoire', 'Costa d’Avorio', '', 0, 'https://ld.admin.ch/country/CIV', 0),
                    ('75dad7d8-f0b5-4c5b-9f05-e849c9b6ad98', now(), 'CountrySyncService', now(), 'CountrySyncService',360, false, 'JM', 'JM', 'JM', '', 'Jamaika', 'Jamaïque', 'Giamaica', '', 0, 'https://ld.admin.ch/country/JAM', 0),
                    ('317da0f2-310c-4ee9-a95a-29dbcf4155fb', now(), 'CountrySyncService', now(), 'CountrySyncService',363, false, 'MW', 'MW', 'MW', '', 'Malawi', 'Malawi', 'Malawi', '', 0, 'https://ld.admin.ch/country/MWI', 0),
                    ('9ba65aaa-a194-4eee-8f62-737e88754c6f', now(), 'CountrySyncService', now(), 'CountrySyncService',366, false, 'TT', 'TT', 'TT', '', 'Trinidad und Tobago', 'Trinité-et-Tobago', 'Trinidad e Tobago', '', 0, 'https://ld.admin.ch/country/TTO', 0),
                    ('c64ae4f0-3e34-4656-ba02-f5048c2913b0', now(), 'CountrySyncService', now(), 'CountrySyncService',369, false, 'ZM', 'ZM', 'ZM', '', 'Sambia', 'Zambie', 'Zambia', '', 0, 'https://ld.admin.ch/country/ZMB', 0),
                    ('eff53be9-b4b8-4224-ad71-bf6d72f4c2f4', now(), 'CountrySyncService', now(), 'CountrySyncService',372, false, 'MR', 'MR', 'MR', '', 'Mauretanien', 'Mauritanie', 'Mauritania', '', 0, 'https://ld.admin.ch/country/MRT', 0),
                    ('48732750-a8aa-470e-8b73-978ad18def63', now(), 'CountrySyncService', now(), 'CountrySyncService',375, false, 'LY', 'LY', 'LY', '', 'Libyen', 'Libye', 'Libia', '', 0, 'https://ld.admin.ch/country/LBY', 0),
                    ('c62132ff-ec72-419a-bc11-604f0a183a3d', now(), 'CountrySyncService', now(), 'CountrySyncService',378, false, 'NO', 'NO', 'NO', '', 'Norwegen', 'Norvège', 'Norvegia', '', 0, 'https://ld.admin.ch/country/NOR', 0),
                    ('65d3beec-775c-4d20-b897-e50eae790d8a', now(), 'CountrySyncService', now(), 'CountrySyncService',381, false, 'ST', 'ST', 'ST', '', 'São Tomé und Príncipe', 'Sao Tomé-et-Principe', 'São Tomé e Príncipe', '', 0, 'https://ld.admin.ch/country/STP', 0),
                    ('d12d5028-dfcc-4279-949d-bcb7ef033402', now(), 'CountrySyncService', now(), 'CountrySyncService',384, false, 'GW', 'GW', 'GW', '', 'Guinea-Bissau', 'Guinée-Bissau', 'Guinea-Bissau', '', 0, 'https://ld.admin.ch/country/GNB', 0),
                    ('48535d17-780b-4fe4-9394-43761b4f7aae', now(), 'CountrySyncService', now(), 'CountrySyncService',387, false, 'US', 'US', 'US', '', 'Vereinigte Staaten', 'États-Unis', 'Stati Uniti', '', 0, 'https://ld.admin.ch/country/USA', 0),
                    ('bc3f34d3-e121-42e6-8924-26954e0ec7b2', now(), 'CountrySyncService', now(), 'CountrySyncService',390, false, 'CA', 'CA', 'CA', '', 'Kanada', 'Canada', 'Canada', '', 0, 'https://ld.admin.ch/country/CAN', 0),
                    ('49d70e19-83da-49c8-a181-229caf914de3', now(), 'CountrySyncService', now(), 'CountrySyncService',393, false, 'MD', 'MD', 'MD', '', 'Moldau', 'Moldavie', 'Moldavia', '', 0, 'https://ld.admin.ch/country/MDA', 0),
                    ('a765bed5-3192-48cb-bd9b-5c307007a4f3', now(), 'CountrySyncService', now(), 'CountrySyncService',396, false, 'TD', 'TD', 'TD', '', 'Tschad', 'Tchad', 'Ciad', '', 0, 'https://ld.admin.ch/country/TCD', 0),
                    ('7878254f-de8c-42d7-bbab-7840320fb0e4', now(), 'CountrySyncService', now(), 'CountrySyncService',399, false, 'ML', 'ML', 'ML', '', 'Mali', 'Mali', 'Mali', '', 0, 'https://ld.admin.ch/country/MLI', 0),
                    ('e0f7aaf7-f732-43e0-8cec-20be9fb81472', now(), 'CountrySyncService', now(), 'CountrySyncService',402, false, 'CO', 'CO', 'CO', '', 'Kolumbien', 'Colombie', 'Colombia', '', 0, 'https://ld.admin.ch/country/COL', 0),
                    ('5fd2e99e-d3b2-422e-a03c-2b3e5adeb459', now(), 'CountrySyncService', now(), 'CountrySyncService',405, false, 'TH', 'TH', 'TH', '', 'Thailand', 'Thaïlande', 'Thailandia', '', 0, 'https://ld.admin.ch/country/THA', 0),
                    ('b328a6bd-f3d5-4d2d-924e-9d427f3dd4c5', now(), 'CountrySyncService', now(), 'CountrySyncService',408, false, 'CV', 'CV', 'CV', '', 'Cabo Verde', 'Cabo Verde', 'Cabo Verde', '', 0, 'https://ld.admin.ch/country/CPV', 0),
                    ('b6a10132-e4ed-4622-a471-2465f27ba8e2', now(), 'CountrySyncService', now(), 'CountrySyncService',411, false, 'MZ', 'MZ', 'MZ', '', 'Mosambik', 'Mozambique', 'Mozambico', '', 0, 'https://ld.admin.ch/country/MOZ', 0),
                    ('7e498e72-e660-4706-ad12-a82f01b7b0e4', now(), 'CountrySyncService', now(), 'CountrySyncService',414, false, 'BY', 'BY', 'BY', '', 'Belarus', 'Biélorussie', 'Bielorussia', '', 0, 'https://ld.admin.ch/country/BLR', 0),
                    ('2432d7a2-2e57-44d4-8bb5-cc4aa299f9ac', now(), 'CountrySyncService', now(), 'CountrySyncService',417, false, 'BS', 'BS', 'BS', '', 'Bahamas', 'Bahamas', 'Bahamas', '', 0, 'https://ld.admin.ch/country/BHS', 0),
                    ('bde50ca9-1575-47a1-ab1b-88d4496f8c70', now(), 'CountrySyncService', now(), 'CountrySyncService',423, false, 'KZ', 'KZ', 'KZ', '', 'Kasachstan', 'Kazakhstan', 'Kazakstan', '', 0, 'https://ld.admin.ch/country/KAZ', 0),
                    ('871c3197-bee8-4032-9cb8-f673676ed24d', now(), 'CountrySyncService', now(), 'CountrySyncService',426, false, 'CM', 'CM', 'CM', '', 'Kamerun', 'Cameroun', 'Camerun', '', 0, 'https://ld.admin.ch/country/CMR', 0),
                    ('f08811e3-1946-4b20-ac21-da5e0807542f', now(), 'CountrySyncService', now(), 'CountrySyncService',429, false, 'GE', 'GE', 'GE', '', 'Georgien', 'Géorgie', 'Georgia', '', 0, 'https://ld.admin.ch/country/GEO', 0),
                    ('a9ea3651-5667-4fec-b661-0bbe37df9ba6', now(), 'CountrySyncService', now(), 'CountrySyncService',432, false, 'BO', 'BO', 'BO', '', 'Bolivien', 'Bolivie', 'Bolivia', '', 0, 'https://ld.admin.ch/country/BOL', 0),
                    ('1b305f2f-f0f7-4b6f-a54f-326b6f67aeb2', now(), 'CountrySyncService', now(), 'CountrySyncService',435, false, 'EC', 'EC', 'EC', '', 'Ecuador', 'Équateur', 'Ecuador', '', 0, 'https://ld.admin.ch/country/ECU', 0),
                    ('7ddd1d68-58aa-4f6a-aac9-674851b6354b', now(), 'CountrySyncService', now(), 'CountrySyncService',438, false, 'LC', 'LC', 'LC', '', 'St. Lucia', 'Sainte-Lucie', 'Saint Lucia', '', 0, 'https://ld.admin.ch/country/LCA', 0),
                    ('64e0e609-196c-470b-8501-726d19505cbd', now(), 'CountrySyncService', now(), 'CountrySyncService',441, false, 'PY', 'PY', 'PY', '', 'Paraguay', 'Paraguay', 'Paraguay', '', 0, 'https://ld.admin.ch/country/PRY', 0),
                    ('44d4643a-f439-4c6c-8acc-29de4dc09df7', now(), 'CountrySyncService', now(), 'CountrySyncService',444, false, 'VE', 'VE', 'VE', '', 'Venezuela', 'Venezuela', 'Venezuela', '', 0, 'https://ld.admin.ch/country/VEN', 0),
                    ('36457776-e40c-41eb-ad4c-e5ad183f04ae', now(), 'CountrySyncService', now(), 'CountrySyncService',447, false, 'HK', 'HK', 'HK', '', 'Hongkong', 'Hong Kong', 'Hong Kong', '', 0, 'https://ld.admin.ch/country/HKG', 0),
                    ('ba664dd9-67e0-4be4-9e10-eb4c4aa66ccd', now(), 'CountrySyncService', now(), 'CountrySyncService',450, false, 'RS', 'RS', 'RS', '', 'Serbien', 'Serbie', 'Serbia', '', 0, 'https://ld.admin.ch/country/SRB', 0),
                    ('84631f2a-dd3d-40ca-be09-f7dc7728a3b0', now(), 'CountrySyncService', now(), 'CountrySyncService',453, false, 'GA', 'GA', 'GA', '', 'Gabun', 'Gabon', 'Gabon', '', 0, 'https://ld.admin.ch/country/GAB', 0),
                    ('f86261f2-cf12-4326-b45e-3ffe1f5278f1', now(), 'CountrySyncService', now(), 'CountrySyncService',456, false, 'PH', 'PH', 'PH', '', 'Philippinen', 'Philippines', 'Filippine', '', 0, 'https://ld.admin.ch/country/PHL', 0),
                    ('70629a55-7bab-43c5-aa8a-95f2ad388dc4', now(), 'CountrySyncService', now(), 'CountrySyncService',459, false, 'RW', 'RW', 'RW', '', 'Ruanda', 'Rwanda', 'Ruanda', '', 0, 'https://ld.admin.ch/country/RWA', 0),
                    ('4ff74a11-97de-428d-a8c0-89fea9141ad0', now(), 'CountrySyncService', now(), 'CountrySyncService',462, false, 'SD', 'SD', 'SD', '', 'Sudan', 'Soudan', 'Sudan', '', 0, 'https://ld.admin.ch/country/SDN', 0),
                    ('88d13b7b-cdde-418b-a8f0-64e5904762e4', now(), 'CountrySyncService', now(), 'CountrySyncService',465, false, 'BW', 'BW', 'BW', '', 'Botsuana', 'Botswana', 'Botswana', '', 0, 'https://ld.admin.ch/country/BWA', 0),
                    ('6c252bca-cd4d-44aa-b7fc-b152775acc0e', now(), 'CountrySyncService', now(), 'CountrySyncService',468, false, 'VA', 'VA', 'VA', '', 'Heiliger Stuhl', 'Saint-Siège', 'Santa Sede', '', 0, 'https://ld.admin.ch/country/VAT', 0),
                    ('ba7f54a4-ffd7-4d31-ab79-52711b3cd84e', now(), 'CountrySyncService', now(), 'CountrySyncService',471, false, 'DJ', 'DJ', 'DJ', '', 'Dschibuti', 'Djibouti', 'Gibuti', '', 0, 'https://ld.admin.ch/country/DJI', 0),
                    ('bb4d2500-7c22-4a2a-b20f-b1e075963337', now(), 'CountrySyncService', now(), 'CountrySyncService',474, false, 'TO', 'TO', 'TO', '', 'Tonga', 'Tonga', 'Tonga', '', 0, 'https://ld.admin.ch/country/TON', 0),
                    ('ed54cbff-f0e0-489a-bd1f-e0506fa40869', now(), 'CountrySyncService', now(), 'CountrySyncService',477, false, 'AQ', 'AQ', 'AQ', '', 'Antarktis', 'Antarctique', 'Antartide', '', 0, 'https://ld.admin.ch/country/ATA', 0),
                    ('29ecf2d2-8c3a-4547-8057-6e5ff8be409f', now(), 'CountrySyncService', now(), 'CountrySyncService',480, false, 'ER', 'ER', 'ER', '', 'Eritrea', 'Érythrée', 'Eritrea', '', 0, 'https://ld.admin.ch/country/ERI', 0),
                    ('f2cf626c-d377-4d94-94be-8688be83d6a1', now(), 'CountrySyncService', now(), 'CountrySyncService',483, false, 'GG', 'GG', 'GG', '', 'Guernsey', 'Guernesey', 'Guernsey', '', 0, 'https://ld.admin.ch/country/GGY', 0),
                    ('4573c729-4d2e-4a05-906a-3aa45443a593', now(), 'CountrySyncService', now(), 'CountrySyncService',486, false, 'NR', 'NR', 'NR', '', 'Nauru', 'Nauru', 'Nauru', '', 0, 'https://ld.admin.ch/country/NRU', 0),
                    ('d4df882c-277b-4856-b868-bc906e633179', now(), 'CountrySyncService', now(), 'CountrySyncService',489, false, 'GL', 'GL', 'GL', '', 'Grönland', 'Groenland', 'Groenlandia', '', 0, 'https://ld.admin.ch/country/GRL', 0),
                    ('2096e910-7f00-4418-83c7-3d2098b5257a', now(), 'CountrySyncService', now(), 'CountrySyncService',492, false, 'GI', 'GI', 'GI', '', 'Gibraltar', 'Gibraltar', 'Gibraltar', '', 0, 'https://ld.admin.ch/country/GIB', 0),
                    ('a348d298-788f-4cbe-976f-d8f938befe0b', now(), 'CountrySyncService', now(), 'CountrySyncService',495, false, 'CW', 'CW', 'CW', '', 'Curaçao', 'Curaçao', 'Curaçao', '', 0, 'https://ld.admin.ch/country/CUW', 0),
                    ('23874f8a-4107-41ce-843d-665dfc55175f', now(), 'CountrySyncService', now(), 'CountrySyncService',498, false, 'CK', 'CK', 'CK', '', 'Cookinseln', 'Îles Cook', 'Isole Cook', '', 0, 'https://ld.admin.ch/country/COK', 0),
                    ('19f13ad8-3983-4be4-b9a5-ee06c0296400', now(), 'CountrySyncService', now(), 'CountrySyncService',501, false, 'AS', 'AS', 'AS', '', 'Amerikanisch-Samoa', 'Samoa américaines', 'Samoa americane', '', 0, 'https://ld.admin.ch/country/ASM', 0),
                    ('5af0363a-afdc-4fa2-9150-ddff27eefafa', now(), 'CountrySyncService', now(), 'CountrySyncService',504, false, 'BV', 'BV', 'BV', '', 'Bouvetinsel', 'Île Bouvet', 'Isola di Bouvet', '', 0, 'https://ld.admin.ch/country/BVT', 0),
                    ('9ae2c550-4423-47c6-93dc-5dd4df550e3d', now(), 'CountrySyncService', now(), 'CountrySyncService',507, false, 'CX', 'CX', 'CX', '', 'Weihnachtsinsel', 'Île Christmas', 'Isola Christmas', '', 0, 'https://ld.admin.ch/country/CXR', 0),
                    ('e04f020f-e02a-45c8-b247-288e9b54f38d', now(), 'CountrySyncService', now(), 'CountrySyncService',510, false, 'GF', 'GF', 'GF', '', 'Französisch-Guayana', 'Guyane', 'Guyana francese', '', 0, 'https://ld.admin.ch/country/GUF', 0),
                    ('2b24d068-1f1a-4dd2-b741-3f7bbe9aef08', now(), 'CountrySyncService', now(), 'CountrySyncService',513, false, 'IO', 'IO', 'IO', '', 'Britisches Territorium im Indischen Ozean', 'Territoire britannique de l’océan Indien', 'Territorio britannico dell’oceano Indiano', '', 0, 'https://ld.admin.ch/country/IOT', 0),
                    ('0b37f2a4-6711-4c26-85ac-8b850a90b989', now(), 'CountrySyncService', now(), 'CountrySyncService',516, false, 'MS', 'MS', 'MS', '', 'Montserrat', 'Montserrat', 'Montserrat', '', 0, 'https://ld.admin.ch/country/MSR', 0),
                    ('ec904f22-354c-4379-a5b5-cedac99bd881', now(), 'CountrySyncService', now(), 'CountrySyncService',519, false, 'NC', 'NC', 'NC', '', 'Neukaledonien', 'Nouvelle-Calédonie', 'Nuova Caledonia', '', 0, 'https://ld.admin.ch/country/NCL', 0),
                    ('1d01fa85-0f7f-49fb-ab60-a97941aeb07e', now(), 'CountrySyncService', now(), 'CountrySyncService',522, false, 'PN', 'PN', 'PN', '', 'Pitcairninseln', 'Îles Pitcairn', 'Isole Pitcairn', '', 0, 'https://ld.admin.ch/country/PCN', 0),
                    ('9b27ca05-b203-4459-8bec-efb83a68b927', now(), 'CountrySyncService', now(), 'CountrySyncService',525, false, 'RE', 'RE', 'RE', '', 'Réunion', 'La Réunion', 'Riunione', '', 0, 'https://ld.admin.ch/country/REU', 0),
                    ('47eec30c-72a4-472e-85d6-083e3bfcaad3', now(), 'CountrySyncService', now(), 'CountrySyncService',528, false, 'SJ', 'SJ', 'SJ', '', 'Svalbard und Jan Mayen', 'Svalbard et Jan Mayen', 'Svalbard e Jan Mayen', '', 0, 'https://ld.admin.ch/country/SJM', 0),
                    ('949aa23a-bdc7-4f47-a975-2d73cc2b85cc', now(), 'CountrySyncService', now(), 'CountrySyncService',531, false, 'TK', 'TK', 'TK', '', 'Tokelau', 'Tokélaou', 'Tokelau', '', 0, 'https://ld.admin.ch/country/TKL', 0),
                    ('0afa1776-d1a8-4950-ab27-cce7adcc5dda', now(), 'CountrySyncService', now(), 'CountrySyncService',534, false, 'VI', 'VI', 'VI', '', 'Amerikanische Jungferninseln', 'Îles Vierges américaines', 'Isole Vergini americane', '', 0, 'https://ld.admin.ch/country/VIR', 0),
                    ('8cc95e62-9b8f-483b-bf35-e71eaae7111e', now(), 'CountrySyncService', now(), 'CountrySyncService',442, false, 'SV', 'SV', 'SV', '', 'El Salvador', 'El Salvador', 'El Salvador', '', 0, 'https://ld.admin.ch/country/SLV', 0),
                    ('d6822fd4-2465-4dbd-bfc2-db1557d64641', now(), 'CountrySyncService', now(), 'CountrySyncService',445, false, 'SO', 'SO', 'SO', '', 'Somalia', 'Somalie', 'Somalia', '', 0, 'https://ld.admin.ch/country/SOM', 0),
                    ('4ec1e1ce-2175-48f8-8559-d53ecebd0d92', now(), 'CountrySyncService', now(), 'CountrySyncService',448, false, 'PW', 'PW', 'PW', '', 'Palau', 'Palaos', 'Palau', '', 0, 'https://ld.admin.ch/country/PLW', 0),
                    ('bb8b4873-2d8a-47fa-9f4d-12dc7ebbac1a', now(), 'CountrySyncService', now(), 'CountrySyncService',451, false, 'ME', 'ME', 'ME', '', 'Montenegro', 'Monténégro', 'Montenegro', '', 0, 'https://ld.admin.ch/country/MNE', 0),
                    ('889fab87-c6f2-4c61-a2f9-f650d8e5b04a', now(), 'CountrySyncService', now(), 'CountrySyncService',454, false, 'HN', 'HN', 'HN', '', 'Honduras', 'Honduras', 'Honduras', '', 0, 'https://ld.admin.ch/country/HND', 0),
                    ('fac68917-4f64-4638-af02-c658316248eb', now(), 'CountrySyncService', now(), 'CountrySyncService',457, false, 'MV', 'MV', 'MV', '', 'Malediven', 'Maldives', 'Maldive', '', 0, 'https://ld.admin.ch/country/MDV', 0),
                    ('297918b3-6c0d-4507-a6f2-4d840929d29e', now(), 'CountrySyncService', now(), 'CountrySyncService',460, false, 'AZ', 'AZ', 'AZ', '', 'Aserbaidschan', 'Azerbaïdjan', 'Azerbaigian', '', 0, 'https://ld.admin.ch/country/AZE', 0),
                    ('d1f31a0e-0ff6-4e74-be2d-a3c44cf3b6da', now(), 'CountrySyncService', now(), 'CountrySyncService',463, false, 'GH', 'GH', 'GH', '', 'Ghana', 'Ghana', 'Ghana', '', 0, 'https://ld.admin.ch/country/GHA', 0),
                    ('2b2b48c4-4e33-41d1-9826-44daa4c24b51', now(), 'CountrySyncService', now(), 'CountrySyncService',466, false, 'LS', 'LS', 'LS', '', 'Lesotho', 'Lesotho', 'Lesotho', '', 0, 'https://ld.admin.ch/country/LSO', 0),
                    ('d30b509e-bbdb-4851-be90-84f4314044a8', now(), 'CountrySyncService', now(), 'CountrySyncService',469, false, 'IQ', 'IQ', 'IQ', '', 'Irak', 'Iraq', 'Iraq', '', 0, 'https://ld.admin.ch/country/IRQ', 0),
                    ('763aa525-e845-4d6d-8a96-44c6c2b672c5', now(), 'CountrySyncService', now(), 'CountrySyncService',472, false, 'KH', 'KH', 'KH', '', 'Kambodscha', 'Cambodge', 'Cambogia', '', 0, 'https://ld.admin.ch/country/KHM', 0),
                    ('428fb6a2-9983-49e5-a03e-0c7eb5c4346e', now(), 'CountrySyncService', now(), 'CountrySyncService',475, false, 'PS', 'PS', 'PS', '', 'Palästina*', 'Palestine*', 'Palestina*', '', 0, 'https://ld.admin.ch/country/PSE', 0),
                    ('e019146c-1a93-48de-a8c9-56fd2d47c6c8', now(), 'CountrySyncService', now(), 'CountrySyncService',478, false, 'BT', 'BT', 'BT', '', 'Bhutan', 'Bhoutan', 'Bhutan', '', 0, 'https://ld.admin.ch/country/BTN', 0),
                    ('a9649914-cd4d-4fdc-a548-2676ede87048', now(), 'CountrySyncService', now(), 'CountrySyncService',481, false, 'EH', 'EH', 'EH', '', 'Westsahara', 'Sahara occidental', 'Sahara occidentale', '', 0, 'https://ld.admin.ch/country/ESH', 0),
                    ('8766a245-6606-4b62-932e-acbc8592116b', now(), 'CountrySyncService', now(), 'CountrySyncService',484, false, 'IM', 'IM', 'IM', '', 'Insel Man', 'Île de Man', 'Isola di Man', '', 0, 'https://ld.admin.ch/country/IMN', 0),
                    ('33a670bb-26d4-4c89-87b5-dd15867695d3', now(), 'CountrySyncService', now(), 'CountrySyncService',487, false, 'KP', 'KP', 'KP', '', 'Nordkorea', 'Corée du Nord', 'Corea del Nord', '', 0, 'https://ld.admin.ch/country/PRK', 0),
                    ('4b184b5e-8e26-42d5-a259-6b8b72fa6b8a', now(), 'CountrySyncService', now(), 'CountrySyncService',490, false, 'KY', 'KY', 'KY', '', 'Kaimaninseln', 'Îles Caïmans', 'Isole Cayman', '', 0, 'https://ld.admin.ch/country/CYM', 0),
                    ('7cf06e53-bae7-4121-81a3-e3c52df3024c', now(), 'CountrySyncService', now(), 'CountrySyncService',493, false, 'AW', 'AW', 'AW', '', 'Aruba', 'Aruba', 'Aruba', '', 0, 'https://ld.admin.ch/country/ABW', 0),
                    ('e75d2817-aeec-408a-a602-79ad66fed0a9', now(), 'CountrySyncService', now(), 'CountrySyncService',496, false, 'SX', 'SX', 'SX', '', 'Sint Maarten', 'Sint-Maarten', 'Sint Maarten', '', 0, 'https://ld.admin.ch/country/SXM', 0),
                    ('dcebf408-0db5-4391-ba1d-5965a72b8ace', now(), 'CountrySyncService', now(), 'CountrySyncService',499, false, 'AI', 'AI', 'AI', '', 'Anguilla', 'Anguilla', 'Anguilla', '', 0, 'https://ld.admin.ch/country/AIA', 0),
                    ('af37afc5-5048-4124-bed3-4e8158dc297f', now(), 'CountrySyncService', now(), 'CountrySyncService',502, false, 'TF', 'TF', 'TF', '', 'Französische Süd- und Antarktisgebiete', 'Terres australes et antarctiques françaises', 'Terre australi e antartiche francesi', '', 0, 'https://ld.admin.ch/country/ATF', 0),
                    ('fedbbc8d-f9b3-478b-acb0-cb0fc84e3bd3', now(), 'CountrySyncService', now(), 'CountrySyncService',505, false, 'CC', 'CC', 'CC', '', 'Kokosinseln', 'Îles des Cocos', 'Isole Cocos', '', 0, 'https://ld.admin.ch/country/CCK', 0),
                    ('1d6846ce-6d9b-4fee-ac2e-fdf930c87b04', now(), 'CountrySyncService', now(), 'CountrySyncService',508, false, 'FK', 'FK', 'FK', '', 'Falklandinseln', 'Îles Falkland', 'Isole Falkland', '', 0, 'https://ld.admin.ch/country/FLK', 0),
                    ('3b42e61b-641b-4f20-aa1e-e586a1fe464c', now(), 'CountrySyncService', now(), 'CountrySyncService',511, false, 'GU', 'GU', 'GU', '', 'Guam', 'Guam', 'Guam', '', 0, 'https://ld.admin.ch/country/GUM', 0),
                    ('8854517b-e991-4fc2-b735-5b512d69ac42', now(), 'CountrySyncService', now(), 'CountrySyncService',514, false, 'MF', 'MF', 'MF', '', 'St. Martin', 'Saint-Martin', 'Saint-Martin', '', 0, 'https://ld.admin.ch/country/MAF', 0),
                    ('ea96da17-cc77-4234-8fe0-bb54223a6409', now(), 'CountrySyncService', now(), 'CountrySyncService',517, false, 'MQ', 'MQ', 'MQ', '', 'Martinique', 'Martinique', 'Martinica', '', 0, 'https://ld.admin.ch/country/MTQ', 0),
                    ('baba6490-6744-4391-8f0c-b7d57a6a37aa', now(), 'CountrySyncService', now(), 'CountrySyncService',520, false, 'NF', 'NF', 'NF', '', 'Norfolkinsel', 'Île Norfolk', 'Isola Norfolk', '', 0, 'https://ld.admin.ch/country/NFK', 0),
                    ('fcc2fba8-9284-45ab-89d4-fb9183a909e5', now(), 'CountrySyncService', now(), 'CountrySyncService',523, false, 'PR', 'PR', 'PR', '', 'Puerto Rico', 'Porto Rico', 'Portorico', '', 0, 'https://ld.admin.ch/country/PRI', 0),
                    ('215df4e8-1b98-4571-963f-e26ef4c510c4', now(), 'CountrySyncService', now(), 'CountrySyncService',526, false, 'GS', 'GS', 'GS', '', 'Südgeorgien und die Südlichen Sandwichinseln', 'Îles Géorgie du Sud et Sandwich du Sud', 'Isole Georgia del sud e Sandwich del sud', '', 0, 'https://ld.admin.ch/country/SGS', 0),
                    ('40375e40-7949-4d7a-8042-0e10f60a4862', now(), 'CountrySyncService', now(), 'CountrySyncService',529, false, 'PM', 'PM', 'PM', '', 'St. Pierre und Miquelon', 'Saint-Pierre-et-Miquelon', 'Saint Pierre e Miquelon', '', 0, 'https://ld.admin.ch/country/SPM', 0),
                    ('5c75e3c3-f843-4417-bfef-83a9b838a876', now(), 'CountrySyncService', now(), 'CountrySyncService',532, false, 'Kleinere Amerikanischen Überseeinseln', 'Îles mineures éloignées des États-Unis', 'Isole minori periferiche degli Stati Uniti', '', 'Kleinere Amerikanischen Überseeinseln', 'Îles mineures éloignées des États-Unis', 'Isole minori periferiche degli Stati Uniti', '', 0, 'https://ld.admin.ch/country/UMI', 0),
                    ('066eac5f-469a-42da-bc5b-50fa3045f621', now(), 'CountrySyncService', now(), 'CountrySyncService',535, false, 'WF', 'WF', 'WF', '', 'Wallis und Futuna', 'Wallis-et-Futuna', 'Wallis e Futuna', '', 0, 'https://ld.admin.ch/country/WLF', 0),
                    ('589db8f7-289e-4bff-bce0-ed7e2642a0b1', now(), 'CountrySyncService', now(), 'CountrySyncService',443, false, 'VC', 'VC', 'VC', '', 'St. Vincent und die Grenadinen', 'Saint-Vincent-et-les-Grenadines', 'Saint Vincent e Grenadine', '', 0, 'https://ld.admin.ch/country/VCT', 0),
                    ('9e5f6115-e510-4fc5-a79c-95832b6d5e0f', now(), 'CountrySyncService', now(), 'CountrySyncService',446, false, 'FJ', 'FJ', 'FJ', '', 'Fidschi', 'Fidji', 'Figi', '', 0, 'https://ld.admin.ch/country/FJI', 0),
                    ('6b266c40-97ab-4d29-8e4e-555c1fa77c57', now(), 'CountrySyncService', now(), 'CountrySyncService',449, false, 'MN', 'MN', 'MN', '', 'Mongolei', 'Mongolie', 'Mongolia', '', 0, 'https://ld.admin.ch/country/MNG', 0),
                    ('21e970e3-d810-4e1f-bd65-8cc85bf48358', now(), 'CountrySyncService', now(), 'CountrySyncService',452, false, 'SY', 'SY', 'SY', '', 'Syrien', 'Syrie', 'Siria', '', 0, 'https://ld.admin.ch/country/SYR', 0),
                    ('50b15a38-0512-4339-8d26-72884273f06b', now(), 'CountrySyncService', now(), 'CountrySyncService',455, false, 'LR', 'LR', 'LR', '', 'Liberia', 'Liberia', 'Liberia', '', 0, 'https://ld.admin.ch/country/LBR', 0),
                    ('77b17239-7423-4557-9fa3-d38ebe5e3ef6', now(), 'CountrySyncService', now(), 'CountrySyncService',458, false, 'MO', 'MO', 'MO', '', 'Macau', 'Macao', 'Macao', '', 0, 'https://ld.admin.ch/country/MAC', 0),
                    ('85ea6ccd-b31e-4c35-a4f4-2cf30fc346a4', now(), 'CountrySyncService', now(), 'CountrySyncService',461, false, 'KI', 'KI', 'KI', '', 'Kiribati', 'Kiribati', 'Kiribati', '', 0, 'https://ld.admin.ch/country/KIR', 0),
                    ('9f221d51-4ba7-4f8d-82cd-c991d3a6869e', now(), 'CountrySyncService', now(), 'CountrySyncService',464, false, 'YE', 'YE', 'YE', '', 'Jemen', 'Yémen', 'Yemen', '', 0, 'https://ld.admin.ch/country/YEM', 0),
                    ('5dcf14b7-f15c-4632-9c4b-e1f41c8fbe36', now(), 'CountrySyncService', now(), 'CountrySyncService',467, false, 'NA', 'NA', 'NA', '', 'Namibia', 'Namibie', 'Namibia', '', 0, 'https://ld.admin.ch/country/NAM', 0),
                    ('063d8086-f390-42a6-a5fd-630e85719ddc', now(), 'CountrySyncService', now(), 'CountrySyncService',470, false, 'NG', 'NG', 'NG', '', 'Nigeria', 'Nigeria', 'Nigeria', '', 0, 'https://ld.admin.ch/country/NGA', 0),
                    ('6fbf3c93-3ee2-4b6b-bef7-9f213c71124d', now(), 'CountrySyncService', now(), 'CountrySyncService',473, false, 'TL', 'TL', 'TL', '', 'Timor-Leste', 'Timor-Oriental', 'Timor orientale', '', 0, 'https://ld.admin.ch/country/TLS', 0),
                    ('e479f19b-1f89-42a5-a25c-791d0f47fa3d', now(), 'CountrySyncService', now(), 'CountrySyncService',476, false, 'MH', 'MH', 'MH', '', 'Marshallinseln', 'Îles Marshall', 'Isole Marshall', '', 0, 'https://ld.admin.ch/country/MHL', 0),
                    ('bb0402c3-e4f3-478b-9ce4-ec6b9c6f17dd', now(), 'CountrySyncService', now(), 'CountrySyncService',479, false, 'MM', 'MM', 'MM', '', 'Myanmar/Birma', 'Myanmar/Birmanie', 'Myanmar/Birmania', '', 0, 'https://ld.admin.ch/country/MMR', 0),
                    ('80516019-cc34-4819-bc93-10381bd0392e', now(), 'CountrySyncService', now(), 'CountrySyncService',482, false, 'ET', 'ET', 'ET', '', 'Äthiopien', 'Éthiopie', 'Etiopia', '', 0, 'https://ld.admin.ch/country/ETH', 0),
                    ('5bca540f-efd3-41aa-aa7d-907225de16f9', now(), 'CountrySyncService', now(), 'CountrySyncService',485, false, 'JE', 'JE', 'JE', '', 'Jersey', 'Jersey', 'Jersey', '', 0, 'https://ld.admin.ch/country/JEY', 0),
                    ('9785824a-a6a6-4474-a521-b295d99fc98f', now(), 'CountrySyncService', now(), 'CountrySyncService',488, false, 'SS', 'SS', 'SS', '', 'Südsudan', 'Soudan du Sud', 'Sud Sudan', '', 0, 'https://ld.admin.ch/country/SSD', 0),
                    ('6d5bc543-b212-4641-93b2-e88cb2610299', now(), 'CountrySyncService', now(), 'CountrySyncService',491, false, 'BM', 'BM', 'BM', '', 'Bermuda', 'Bermudes', 'Bermuda', '', 0, 'https://ld.admin.ch/country/BMU', 0),
                    ('eb9c61a3-623c-4ed2-afbc-0b81fe4b73a5', now(), 'CountrySyncService', now(), 'CountrySyncService',494, false, 'BQ', 'BQ', 'BQ', '', 'Bonaire, Sint Eustatius und Saba', 'Bonaire, Saint-Eustache et Saba', 'Bonaire, Sant’Eustachio e Saba', '', 0, 'https://ld.admin.ch/country/BES', 0),
                    ('0df86b6e-5013-4c14-abe7-0e2187250b41', now(), 'CountrySyncService', now(), 'CountrySyncService',497, false, 'FO', 'FO', 'FO', '', 'Färöer', 'Îles Féroé', 'Isole Fær Øer', '', 0, 'https://ld.admin.ch/country/FRO', 0),
                    ('80df1365-894d-4a13-b2c4-ef74da092720', now(), 'CountrySyncService', now(), 'CountrySyncService',500, false, 'AX', 'AX', 'AX', '', 'Ålandinseln', 'Îles Åland', 'Isole Åland', '', 0, 'https://ld.admin.ch/country/ALA', 0),
                    ('ba1dbf72-dab4-4bee-b55e-f76818a712fa', now(), 'CountrySyncService', now(), 'CountrySyncService',503, false, 'BL', 'BL', 'BL', '', 'St. Barthélemy', 'Saint-Barthélemy', 'Saint-Barthélemy', '', 0, 'https://ld.admin.ch/country/BLM', 0),
                    ('28f429f8-4871-4f84-98ec-88807f748137', now(), 'CountrySyncService', now(), 'CountrySyncService',506, false, 'CP', 'CP', 'CP', '', 'Clipperton', 'Clipperton', 'Clipperton', '', 0, 'https://ld.admin.ch/country/CPT', 0),
                    ('cbf02c5a-75d1-4f07-af17-15a54e5e642a', now(), 'CountrySyncService', now(), 'CountrySyncService',509, false, 'GP', 'GP', 'GP', '', 'Guadeloupe', 'Guadeloupe', 'Guadalupa', '', 0, 'https://ld.admin.ch/country/GLP', 0),
                    ('86b80ead-83e0-452e-b086-56ce63000d10', now(), 'CountrySyncService', now(), 'CountrySyncService',512, false, 'HM', 'HM', 'HM', '', 'Heard und die McDonaldinseln', 'Îles Heard et McDonald', 'Isole Heard e McDonald', '', 0, 'https://ld.admin.ch/country/HMD', 0),
                    ('d08f1f10-3d2c-4587-84d2-2d28ba49be58', now(), 'CountrySyncService', now(), 'CountrySyncService',515, false, 'MP', 'MP', 'MP', '', 'Nördliche Marianen', 'Îles Mariannes du Nord', 'Isole Marianne settentrionali', '', 0, 'https://ld.admin.ch/country/MNP', 0),
                    ('36026d0f-b95d-4665-afc0-6d10f2b46f82', now(), 'CountrySyncService', now(), 'CountrySyncService',518, false, 'YT', 'YT', 'YT', '', 'Mayotte', 'Mayotte', 'Mayotte', '', 0, 'https://ld.admin.ch/country/MYT', 0),
                    ('b8764c2c-368a-4204-b85a-809328b8da01', now(), 'CountrySyncService', now(), 'CountrySyncService',521, false, 'NU', 'NU', 'NU', '', 'Niue', 'Niué', 'Niue', '', 0, 'https://ld.admin.ch/country/NIU', 0),
                    ('17a65994-2def-4477-892c-42ccfa02624c', now(), 'CountrySyncService', now(), 'CountrySyncService',524, false, 'PF', 'PF', 'PF', '', 'Französisch-Polynesien', 'Polynésie française', 'Polinesia francese', '', 0, 'https://ld.admin.ch/country/PYF', 0),
                    ('c1cde044-0f11-41d3-a94e-25ac3038b89c', now(), 'CountrySyncService', now(), 'CountrySyncService',527, false, 'SH', 'SH', 'SH', '', 'St. Helena, Ascension und Tristan da Cunha', 'Sainte-Hélène, Ascension et Tristan da Cunha', 'Sant’Elena, Ascensione e Tristan da Cunha', '', 0, 'https://ld.admin.ch/country/SHN', 0),
                    ('fe200143-7799-4bf9-bf56-9e8065f4467d', now(), 'CountrySyncService', now(), 'CountrySyncService',530, false, 'TC', 'TC', 'TC', '', 'Turks- und Caicosinseln', 'Îles Turks-et-Caïcos', 'Isole Turks e Caicos', '', 0, 'https://ld.admin.ch/country/TCA', 0),
                    ('0cb727c0-6b8a-4010-8894-85ce047a467c', now(), 'CountrySyncService', now(), 'CountrySyncService',533, false, 'VG', 'VG', 'VG', '', 'Britische Jungferninseln', 'Îles Vierges britanniques', 'Isole Vergini britanniche', '', 0, 'https://ld.admin.ch/country/VGB', 0),
                    ('fd7a8fe6-ef1a-460f-962c-7830806430de', now(), 'CountrySyncService', now(), 'CountrySyncService',536, false, '1A', '1A', '1A', '', 'Kosovo*', 'Kosovo*', 'Kosovo*', '', 0, 'https://ld.admin.ch/country/XKX', 0)
            ");

            migrationBuilder.Sql(@$"
                update data.addresses set country_id = (select id from data.countries where description_de = 'Schweiz') where country_code in ('CH');
                update data.addresses set country_id = (select id from data.countries where description_de = 'Österreich') where country_code in ('A', 'A-');
                update data.addresses set country_id = (select id from data.countries where description_de = 'Belgien') where country_code in ('B-');
                update data.addresses set country_id = (select id from data.countries where description_de = 'Deutschland') where country_code in ('D', 'D-', 'DE');
                update data.addresses set country_id = (select id from data.countries where description_de = 'Spanien') where country_code in ('E', 'E-', 'ES');
                update data.addresses set country_id = (select id from data.countries where description_de = 'Frankreich') where country_code in ('F', 'F-', 'FR');
                update data.addresses set country_id = (select id from data.countries where description_de = 'Liechtenstein') where country_code in ('FL');
                update data.addresses set country_id = (select id from data.countries where description_de = 'Vereinigtes Königreich') where country_code in ('GB');
                update data.addresses set country_id = (select id from data.countries where description_de = 'Hongkong') where country_code in ('HK');
                update data.addresses set country_id = (select id from data.countries where description_de = 'Italien') where country_code in ('I-', 'IT');
                update data.addresses set country_id = (select id from data.countries where description_de = 'Luxemburg') where country_code in ('LU');
                update data.addresses set country_id = (select id from data.countries where description_de = 'Mexiko') where country_code in ('MX');
                update data.addresses set country_id = (select id from data.countries where description_de = 'Niederlande') where country_code in ('NL');
                update data.addresses set country_id = (select id from data.countries where description_de = 'Kanada') where country_code in ('Ca');
                update data.addresses set country_id = (select id from data.countries where description_de = 'Vereinigte Staaten') where country_code in ('US', 'USA');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_addresses_countries_country_id",
                schema: "data",
                table: "addresses");

            migrationBuilder.DropTable(
                name: "countries",
                schema: "data");

            migrationBuilder.DropIndex(
                name: "ix_addresses_country_id",
                schema: "data",
                table: "addresses");

            migrationBuilder.DropColumn(
                name: "country_id",
                schema: "data",
                table: "addresses");
        }
    }
}
