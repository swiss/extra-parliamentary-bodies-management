using Bk.APG.Business.Models;
using Bogus;

namespace Bk.APG.CrossCutting.Tests.Builders;

public class GeneralElectionCommitteeBuilder
{
    private readonly Faker _faker = new();

    private Guid _id;
    private readonly DateTime _created;
    private readonly string _createdBy;
    private readonly DateTime _modified;
    private readonly string _modifiedBy;
    private DateOnly _beginDate;
    private DateOnly? _endDate;
    private string _descriptionGerman;
    private string _descriptionFrench;
    private string _descriptionItalian;
    private string _descriptionRomansh;
    private Committee? _committee;
    private Guid _committeeId;
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
    private int _minimalMembers;
    private int _maximalMembers;
    private int _vacanciesGeneralElection;
    private readonly bool _additionalAuthorityMembers;
    private readonly string _linkAuthorityWebsite;
    private readonly string _remarksBaseData;
    private readonly string _remarksBaseDataAdmin;
    private readonly bool _isDeleted;
    private readonly List<MembershipCandidate> _membershipCandidates;
    private CandidateListState? _candidateListState;
    private Guid? _candidateListStateId;
    private readonly string? _assignedToRole;
    private bool _isValidated;
    private readonly string _selectionProcedure;
    private DateOnly? _officeReadyForProposalDueDate;
    private DateOnly? _secretariatReadyForProposalDueDate;
    private string _justificationGenders;
    private string _measuresGenders;
    private string _justificationLanguages;
    private string _measuresLanguages;

    private readonly uint _rowVersion;

    public GeneralElectionCommitteeBuilder()
    {
        _id = _faker.Random.Guid();
        _committeeId = _faker.Random.Guid();
        _committee = new CommitteeBuilder().WithId(_committeeId).Build();
        _created = _faker.Date.Past();
        _createdBy = _faker.Person.Email;
        _modified = _faker.Date.Past();
        _modifiedBy = _faker.Person.Email;
        _beginDate = _faker.Date.PastDateOnly();
        _endDate = _faker.Date.FutureDateOnly();
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
        _releaseGeneralElection = false;
        _federalLawEstablishment = false;
        _marketOrientated = false;
        _supervisionDuty = false;
        _termOfOfficeId = _faker.Random.Guid();
        _termOfOffice = new TermOfOfficeBuilder().WithId(_termOfOfficeId).Build();
        _termOfOfficeDateId = _faker.Random.Guid();
        _termOfOfficeDate = new TermOfOfficeDateBuilder().WithId(_termOfOfficeDateId).Build();
        _minimalMembers = _faker.Random.Int();
        _maximalMembers = _faker.Random.Int();
        _vacanciesGeneralElection = _faker.Random.Int();
        _additionalAuthorityMembers = false;
        _linkAuthorityWebsite = _faker.Random.String();
        _remarksBaseData = _faker.Random.String();
        _remarksBaseDataAdmin = _faker.Random.String();
        _isDeleted = false;
        _membershipCandidates = [];
        _candidateListState = null;
        _candidateListStateId = null;
        _assignedToRole = null;
        _isValidated = false;
        _selectionProcedure = _faker.Random.String();
        _rowVersion = _faker.Random.UInt();
        _officeReadyForProposalDueDate = _faker.Date.FutureDateOnly();
        _secretariatReadyForProposalDueDate = _faker.Date.FutureDateOnly();
        _justificationGenders = string.Empty;
        _measuresGenders = string.Empty;
        _justificationLanguages = string.Empty;
        _measuresLanguages = string.Empty;
    }

    public GeneralElectionCommitteeBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithBeginDate(DateOnly beginDate)
    {
        _beginDate = beginDate;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithEndDate(DateOnly? endDate)
    {
        _endDate = endDate;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithGermanDescription(string description)
    {
        _descriptionGerman = description;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithFrenchDescription(string description)
    {
        _descriptionFrench = description;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithItalianDescription(string description)
    {
        _descriptionItalian = description;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithRomanschDescription(string description)
    {
        _descriptionRomansh = description;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithCommitteeLevelId(Guid id)
    {
        _committeeLevelId = id;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithCommitteeLevel(CommitteeLevel committeeLevel)
    {
        _committeeLevel = committeeLevel;
        _committeeLevelId = committeeLevel.Id;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithCommitteeTypeId(Guid id)
    {
        _committeeTypeId = id;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithCommitteeType(CommitteeType committeeType)
    {
        _committeeType = committeeType;
        _committeeTypeId = committeeType.Id;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithTermOfOfficeId(Guid id)
    {
        _termOfOfficeId = id;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithTermOfOffice(TermOfOffice termOfOffice)
    {
        _termOfOffice = termOfOffice;
        _termOfOfficeId = termOfOffice.Id;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithTermOfOfficeDateId(Guid id)
    {
        _termOfOfficeDateId = id;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithTermOfOfficeDate(TermOfOfficeDate termOfOfficeDate)
    {
        _termOfOfficeDate = termOfOfficeDate;
        _termOfOfficeDateId = termOfOfficeDate.Id;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithDepartmentId(Guid id)
    {
        _departmentId = id;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithDepartment(Department department)
    {
        _department = department;
        _departmentId = department.Id;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithCommitteeId(Guid id)
    {
        _committeeId = id;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithCommittee(Committee committee)
    {
        _committee = committee;
        _committeeId = committee.Id;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithOfficeId(Guid id)
    {
        _officeId = id;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithOffice(Office office)
    {
        _office = office;
        _officeId = office.Id;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithMarketOrientated(bool marketOrientated)
    {
        _marketOrientated = marketOrientated;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithSupervisionDuty(bool supervisionDuty)
    {
        _supervisionDuty = supervisionDuty;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithMaximalMember(int maximalMember)
    {
        _maximalMembers = maximalMember;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithMinimalMember(int minimalMember)
    {
        _minimalMembers = minimalMember;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithVacanciesGeneralElection(int vacanciesGeneralElection)
    {
        _vacanciesGeneralElection = vacanciesGeneralElection;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithCandidateListState(CandidateListState candidateListState)
    {
        _candidateListState = candidateListState;
        _candidateListStateId = candidateListState.Id;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithCandidateListStateId(Guid candidateListStateId)
    {
        _candidateListStateId = candidateListStateId;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithMembershipCandidate(MembershipCandidate membershipCandidate)
    {
        _membershipCandidates.Add(membershipCandidate);
        return this;
    }

    public GeneralElectionCommitteeBuilder WithMembershipCandidates(IEnumerable<MembershipCandidate> membershipCandidates)
    {
        _membershipCandidates.AddRange(membershipCandidates);
        return this;
    }

    public GeneralElectionCommitteeBuilder WithIsValidated(bool isValidated)
    {
        _isValidated = isValidated;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithOfficeReadyForProposalDueDate(DateOnly? officeReadyForProposalDueDate)
    {
        _officeReadyForProposalDueDate = officeReadyForProposalDueDate;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithSecretariatReadyForProposalDueDate(DateOnly? secretariatReadyForProposalDueDate)
    {
        _secretariatReadyForProposalDueDate = secretariatReadyForProposalDueDate;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithJustificationGenders(string justificationGenders)
    {
        _justificationGenders = justificationGenders;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithMeasuresGenders(string measuresGenders)
    {
        _measuresGenders = measuresGenders;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithJustificationLanguages(string justificationLanguages)
    {
        _justificationLanguages = justificationLanguages;
        return this;
    }

    public GeneralElectionCommitteeBuilder WithMeasuresLanguages(string measuresLanguages)
    {
        _measuresLanguages = measuresLanguages;
        return this;
    }

    public GeneralElectionCommittee Build()
    {
        return new GeneralElectionCommittee
        {
            Id = _id,
            Created = _created,
            CreatedBy = _createdBy,
            Modified = _modified,
            ModifiedBy = _modifiedBy,
            BeginDate = _beginDate,
            EndDate = _endDate,
            DescriptionGerman = _descriptionGerman,
            DescriptionFrench = _descriptionFrench,
            DescriptionItalian = _descriptionItalian,
            DescriptionRomansh = _descriptionRomansh,
            CommitteeId = _committeeId,
            Committee = _committee,
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
            MembershipCandidates = _membershipCandidates,
            CandidateListState = _candidateListState,
            CandidateListStateId = _candidateListStateId,
            AssignedToRole = _assignedToRole,
            IsValidated = _isValidated,
            SelectionProcedure = _selectionProcedure,
            RowVersion = _rowVersion,
            OfficeReadyForProposalDueDate = _officeReadyForProposalDueDate,
            SecretariatReadyForProposalDueDate = _secretariatReadyForProposalDueDate,
            JustificationGenders = _justificationGenders,
            MeasuresGenders = _measuresGenders,
            JustificationLanguages = _justificationLanguages,
            MeasuresLanguages = _measuresLanguages
        };
    }
}
