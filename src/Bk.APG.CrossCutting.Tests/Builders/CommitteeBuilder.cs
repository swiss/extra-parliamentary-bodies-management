using Bk.APG.Business.Models;
using Bogus;

namespace Bk.APG.CrossCutting.Tests.Builders;

public class CommitteeBuilder
{
    private readonly Faker _faker = new();

    private Guid _id;
    private int _ogdId;
    private readonly DateTime _created;
    private readonly string _createdBy;
    private readonly DateTime _modified;
    private readonly string _modifiedBy;
    private DateOnly _beginDate;
    private DateOnly? _endDate;
    private int _committeeNumber;
    private string _descriptionGerman;
    private string _descriptionFrench;
    private string _descriptionItalian;
    private string _descriptionRomansh;
    private Department? _department;
    private Guid _departmentId;
    private Office? _office;
    private Guid _officeId;
    private CommitteeLevel? _committeeLevel;
    private Guid _committeeLevelId;
    private CommitteeType? _committeeType;
    private Guid _committeeTypeId;
    private readonly LegalForm? _legalForm;
    private readonly string _oldLegalForm;
    private readonly string _legalBase;
    private readonly bool _releaseGeneralElection;
    private readonly bool _federalLawEstablishment;
    private bool _marketOrientated;
    private bool _supervisionDuty;
    private TermOfOffice? _termOfOffice;
    private Guid _termOfOfficeId;
    private TermOfOfficeDate? _termOfOfficeDate;
    private Guid _termOfOfficeDateId;
    private readonly int _minimalMembers;
    private int _maximalMembers;
    private readonly int _vacanciesGeneralElection;
    private readonly bool _additionalAuthorityMembers;
    private string _linkAuthorityWebsite;
    private readonly string _remarksBaseData;
    private readonly string _remarksBaseDataAdmin;
    private string? _linkHomepageGerman;
    private string? _linkHomepageFrench;
    private string? _linkHomepageItalian;
    private string? _linkHomepageRomansh;
    private readonly bool _isDeleted;
    private readonly List<Membership> _memberships;
    private readonly List<ContactPoint> _contactPoints;
    private readonly List<GeneralElectionCommittee> _generalElectionCommittees;
    private readonly Guid _eiamAssignmentId;

    private readonly uint _rowVersion;

    public CommitteeBuilder()
    {
        _id = _faker.Random.Guid();
        _ogdId = _faker.Random.Int();
        _created = _faker.Date.Past();
        _createdBy = _faker.Person.Email;
        _modified = _faker.Date.Past();
        _modifiedBy = _faker.Person.Email;
        _beginDate = _faker.Date.PastDateOnly();
        _endDate = _faker.Date.FutureDateOnly();
        _committeeNumber = _faker.Random.Int(1);
        _created = _faker.Date.Past();
        _createdBy = _faker.Person.Email;
        _modified = _faker.Date.Past();
        _modifiedBy = _faker.Person.Email;
        _descriptionGerman = _faker.Lorem.Text();
        _descriptionFrench = _faker.Lorem.Text();
        _descriptionItalian = _faker.Lorem.Text();
        _descriptionRomansh = _faker.Lorem.Text();
        _departmentId = _faker.Random.Guid();
        _department = new DepartmentBuilder().WithId(_departmentId).Build();
        _officeId = _faker.Random.Guid();
        _office = new OfficeBuilder().WithId(_officeId).Build();
        _committeeLevelId = _faker.Random.Guid();
        _committeeLevel = new CommitteeLevelBuilder().WithId(_committeeLevelId).Build();
        _committeeTypeId = _faker.Random.Guid();
        _committeeType = new CommitteeTypeBuilder().WithId(_committeeTypeId).Build();
        _legalForm = new LegalFormBuilder().Build();
        _oldLegalForm = _faker.Random.String();
        _legalBase = _faker.Random.String();
        _releaseGeneralElection = _faker.Random.Bool();
        _federalLawEstablishment = _faker.Random.Bool();
        _marketOrientated = _faker.Random.Bool();
        _supervisionDuty = _faker.Random.Bool();
        _termOfOfficeId = _faker.Random.Guid();
        _termOfOffice = new TermOfOfficeBuilder().WithId(_termOfOfficeId).Build();
        _termOfOfficeDateId = _faker.Random.Guid();
        _termOfOfficeDate = new TermOfOfficeDateBuilder().WithId(_termOfOfficeDateId).Build();
        _minimalMembers = _faker.Random.Int();
        _maximalMembers = _faker.Random.Int();
        _vacanciesGeneralElection = _faker.Random.Int();
        _additionalAuthorityMembers = _faker.Random.Bool();
        _linkAuthorityWebsite = _faker.Random.String();
        _remarksBaseData = _faker.Random.String();
        _remarksBaseDataAdmin = _faker.Random.String();
        _linkHomepageGerman = _faker.Random.String().OrNull(_faker);
        _linkHomepageFrench = _faker.Random.String().OrNull(_faker);
        _linkHomepageItalian = _faker.Random.String().OrNull(_faker);
        _linkHomepageRomansh = _faker.Random.String().OrNull(_faker);
        _isDeleted = false;
        _memberships = [];
        _contactPoints = [];
        _rowVersion = _faker.Random.UInt();
        _generalElectionCommittees = [];
        _eiamAssignmentId = _faker.Random.Guid();
    }

    public CommitteeBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public CommitteeBuilder WithOgdId(int id)
    {
        _ogdId = id;
        return this;
    }

    public CommitteeBuilder WithOldId(int oldId)
    {
        _committeeNumber = oldId;
        return this;
    }

    public CommitteeBuilder WithBeginDate(DateOnly beginDate)
    {
        _beginDate = beginDate;
        return this;
    }

    public CommitteeBuilder WithEndDate(DateOnly? endDate)
    {
        _endDate = endDate;
        return this;
    }

    public CommitteeBuilder WithGermanDescription(string description)
    {
        _descriptionGerman = description;
        return this;
    }

    public CommitteeBuilder WithFrenchDescription(string description)
    {
        _descriptionFrench = description;
        return this;
    }

    public CommitteeBuilder WithItalianDescription(string description)
    {
        _descriptionItalian = description;
        return this;
    }

    public CommitteeBuilder WithRomanschDescription(string description)
    {
        _descriptionRomansh = description;
        return this;
    }

    public CommitteeBuilder WithCommitteeLevelId(Guid id)
    {
        _committeeLevelId = id;
        return this;
    }

    public CommitteeBuilder WithCommitteeLevel(CommitteeLevel committeeLevel)
    {
        _committeeLevel = committeeLevel;
        _committeeLevelId = committeeLevel.Id;
        return this;
    }

    public CommitteeBuilder WithCommitteeTypeId(Guid id)
    {
        _committeeTypeId = id;
        return this;
    }

    public CommitteeBuilder WithCommitteeType(CommitteeType committeeType)
    {
        _committeeType = committeeType;
        _committeeTypeId = committeeType.Id;
        return this;
    }

    public CommitteeBuilder WithTermOfOfficeId(Guid id)
    {
        _termOfOfficeId = id;
        return this;
    }

    public CommitteeBuilder WithTermOfOffice(TermOfOffice termOfOffice)
    {
        _termOfOffice = termOfOffice;
        _termOfOfficeId = termOfOffice.Id;
        return this;
    }

    public CommitteeBuilder WithTermOfOfficeDate(TermOfOfficeDate termOfOfficeDate)
    {
        _termOfOfficeDate = termOfOfficeDate;
        _termOfOfficeDateId = termOfOfficeDate.Id;
        return this;
    }

    public CommitteeBuilder WithDepartmentId(Guid id)
    {
        _departmentId = id;
        return this;
    }

    public CommitteeBuilder WithDepartment(Department department)
    {
        _department = department;
        _departmentId = department.Id;
        return this;
    }

    public CommitteeBuilder WithOfficeId(Guid id)
    {
        _officeId = id;
        return this;
    }

    public CommitteeBuilder WithOffice(Office office)
    {
        _office = office;
        _officeId = office.Id;
        return this;
    }

    public CommitteeBuilder WithMarketOrientated(bool marketOrientated)
    {
        _marketOrientated = marketOrientated;
        return this;
    }

    public CommitteeBuilder WithSupervisionDuty(bool supervisionDuty)
    {
        _supervisionDuty = supervisionDuty;
        return this;
    }

    public CommitteeBuilder WithMaximalMember(int maximalMember)
    {
        _maximalMembers = maximalMember;
        return this;
    }

    public CommitteeBuilder WithMembership(Membership membership)
    {
        _memberships.Add(membership);
        return this;
    }

    public CommitteeBuilder WithLinkAuthorityWebsite(string website)
    {
        _linkAuthorityWebsite = website;
        return this;
    }

    public CommitteeBuilder WithContactPoint(ContactPoint contactPoint)
    {
        _contactPoints.Add(contactPoint);
        return this;
    }

    public CommitteeBuilder WithGeneralElectionCommittee(GeneralElectionCommittee generalElectionCommittee)
    {
        _generalElectionCommittees.Add(generalElectionCommittee);
        return this;
    }

    public CommitteeBuilder WithGermanLinkHomepage(string link)
    {
        _linkHomepageGerman = link;
        return this;
    }

    public CommitteeBuilder WithFrenchLinkHomepage(string link)
    {
        _linkHomepageFrench = link;
        return this;
    }

    public CommitteeBuilder WithItalianLinkHomepage(string link)
    {
        _linkHomepageItalian = link;
        return this;
    }

    public CommitteeBuilder WithRomanshLinkHomepage(string link)
    {
        _linkHomepageRomansh = link;
        return this;
    }

    public Committee Build()
    {
        return new Committee
        {
            Id = _id,
            OgdId = _ogdId,
            Created = _created,
            CreatedBy = _createdBy,
            Modified = _modified,
            ModifiedBy = _modifiedBy,
            BeginDate = _beginDate,
            EndDate = _endDate,
            CommitteeNumber = _committeeNumber,
            DescriptionGerman = _descriptionGerman,
            DescriptionFrench = _descriptionFrench,
            DescriptionItalian = _descriptionItalian,
            DescriptionRomansh = _descriptionRomansh,
            DepartmentId = _departmentId,
            Department = _department,
            OfficeId = _officeId,
            Office = _office,
            CommitteeLevelId = _committeeLevelId,
            CommitteeLevel = _committeeLevel,
            CommitteeTypeId = _committeeTypeId,
            CommitteeType = _committeeType,
            LegalBase = _legalBase,
            LegalForm = _legalForm,
            OldLegalForm = _oldLegalForm,
            ReleaseGeneralElection = _releaseGeneralElection,
            FederalLawEstablishment = _federalLawEstablishment,
            MarketOrientated = _marketOrientated,
            SupervisionDuty = _supervisionDuty,
            TermOfOfficeId = _termOfOfficeId,
            TermOfOffice = _termOfOffice,
            TermOfOfficeDateId = _termOfOfficeDateId,
            TermOfOfficeDate = _termOfOfficeDate,
            MinimalMembers = _minimalMembers,
            MaximalMembers = _maximalMembers,
            VacanciesGeneralElection = _vacanciesGeneralElection,
            AdditionalAuthorityMembers = _additionalAuthorityMembers,
            LinkAuthorityWebsite = _linkAuthorityWebsite,
            RemarksBaseData = _remarksBaseData,
            RemarksBaseDataAdmin = _remarksBaseDataAdmin,
            IsDeleted = _isDeleted,
            Memberships = _memberships,
            ContactPoints = _contactPoints,
            LinkHomepageGerman = _linkHomepageGerman,
            LinkHomepageFrench = _linkHomepageFrench,
            LinkHomepageItalian = _linkHomepageItalian,
            LinkHomepageRomansh = _linkHomepageRomansh,
            RowVersion = _rowVersion,
            GeneralElectionCommittees = _generalElectionCommittees,
            EiamAssignmentId = _eiamAssignmentId
        };
    }
}
