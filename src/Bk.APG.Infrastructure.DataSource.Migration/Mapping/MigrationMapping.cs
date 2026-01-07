using Bk.APG.Business.Models;
using Bk.APG.Infrastructure.DataSource.Migration.Models;
using Person = Bk.APG.Business.Models.Person;

namespace Bk.APG.Infrastructure.DataSource.Migration.Mapping;

public static class MigrationMapping
{
    public static Address ToAddress(Adresse adresse)
    {
        return new Address
        {
            Id = adresse.Guid,
            CompanyName = adresse.Firma,
            Street = adresse.Strasse,
            PoBox = adresse.Postfach,
            CountryCode = adresse.LänderCode,
            Zip = adresse.Plz,
            City = adresse.Ort,
            CantonId = adresse.KantonGuid,
            Phone = adresse.Telefon,
            Mobile = adresse.Mobile,
            Email = adresse.Email,
            Created = adresse.InsertDate,
            CreatedBy = adresse.LastupdateUser is not null ? adresse.LastupdateUser : "migration",
            Modified = adresse.UpdateDate,
            ModifiedBy = adresse.LastupdateUser is not null ? adresse.LastupdateUser : "migration",
            OldId = adresse.Id,
        };
    }

    public static Committee ToCommittee(Gremium gremium)
    {
        return new Committee
        {
            Id = gremium.Guid,
            BeginDate = DateOnly.FromDateTime(gremium.BeginnDatum != null ? (DateTime)gremium.BeginnDatum : DateTime.Now),
            EndDate = gremium.EndDatum != null ? DateOnly.FromDateTime((DateTime)gremium.EndDatum) : null,
            CommitteeNumber = gremium.Id,
            Created = gremium.InsertDate,
            CreatedBy = gremium.LastupdateUser ?? "migration",
            Modified = gremium.UpdateDate,
            ModifiedBy = gremium.LastupdateUser ?? "migration",
            DescriptionGerman = gremium.BezeichnungD ?? string.Empty,
            DescriptionFrench = gremium.BezeichnungF ?? string.Empty,
            DescriptionItalian = gremium.BezeichnungI ?? string.Empty,
            DescriptionRomansh = gremium.BezeichnungR ?? string.Empty,
            DepartmentId = gremium.DepartementGuid,
            OfficeId = gremium.AmtGuid,
            CommitteeLevelId = gremium.StufeGuid,
            CommitteeTypeId = gremium.GremiumartGuid,
            OldLegalForm = gremium.Rechtsform,
            LegalBase = gremium.GesetzlicheGrundlagen,
            // this field is basically not needed anymore on a committee. We still migrate the values however...
            ReleaseGeneralElection = gremium.FreigabeSammelantrag ?? false,
            FederalLawEstablishment = gremium.Bundesgesetz,
            MarketOrientated = gremium.Marktorientiert,
            // This information is redundant, as the committee type already defines, if a committee is extra parliamentary or not
            // ExtraParliamentaryCommission = gremium.KomVer ?? false,
            SupervisionDuty = gremium.Aufsichtsaufgabe,
            TermOfOfficeId = gremium.AmtsperiodeGuid,
            // Currently in migration, we only expect to different options... Theoretically, we could also create past TermOfOfficeDate records (e.g. Amtsperiode von 1999 - 2003) and assign these, but that is very difficult...
            TermOfOfficeDateId = gremium.AmtsperiodeGuid == TermOfOffice.Period4YearsInGeneralElectionGuid ? TermOfOfficeDate.CurrentGeneralElectionGuid : TermOfOfficeDate.IndefiniteDurationGuid,
            MinimalMembers = gremium.MinAnzMitglieder,
            MaximalMembers = gremium.MaxAnzMitglieder,
            VacanciesGeneralElection = gremium.AnzahlVakanzenGEW,
            AdditionalAuthorityMembers = gremium.ZusätzKantonGewMitglieder ?? false,
            LinkAuthorityWebsite = gremium.ZusätzKantonGewMitgliederUrl,
            RemarksBaseData = gremium.BemerkungGrunddaten,
            RemarksBaseDataAdmin = gremium.BemerkungGrunddatenAdmin,
            IsDeleted = false,
            JustificationMembers = gremium.BegrAnzMitglieder,
            JustificationGenders = gremium.BegrGeschlechter,
            MeasuresGenders = gremium.MassnaGremGeschlechter,
            JustificationLanguages = gremium.BegrSprachen,
            MeasuresLanguages = gremium.MassnaGremSprachen,
            LinkHomepageGerman = gremium.HomepageLinkDe,
            LinkHomepageFrench = gremium.HomepageLinkFr,
            LinkHomepageItalian = gremium.HomepageLinkIt,
            LinkHomepageRomansh = gremium.HomepageLinkRm,
            FederalInstitution = null
        };
    }

    public static Person ToPerson(Models.Person person)
    {
        return new Person
        {
            Id = person.Guid,
            BirthYear = person.Jahrgang,
            GivenName = person.Vorname,
            Surname = person.Name,
            Occupation = person.Beruf,
            FederalDuty = person.AngestelltBD,
            FederalAssembly = person.MitgliedBV,
            Title = person.TitelText,
            SalutationId = person.AnredeGuid,
            SalutationText = person.Briefanrede,
            LanguageId = person.SpracheGuid!,
            CorrespondenceLanguageId = person.KorrespondenzSpracheGuid,
            GenderId = person.GeschlechtGuid,
            OfficeAddressId = person.GeschäftsAdresseGuid,
            PrivateAddressId = person.PrivatAdresseGuid,
            RemarksPersonData = person.BemerkungPersonendaten,
            RemarksPersonDataAdmin = person.BemerkungPersonendatenAdmin,
            CorrespondenceAddressId = person.AktiveAdresseGuid,
            Created = person.InsertDate,
            CreatedBy = person.LastupdateUser is not null ? person.LastupdateUser : "migration",
            Modified = person.UpdateDate,
            ModifiedBy = person.LastupdateUser is not null ? person.LastupdateUser : "migration",
            OldId = person.Id,
            NoInterest = person.AnzahlInteressen < 1,
        };
    }

    public static Membership ToMembership(Mitglied mitglied)
    {
        return new Membership
        {
            Id = mitglied.Guid,
            CommitteeId = mitglied.GremiumGuid,
            PersonId = mitglied.PersonGuid,
            MaximumEmploymentLevel = mitglied.BeschäftigungsGradBis > 0 ? mitglied.BeschäftigungsGradBis : null,
            BeginDate = DateOnly.FromDateTime(mitglied.SeitDate),
            EndDate = DateOnly.FromDateTime(mitglied.BisDate),
            ElectionTypeId = mitglied.WahlartGuid,
            ElectionOfficeId = mitglied.WahlbehördeGuid,
            FunctionId = mitglied.FunktionGuid,
            OldMembershipAddition = mitglied.Mitgliedzusatz,
            JustificationLongerDuty = mitglied.BegrAmtszeit,
            JustificationShorterDuty = mitglied.BegrAlter,
            JustificationMemberInFederalDuty = mitglied.BegrBangest,
            JustificationMemberInFederalAssembly = mitglied.BegrBvers,
            Remarks = mitglied.Bemerkung,
            RemarksStatus = mitglied.BemerkungStatus,
            Created = mitglied.InsertDate,
            CreatedBy = mitglied.LastupdateUser ?? "migration",
            Modified = mitglied.UpdateDate,
            ModifiedBy = mitglied.LastupdateUser ?? "migration",
            OldId = mitglied.Id,
            InCorrelationWithFederalDuty = false,
            IsDeleted = false
        };
    }

    public static Interest ToInterest(Interessenbindung interessenbindung)
    {
        return new Interest
        {
            Id = interessenbindung.Guid,
            PersonId = interessenbindung.PersonGuid,
            InterestCommitteeId = interessenbindung.GremiumGuid,
            InterestFunctionId = interessenbindung.FunktionGuid,
            InterestLegalFormId = interessenbindung.RechtsformGuid,
            BeginDate = null,
            EndDate = null,
            Text = interessenbindung.Text,
            Created = interessenbindung.InsertDate,
            CreatedBy = interessenbindung.LastupdateUser is not null ? interessenbindung.LastupdateUser : "migration",
            Modified = interessenbindung.UpdateDate,
            ModifiedBy = interessenbindung.LastupdateUser is not null ? interessenbindung.LastupdateUser : "migration",
            OldId = interessenbindung.Id,
            IsDeleted = false
        };
    }

    public static ContactPoint ToContactPoint(Sekretariat sekretariat, Guid committeeId)
    {
        return new ContactPoint
        {
            Id = sekretariat.Guid,
            CommitteeId = committeeId,
            ContactPointTypeId = sekretariat.ContactPointTypeGuid,
            CompanyName = sekretariat.NameOrganisation,
            Section = sekretariat.Sektion,
            BeginDate = DateOnly.FromDateTime(sekretariat.Seit),
            EndDate = sekretariat.Bis != null ? DateOnly.FromDateTime((DateTime)sekretariat.Bis) : null,
            Street = sekretariat.Strasse,
            PoBox = sekretariat.Postfach,
            Zip = sekretariat.Plz,
            City = sekretariat.Ort,
            Phone = sekretariat.Tel,
            Email = sekretariat.EMail,
            Surname = sekretariat.Nachname,
            GivenName = sekretariat.Vorname,
            Title = sekretariat.AkadTitel,
            LanguageId = sekretariat.SpracheGuid,
            GenderId = string.IsNullOrEmpty(sekretariat.Vorname) && string.IsNullOrEmpty(sekretariat.Nachname) ? null : sekretariat.GeschlechtGuid,
            PersonalPhone = sekretariat.PersonTel,
            PersonalMobile = sekretariat.PersonMobile,
            PersonalEmail = sekretariat.PersonEMail,
            ReleasePersonData = false,
            Created = sekretariat.InsertDate,
            CreatedBy = sekretariat.LastUpdateUser is not null ? sekretariat.LastUpdateUser : "migration",
            Modified = sekretariat.UpdateDate,
            ModifiedBy = sekretariat.LastUpdateUser is not null ? sekretariat.LastUpdateUser : "migration",
            OldId = sekretariat.Id,
        };
    }

    public static AppointmentDecision ToAppointmentDecision(Journal journal)
    {
        return new AppointmentDecision
        {
            Id = journal.Guid,
            CommitteeId = journal.GremiumGuid,
            AppointmentDecisionTypeId = journal.JournalCodeGuid != Guid.Empty ? journal.JournalCodeGuid : null,
            AppointmentDecisionLinkTypeId = journal.LinkTypGuid != Guid.Empty ? journal.LinkTypGuid : null,
            AppointmentDecisionDate = DateOnly.FromDateTime(journal.Datum),
            Text = journal.Text,
            Link = journal.Link == "http://" ? null : journal.Link,
            // we only have one file in the past, so we have to assume the language and choose German as default.
            // FileReferenceGermanId = journal.Guid,
            // OriginalDocumentId = journal.Guid,
            Created = journal.InsertDate,
            CreatedBy = journal.LastUpdateUser is not null ? journal.LastUpdateUser : "migration",
            Modified = journal.UpdateDate,
            ModifiedBy = journal.LastUpdateUser is not null ? journal.LastUpdateUser : "migration",
            OldId = journal.Id,
        };
    }

    // This is only needed, when migrating the occupations directly to the repository.
    public static Occupation ToOccupation(OccupationAnnotation maleTexts, OccupationAnnotation femaleTexts, string code)
    {
        var newId = Guid.NewGuid();

        return new Occupation
        {
            Id = newId,
            TextDe = maleTexts.text != null && maleTexts.text.de != null ? maleTexts.text.de : code,
            TextFr = maleTexts.text != null && maleTexts.text.fr != null ? maleTexts.text.fr : code,
            TextIt = maleTexts.text != null && maleTexts.text.it != null ? maleTexts.text.it : code,
            TextRm = string.Empty,
            TextFemaleDe = femaleTexts.text != null && femaleTexts.text.de != null ? femaleTexts.text.de : code,
            TextFemaleFr = femaleTexts.text != null && femaleTexts.text.fr != null ? femaleTexts.text.fr : code,
            TextFemaleIt = femaleTexts.text != null && femaleTexts.text.it != null ? femaleTexts.text.it : code,
            TextFemaleRm = string.Empty,
            Created = DateTime.UtcNow,
            CreatedBy = "migration",
            Modified = DateTime.UtcNow,
            ModifiedBy = "migration",
            IsDeleted = false,
            DescriptionDe = string.Empty,
            DescriptionFr = string.Empty,
            DescriptionIt = string.Empty,
            DescriptionRm = string.Empty,
            Sort = 0,
            Uri = "www.todo.occupation." + code
        };
    }
}
