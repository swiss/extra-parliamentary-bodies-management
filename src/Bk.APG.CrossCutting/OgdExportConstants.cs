namespace Bk.APG.CrossCutting;

public static class OgdExportConstants
{
    public const string SchemaName = "schema:name";
    public const string SchemaLegalName = "schema:legalName";
    public const string SchemaIdentifier = "schema:identifier";
    public const string SchemaDescription = "schema:description";
    public const string SchemaUrl = "schema:url";
    public const string SchemaGender = "schema:gender";
    public const string SchemaLanguage = "schema:language";
    public const string SchemaGivenName = "schema:givenName";
    public const string SchemaFamilyName = "schema:familyName";
    public const string SchemaHonorificPrefix = "schema:honorificPrefix";
    public const string SchemaWorksFor = "schema:worksFor";
    public const string SchemaPublisher = "schema:publisher";
    public const string SchemaCreator = "schema:creator";
    public const string SchemaDateCreated = "schema:dateCreated";
    public const string SchemaDatePublished = "schema:datePublished";
    public const string SchemaDateModified = "schema:dateModified";
    public const string SchemaAddress = "schema:address";
    public const string SchemaGovernmentOrganization = "schema:GovernmentOrganization";
    public const string SchemaPostalAddress = "schema:PostalAddress";
    public const string SchemaStreetAddress = "schema:streetAddress";
    public const string SchemaPostalCode = "schema:postalCode";
    public const string SchemaAddressLocality = "schema:addressLocality";
    public const string SchemaMember = "schema:member";
    public const string SchemaAdditionalType = "schema:additionalType";
    public const string SchemaEmail = "schema:email";
    public const string SchemaSubjectOf = "schema:subjectOf";
    public const string SchemaTelephone = "schema:telephone";
    public const string SchemaTitle = "schema:title";
    public const string SchemaSection = "schema:section";
    public const string SchemaPostOfficeBoxNumber = "schema:postOfficeBoxNumber";

    public const string PersonHasOccupation = "person:hasOccupation";
    public const string PersonHasOffice = "person:hasOffice";
    public const string PersonHasDepartment = "person:hasDepartment";

    public const string RdfType = "rdf:type";

    public const string LdAdminChLink = "https://ld.admin.ch/";
    public const string LdFCh = "ld:FCh";

    public const string CommitteeHasSecretariat = "committee:hasSecretariat";
    public const string CommitteeHasDataProtectionOfficer = "committee:hasDataProtectionOfficer";

    public const string LanguageDe = "de";
    public const string LanguageFr = "fr";
    public const string LanguageIt = "it";
    public const string LanguageRm = "rm";

    public const string DepartmentEda = "eda";
    public const string DepartmentEdi = "edi";
    public const string DepartmentEjpd = "ejpd";
    public const string DepartmentVbs = "vbs";
    public const string DepartmentEfd = "efd";
    public const string DepartmentWbf = "wbf";
    public const string DepartmentUvek = "uvek";

    public const string NamespaceApg = "apg";
    public const string NamespaceContactPointType = "contact-point-type";
    public const string NamespaceCommitteeType = "committee-type";
    public const string NamespaceFunction = "function";
    public const string NamespaceCanton = "canton";
    public const string NamespaceInterestFunction = "interest-function";
    public const string NamespaceInterestCommittee = "interest-committee";
    public const string NamespaceOccupation = "occupation";
    public const string NamespacePerson = "person";
    public const string NamespaceCommittee = "committee";
    public const string NamespaceContactPoint = "contact-point";
    public const string NamespaceMembership = "membership";
    public const string NamespaceOrganization = "organization";
    public const string NamespaceAppointmentDecision = "appointment-decision";
    public const string NamespaceVestedInterest = "vested-interest";
    public const string NamespaceCommitteeFunctionStatistic = "committee-function-statistic";
    public const string NamespaceCommitteeCantonStatistic = "committee-canton-statistic";
    public const string NamespaceCommitteeCantonDetailStatistic = "committee-canton-detail-statistic";
    public const string NamespaceCommitteeGenderLanguageStatistic = "committee-gender-language-statistic";
    public const string NamespaceCommitteeTypeStatistic = "committee-type-statistic";
    public const string NamespaceExtraparliamentaryCommission = "extra-parliamentary-commission";
    public const string NamespaceNonExtraparliamentaryCommission = "non-extra-parliamentary-commission";
    public const string NamespaceAdministrationCommission = "administration-commission";
    public const string NamespaceAuthoritiesCommission = "authorities-commission";
    public const string NamespaceManagementCommission = "management-commission";
    public const string NamespaceFederalAgenciesCommission = "federal-agencies-commission";

    public const string NamespaceLd = "ld";
    public const string NamespaceRld = "rld";
    public const string NamespaceSchema = "schema";
    public const string NamespaceRdf = "rdf";
    public const string NamespaceW3 = "w3";
    public const string NamespaceCube = "cube";

    public const string UriMembership = "membership:1";
    public const string UriVestedInterests = "vested-interest:1";

    public const string UriCommitteeFunctionStatistic = $"{NamespaceCommitteeFunctionStatistic}:1";
    public const string UriCommitteeCantonStatistic = $"{NamespaceCommitteeCantonStatistic}:1";
    public const string UriCommitteeCantonDetailStatistic = $"{NamespaceCommitteeCantonDetailStatistic}:1";
    public const string UriCommitteeGenderLanguageStatistic = $"{NamespaceCommitteeGenderLanguageStatistic}:1";
    public const string UriCommitteeTypeStatistic = $"{NamespaceCommitteeTypeStatistic}:1";

    // set to the production date of APG
    public const string SchemaPublishedDate = "2026-01-14";

    public static string CreateUriLinkForLdAdminCh(string uri)
    {
        var subPath = uri.Replace(LdAdminChLink, "");
        var ldAdminReference = $"ld:{subPath}";

        return ldAdminReference;
    }
}
