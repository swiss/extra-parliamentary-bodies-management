using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class TranslateMissingMasterDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"
                update data.membership_additions set text_fr = 'Représentant des employeurs', text_it = 'Rappresentanza dei datori di lavoro' where id = '0142f21e-4036-4a59-be34-c35664886bb1';
                update data.membership_additions set text_fr = 'Représentant des travailleurs', text_it = 'Rappresentanza dei lavoratori' where id = 'd7e3d087-0cac-4442-937b-4d19b66a4c2e';
                update data.membership_additions set text_fr = 'Représentant des cantons', text_it = 'Rappresentanza dei Cantoni' where id = '58ef0bd2-c357-43ec-9190-c5c1e0b01d29';
                update data.membership_additions set text_fr = 'Représentant du secteur privé', text_it = 'Rappresentanza del settore privato' where id = 'bfbe300d-dada-4a8c-8f38-1ebbe9c63719';
                update data.membership_additions set text_fr = 'Représentant de l''administration fédérale', text_it = 'Rappresentanza dell''Amministrazione federale' where id = '4108d747-10f2-4f20-afc1-54161b5996c8';
                update data.membership_additions set text_fr = 'Représentant des milieux scientifiques', text_it = 'Rappresentanza della scienza' where id = '7c2c1ecb-7ffd-47eb-b817-82ded59df633';
                update data.membership_additions set text_fr = 'Représentant des assureurs-maladie', text_it = 'Rappresentanza degli assicuratori malattie' where id = '219b22c2-03fd-4418-ac52-da5ca1f44d3b';
                update data.membership_additions set text_fr = 'Représentant des assurés', text_it = 'Rappresentanza degli assicurati' where id = '13b13d4a-9fdd-4238-b5c6-f16c2c992dd0';
                update data.membership_additions set text_fr = 'Représentant des milieux de l''éthique médicale', text_it = 'Rappresentanza dell''etica medica' where id = '40e66991-0f00-432a-a32f-4f9038adda7b';
                update data.membership_additions set text_fr = 'Représentant de l''Association des communes suisses', text_it = 'Rappresentanza dell''Associazione dei Comuni' where id = '0be26faa-0ae4-45c6-b971-07c53dc637b8';
                update data.membership_additions set text_fr = 'D''office', text_it = 'D''ufficio' where id = '81da54ba-0dee-477d-8a9a-0859789c5d75';
                update data.membership_additions set text_fr = 'Cadre du domaine des EPF', text_it = 'Responsabile del settore Politecnici federali (PF)' where id = '9dcbf649-5237-4060-94ad-fea728e9cd6c';
                update data.membership_additions set text_fr = 'Représentant du corps médical', text_it = 'Rappresentanza dell''ordine dei medici' where id = 'ac40cb0b-8712-4a4a-864b-035e5eef680f';
                update data.membership_additions set text_fr = 'Représentant des pharmaciens', text_it = 'Rappresentanza dell''ordine dei farmacisti' where id = '32ffc467-4e21-459e-aceb-0eb64dc71f40';
                update data.membership_additions set text_fr = 'Représentant des pharmaciens et des milieux de la médecine complémentaire', text_it = 'Rappresentanza dell''ordine dei farmacisti e della medicina complementare' where id = '99f0afb7-e34b-4afb-b253-4aac94dfb8eb';
                update data.membership_additions set text_fr = 'Représentant des médecins de confiance', text_it = 'Rappresentanza dei medici di fiducia' where id = '3af5c62f-a99d-4765-804d-a28030909998';
                update data.membership_additions set text_fr = 'Représentant des hôpitaux', text_it = 'Rappresentanza degli ospedali' where id = 'eb519b3c-5331-4648-935d-66deb3a8e1d2';
                update data.membership_additions set text_fr = 'Représentant des milieux de l''éthique médicale', text_it = 'Rappresentanza dell''etica medica' where id = '80ef61f4-2eca-438a-9f30-eb82d71e4431';
                update data.membership_additions set text_fr = 'Représentant des laboratoires', text_it = 'Rappresentanza dei laboratori' where id = 'd3a5c29d-ff6c-4db6-847f-1bc61708198a';
                update data.membership_additions set text_fr = 'Représentant des professeurs d''analyses de laboratoire', text_it = 'Rappresentanza dei docenti di analisi di laboratorio' where id = '1c414165-bdb0-4d01-9f8f-244e5271594e';
                update data.membership_additions set text_fr = 'Représentant des centres de remise de moyens et d''appareils', text_it = 'Rappresentanza dei centri di consegna di mezzi e apparecchi' where id = '3a092988-1dba-4d3c-bd71-ba49d6492006';
                update data.membership_additions set text_fr = 'Représentant des fabricants et distributeurs de moyens et d''appareils', text_it = 'Rappresentanza dei produttori e distributori di mezzi e apparecchi' where id = '7e9e415b-e7d2-4caf-917c-5d6d0dc3393d';
                update data.membership_additions set text_fr = 'Représentant spécialisé / Spécialiste', text_it = 'Organo di rappresentanza del settore/Specialisti' where id = '934915b3-831a-4d66-a890-263839c4ae68';
                update data.membership_additions set text_fr = 'Représentant de l''industrie pharmaceutique', text_it = 'Rappresentanza dell''industria farmaceutica' where id = 'fad6daeb-5e5c-445f-b2ab-cb02690397af';
                update data.membership_additions set text_fr = 'Représentant des organisations patronales', text_it = 'Rappresentanza delle organizzazioni dei datori di lavoro' where id = '91240fa6-4480-4e87-b8cd-97380c571daf';
                update data.membership_additions set text_fr = 'Représentant des organisations d''employés', text_it = 'Rappresentanza delle organizzazioni dei lavoratori' where id = '485e464e-2159-4848-af9a-0bad11ec47e6';
                update data.membership_additions set text_fr = 'Représentant des syndicats', text_it = 'Rappresentanza dei sindacati' where id = 'f49ef77f-37e2-430b-8d52-0570178ac7e0';
                update data.membership_additions set text_fr = 'Représentant des hautes écoles universitaires', text_it = 'Rappresentanza delle scuole universitarie' where id = '641b8652-56e4-42ac-8f79-60a1fc315981';
                update data.membership_additions set text_fr = 'Représentant des organes d''exécution', text_it = 'Rappresentanza degli organi esecutivi' where id = 'a7cf0ea1-22e3-470b-b928-0520f5cb0ba8';
                update data.membership_additions set text_fr = 'Représentant des institutions d''assurance', text_it = 'Rappresentanza degli istituti d''assicurazione' where id = '5aee289e-d943-4fdd-96a1-a4d39c2136d5';
                update data.membership_additions set text_fr = 'Représentant de l''Association des communes suisses', text_it = 'Rappresentanza dell''Associazione dei Comuni' where id = '0d70b752-6591-44c6-9cfc-984693b1b8d5';
            ");

            migrationBuilder.Sql(@$"
                update data.appointment_decision_link_types set text_fr = 'Numéro EXE', text_it = 'Numero EXE' where id = 'add3267c-b676-4ee0-a1c9-a0ac2c703d06';
                update data.appointment_decision_link_types set text_fr = 'Standard', text_it = 'Standard' where id = '3e0016ae-13db-4a1f-9e26-ada79d93834e';
            ");

            migrationBuilder.Sql(@$"
                update data.appointment_decision_types set text_fr = 'Décision du CF', text_it = 'Decisione del Consiglio federale' where id = '03043662-caa9-40ec-ab77-d8f2825eb775';
                update data.appointment_decision_types set text_fr = 'Rapport', text_it = 'Rapporto' where id = 'bdfcfe40-93d0-11f0-8674-5d4ae449bed2';
            ");

            migrationBuilder.Sql(@$"
                update data.candidate_list_states set text_fr = 'Terminé', text_it = 'Concluso' where id = '7ec5c44a-b6ae-45bc-8d91-5b8d2db7c897';
                update data.candidate_list_states set text_fr = 'Prêt pour la proposition au CF', text_it = 'Pronto per la proposta al Consiglio federale' where id = 'b73a46c7-3222-4d5f-87ff-42c3e82b23dc';
                update data.candidate_list_states set text_fr = 'Proposition', text_it = 'Proposta' where id = 'c9d2b6e3-0f3f-40f3-8bb2-fdb6cf1f2073';
                update data.candidate_list_states set text_fr = 'Projet', text_it = 'Progetto' where id = 'fa78e8d2-8c67-4e1c-8497-6bcac845b3d1';
            ");

            migrationBuilder.Sql(@$"
                update data.worklist_task_states set text_fr = 'Inactif', text_it = 'Inattivo' where id = '0c4f6a7d-3e42-49a0-9b1c-27a2e7d2f621';
                update data.worklist_task_states set text_fr = 'Actif', text_it = 'Attivo' where id = '4d5fbb42-93b7-4a71-b0a4-3a3fbd6f9e75';
                update data.worklist_task_states set text_fr = 'Expédié', text_it = 'Completato' where id = 'a5c92d78-70f3-4ca1-9e2e-814f8d7f30c0';
            ");

            migrationBuilder.Sql(@$"
                update data.worklist_task_types set text_fr = 'Finaliser le RI : justification obligatoire', text_it = 'Finalizzazione ERI: obbligo di fornire una motivazione' where id = '28f59879-a211-4816-b894-66f8d60961a8';
                update data.worklist_task_types set text_fr = 'Prêt pour la proposition au Conseil fédéral relative au RI', text_it = 'Pronto per la proposta al Consiglio federale nell''ambito delle ERI' where id = '46aacc55-40b7-49d1-ac13-d6ab445a9943';
                update data.worklist_task_types set text_fr = 'Vérification', text_it = 'Verifica' where id = '5d1d9558-5b78-4e70-a560-4fd1d078ae60';
                update data.worklist_task_types set text_fr = 'Lancement du RI', text_it = 'Avvio ERI' where id = '6b8ea0f1-12d7-49de-ae38-fc57a87a6b1d';
                update data.worklist_task_types set text_fr = 'Transmettre la liste de candidats pour le RI', text_it = 'Inviare proposta di candidatura ERI ' where id = '95fd596a-1a7c-47d7-9b5f-50a5cc3fe43c';
                update data.worklist_task_types set text_fr = 'Approuver la liste de candidats pour le RI', text_it = 'Approvare proposta di candidatura ERI' where id = 'bd516645-e427-4dcf-b6e1-8464b8645818';
                update data.worklist_task_types set text_fr = 'Établir la liste de candidats pour le RI', text_it = 'Elaborare proposta di candidatura ERI' where id = 'd2de9cdd-9d16-4564-8968-1df10f9fb3ce';
                update data.worklist_task_types set text_fr = 'Transmettre le lancement du RI', text_it = 'Inoltrare avvio ERI' where id = 'f3e2b1c4-9d7a-4b2e-9231-7cb04a5b0d66';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
