using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentsOfficesInRomansh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.Sql(@$"
                update data.departments set text_rm = 'ChF', description_rm = 'Chanzlia federala' where uri = 'https://ld.admin.ch/FCh';
                update data.departments set text_rm = 'DFAE', description_rm = 'Departament federal d''affars exteriurs' where uri = 'https://ld.admin.ch/department/I';
                update data.departments set text_rm = 'DFI', description_rm = 'Departament federal da l''intern' where uri = 'https://ld.admin.ch/department/II';
                update data.departments set text_rm = 'DFGP', description_rm = 'Departament federal da giustia e polizia' where uri = 'https://ld.admin.ch/department/III';
                update data.departments set text_rm = 'DDPS', description_rm = 'Departament federal da defensiun, protecziun da la populaziun e sport' where uri = 'https://ld.admin.ch/department/IV';
                update data.departments set text_rm = 'DFF', description_rm = 'Departament federal da finanzas' where uri = 'https://ld.admin.ch/department/V';
                update data.departments set text_rm = 'DEFR', description_rm = 'Departament federal d’economia, furmaziun e retschertga' where uri = 'https://ld.admin.ch/department/VI';
                update data.departments set text_rm = 'DATEC', description_rm = 'Departament federal per ambient, traffic, energia e communicaziun' where uri = 'https://ld.admin.ch/department/VII';
             ");

            migrationBuilder.Sql(@$"
                update data.offices set description_rm = 'Chanzlia federala', text_rm = 'ChF' where uri = 'https://ld.admin.ch/FCh';
                update data.offices set description_rm = 'Secretariat general', text_rm = 'SG-DFAE' where uri = 'https://ld.admin.ch/office/I.1.1';
                update data.offices set description_rm = 'Secretariat da stadi dal DFAE', text_rm = 'SES-DFAE' where uri = 'https://ld.admin.ch/office/I.1.2';
                update data.offices set description_rm = 'Direcziun da dretg internaziunal public', text_rm = 'DDIP' where uri = 'https://ld.admin.ch/office/I.1.4';
                update data.offices set description_rm = 'Direcziun da svilup e da cooperaziun', text_rm = 'DSC' where uri = 'https://ld.admin.ch/office/I.1.5';
                update data.offices set description_rm = 'Direcziun da resursas', text_rm = 'DR' where uri = 'https://ld.admin.ch/office/I.1.7';
                update data.offices set description_rm = 'Direcziun consulara', text_rm = 'DC' where uri = 'https://ld.admin.ch/office/I.1.8';
                update data.offices set description_rm = 'Secretariat general', text_rm = 'SG-DFI' where uri = 'https://ld.admin.ch/office/II.1.1';
                update data.offices set description_rm = 'Uffizi federal per l''egualitad tranter dunna ed um', text_rm = 'UFEG' where uri = 'https://ld.admin.ch/office/II.1.2';
                update data.offices set description_rm = 'Uffizi federal da cultura', text_rm = 'UFC' where uri = 'https://ld.admin.ch/office/II.1.3';
                update data.offices set description_rm = 'Archiv federal svizzer', text_rm = 'AFS' where uri = 'https://ld.admin.ch/office/II.1.4';
                update data.offices set description_rm = 'Uffizi federal per meteorologia e climatologia', text_rm = 'MeteoSvizra' where uri = 'https://ld.admin.ch/office/II.1.5';
                update data.offices set description_rm = 'Uffizi federal da sanadad publica', text_rm = 'UFSP' where uri = 'https://ld.admin.ch/office/II.1.6';
                update data.offices set description_rm = 'Uffizi federal da statistica', text_rm = 'UST' where uri = 'https://ld.admin.ch/office/II.1.7';
                update data.offices set description_rm = 'Uffizi federal d''assicuranzas socialas', text_rm = 'UFAS' where uri = 'https://ld.admin.ch/office/II.1.8';
                update data.offices set description_rm = 'Uffizi federal da segirezza alimentara e fatgs veterinars', text_rm = 'USAV' where uri = 'https://ld.admin.ch/office/II.1.9';
                update data.offices set description_rm = 'Museum naziunal svizzer', text_rm = 'MNS' where uri = 'https://ld.admin.ch/office/II.2.2.1';
                update data.offices set description_rm = 'Pro Helvetia', text_rm = 'Pro Helvetia' where uri = 'https://ld.admin.ch/office/II.2.2.2';
                update data.offices set description_rm = 'Institut svizzer per products terapeutics', text_rm = 'Swissmedic' where uri = 'https://ld.admin.ch/office/II.2.2.3';
                update data.offices set description_rm = 'Fonds da cumpensaziun AVS/AI/UCG', text_rm = 'compenswiss' where uri = 'https://ld.admin.ch/office/II.2.2.4';
                update data.offices set description_rm = 'Secretariat general', text_rm = 'SG-DFGP' where uri = 'https://ld.admin.ch/office/III.1.1';
                update data.offices set description_rm = 'Uffizi federal da giustia', text_rm = 'UFG' where uri = 'https://ld.admin.ch/office/III.1.2';
                update data.offices set description_rm = 'Uffizi federal da polizia', text_rm = 'fedpol' where uri = 'https://ld.admin.ch/office/III.1.3';
                update data.offices set description_rm = 'Secretariat da stadi per migraziun', text_rm = 'SEM' where uri = 'https://ld.admin.ch/office/III.1.4';
                update data.offices set description_rm = 'Servetsch Surveglianza da la correspundenza postala e dal traffic da telecommunicaziun SCPT', text_rm = 'SCPT' where uri = 'https://ld.admin.ch/office/III.2.1.2';
                update data.offices set description_rm = 'Cumissiun per la prevenziun cunter la tortura', text_rm = 'CNPT' where uri = 'https://ld.admin.ch/office/III.2.1.3';
                update data.offices set description_rm = 'Institut svizzer da dretg cumparativ', text_rm = 'ISDC' where uri = 'https://ld.admin.ch/office/III.2.2.1';
                update data.offices set description_rm = 'Institut Federal da Proprietad Intellectuala', text_rm = 'IPI' where uri = 'https://ld.admin.ch/office/III.2.2.2';
                update data.offices set description_rm = 'Autoritad federala da surveglianza en chaussas da revisiun', text_rm = 'ASR' where uri = 'https://ld.admin.ch/office/III.2.2.3';
                update data.offices set description_rm = 'Institut federal da metrologia', text_rm = 'METAS' where uri = 'https://ld.admin.ch/office/III.2.2.4';
                update data.offices set description_rm = 'Secretariat general', text_rm = 'SG-DDPS' where uri = 'https://ld.admin.ch/office/IV.1.1';
                update data.offices set description_rm = 'Secretariat da stadi per la politica da segirezza', text_rm = 'SEPOS' where uri = 'https://ld.admin.ch/office/IV.1.1a';
                update data.offices set description_rm = 'Servetsch d''infurmaziun da la Confederaziun', text_rm = 'SIC' where uri = 'https://ld.admin.ch/office/IV.1.2';
                update data.offices set description_rm = 'Auditorat superiur', text_rm = 'AS' where uri = 'https://ld.admin.ch/office/IV.1.3';
                update data.offices set description_rm = 'Gruppa da defensiun', text_rm = 'Gruppa da defensiun' where uri = 'https://ld.admin.ch/office/IV.1.4';
                update data.offices set description_rm = 'Uffizi federal da l''armament', text_rm = 'armasuisse' where uri = 'https://ld.admin.ch/office/IV.1.5';
                update data.offices set description_rm = 'Uffizi federal da topografia', text_rm = 'swisstopo' where uri = 'https://ld.admin.ch/office/IV.1.5a';
                update data.offices set description_rm = 'Uffizi federal da protecziun da la populaziun', text_rm = 'UFPP' where uri = 'https://ld.admin.ch/office/IV.1.6';
                update data.offices set description_rm = 'Uffizi federal da sport', text_rm = 'UFSPO' where uri = 'https://ld.admin.ch/office/IV.1.7';
                update data.offices set description_rm = 'Uffizi federal da la cybersegirezza', text_rm = 'UFCS' where uri = 'https://ld.admin.ch/office/IV.1.8';
                update data.offices set description_rm = 'Autoridad da surveglianza independenta davart las activitads d''informaziun', text_rm = 'AS-AIn' where uri = 'https://ld.admin.ch/office/IV.2.1.1';
                update data.offices set description_rm = 'Secretariat general', text_rm = 'SG-DFF' where uri = 'https://ld.admin.ch/office/V.1.1';
                update data.offices set description_rm = 'Secretariat da stadi per dumondas finanzialas internaziunalas', text_rm = 'SFI' where uri = 'https://ld.admin.ch/office/V.1.2';
                update data.offices set description_rm = 'Administraziun federala da finanzas', text_rm = 'AFF' where uri = 'https://ld.admin.ch/office/V.1.3';
                update data.offices set description_rm = 'Uffizi federal da persunal', text_rm = 'UFPER' where uri = 'https://ld.admin.ch/office/V.1.4';
                update data.offices set description_rm = 'Administraziun federala da taglia', text_rm = 'AFT' where uri = 'https://ld.admin.ch/office/V.1.5';
                update data.offices set description_rm = 'Uffizi federal da la duana e da la segirezza dals cunfins', text_rm = 'UDSC' where uri = 'https://ld.admin.ch/office/V.1.6';
                update data.offices set description_rm = 'Uffizi federal d''informatica e da telecommunicaziun', text_rm = 'UFIT' where uri = 'https://ld.admin.ch/office/V.1.7';
                update data.offices set description_rm = 'Uffizi federal per edifizis e logistica', text_rm = 'UFEL' where uri = 'https://ld.admin.ch/office/V.1.8';
                update data.offices set description_rm = 'Organ da direcziun informatica da la Confederaziun', text_rm = 'ODIC' where uri = 'https://ld.admin.ch/office/V.1.9';
                update data.offices set description_rm = 'Controlla federala da finanzas', text_rm = 'CDF' where uri = 'https://ld.admin.ch/office/V.2.1.1';
                update data.offices set description_rm = 'Autoritad federala per la surveglianza dals martgads da finanzas', text_rm = 'FINMA' where uri = 'https://ld.admin.ch/office/V.2.2.1';
                update data.offices set description_rm = 'Cassa federala da pensiun', text_rm = 'PUBLICA' where uri = 'https://ld.admin.ch/office/V.2.2.2';
                update data.offices set description_rm = 'Secretariat general', text_rm = 'SG-DEFR' where uri = 'https://ld.admin.ch/office/VI.1.1';
                update data.offices set description_rm = 'Surveglianza dals pretschs', text_rm = 'SPR' where uri = 'https://ld.admin.ch/office/VI.1.2';
                update data.offices set description_rm = 'Secretariat da stadi per l''economia', text_rm = 'SECO' where uri = 'https://ld.admin.ch/office/VI.1.3';
                update data.offices set description_rm = 'Secretariat da stadi per furmaziun, retschertga ed innovaziun', text_rm = 'SEFRI' where uri = 'https://ld.admin.ch/office/VI.1.4';
                update data.offices set description_rm = 'Uffizi federal d''agricultura', text_rm = 'UFAG' where uri = 'https://ld.admin.ch/office/VI.1.5';
                update data.offices set description_rm = 'Uffizi federal per il provediment economic dal pajais', text_rm = 'UFPE' where uri = 'https://ld.admin.ch/office/VI.1.7';
                update data.offices set description_rm = 'Uffizi federal d''abitaziuns', text_rm = 'UFAB' where uri = 'https://ld.admin.ch/office/VI.1.8';
                update data.offices set description_rm = 'Uffizi federal dal servetsch civil', text_rm = 'CIVI' where uri = 'https://ld.admin.ch/office/VI.1.9';
                update data.offices set description_rm = 'Sectur da las scolas politecnicas federalas', text_rm = 'sectur da las PF' where uri = 'https://ld.admin.ch/office/VI.2.1.1';
                update data.offices set description_rm = 'Svizra Turissem', text_rm = 'ST' where uri = 'https://ld.admin.ch/office/VI.2.2.1';
                update data.offices set description_rm = 'Institut federal per provediment, serenaziun e protecziun da las auas', text_rm = 'EAWAG' where uri = 'https://ld.admin.ch/office/VI.2.2.10';
                update data.offices set description_rm = 'Agentura svizra per la promoziun da l''innovaziun', text_rm = 'Innosuisse' where uri = 'https://ld.admin.ch/office/VI.2.2.11';
                update data.offices set description_rm = 'Societad svizra da credit d''hotel', text_rm = 'SCH' where uri = 'https://ld.admin.ch/office/VI.2.2.2';
                update data.offices set description_rm = 'Assicuranza svizra cunter las ristgas da l''export', text_rm = 'ASRE' where uri = 'https://ld.admin.ch/office/VI.2.2.3';
                update data.offices set description_rm = 'Institut federal da scola auta per la furmaziun professiunala', text_rm = 'IFFP' where uri = 'https://ld.admin.ch/office/VI.2.2.4';
                update data.offices set description_rm = 'Scola politecnica federala Turitg', text_rm = 'SPFT' where uri = 'https://ld.admin.ch/office/VI.2.2.5';
                update data.offices set description_rm = 'Scola politecnica federala Losanna', text_rm = 'SPFL' where uri = 'https://ld.admin.ch/office/VI.2.2.6';
                update data.offices set description_rm = 'Institut Paul Scherrer', text_rm = 'PSI' where uri = 'https://ld.admin.ch/office/VI.2.2.7';
                update data.offices set description_rm = 'Institut federal per la perscrutaziun da guaud, naiv e cuntrada', text_rm = 'WSL' where uri = 'https://ld.admin.ch/office/VI.2.2.8';
                update data.offices set description_rm = 'Institut federal da controlla da material e da perscrutaziun', text_rm = 'Empa' where uri = 'https://ld.admin.ch/office/VI.2.2.9';
                update data.offices set description_rm = 'Swiss Investment Fund for Emerging Markets', text_rm = 'SIFEM SA' where uri = 'https://ld.admin.ch/office/VI.2.3.1';
                update data.offices set description_rm = 'Secretariat general', text_rm = 'SG-DATEC' where uri = 'https://ld.admin.ch/office/VII.1.1';
                update data.offices set description_rm = 'Uffizi federal da traffic', text_rm = 'UFT' where uri = 'https://ld.admin.ch/office/VII.1.2';
                update data.offices set description_rm = 'Uffizi federal d''aviatica civila', text_rm = 'UFAC' where uri = 'https://ld.admin.ch/office/VII.1.3';
                update data.offices set description_rm = 'Uffizi federal d''energia', text_rm = 'UFE' where uri = 'https://ld.admin.ch/office/VII.1.4';
                update data.offices set description_rm = 'Uffizi federal da vias', text_rm = 'UVIAS' where uri = 'https://ld.admin.ch/office/VII.1.5';
                update data.offices set description_rm = 'Uffizi federal da communicaziun', text_rm = 'UFCOM' where uri = 'https://ld.admin.ch/office/VII.1.6';
                update data.offices set description_rm = 'Uffizi federal d''ambient', text_rm = 'UFAM' where uri = 'https://ld.admin.ch/office/VII.1.7';
                update data.offices set description_rm = 'Uffizi federal da svilup dal territori', text_rm = 'ARE' where uri = 'https://ld.admin.ch/office/VII.1.8';
                update data.offices set description_rm = 'Inspecturat federal per la segirezza nucleara', text_rm = 'IFSN' where uri = 'https://ld.admin.ch/office/VII.2.2.1';
                update data.offices set description_rm = 'Fond da serrada e fond da dismessa per ils implants nuclears', text_rm = 'STENFO' where uri = 'https://ld.admin.ch/office/VII.2.2.2';
                update data.offices set description_rm = 'Servetsch svizzer d''attribuziun dals trassés', text_rm = 'SAT' where uri = 'https://ld.admin.ch/office/VII.2.2.3';
             ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
