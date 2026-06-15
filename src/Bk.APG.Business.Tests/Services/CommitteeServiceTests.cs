using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting;
using Bk.APG.CrossCutting.Exception;
using Bk.APG.CrossCutting.Tests.Builders;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
internal class CommitteeServiceTests
{
    private ICommitteeRepository _committeeRepository;
    private IPersonRepository _personRepository;
    private ICultureService _cultureService;
    private IAuthorizationService _authorizationService;
    private IEiamAssignmentService _eiamAssignmentService;
    private ITermOfOfficeDateService _termOfOfficeDateService;
    private IMasterDataRepository _masterDataRepository;
    private IGeneralMeasureRepository _generalMeasureRepository;
    private IMembershipRepository _membershipRepository;
    private IGeneralElectionCommitteeRepository _generalElectionCommitteeRepository;
    private IWorklistTaskService _worklistTaskService;
    private IMembershipMirrorService _membershipMirrorService;
    private IWorklistTaskRepository _worklistTaskRepository;
    private IEiamAssignmentRepository _eiamAssignmentRepository;
    private CommitteeService _committeeService;

    private List<Membership> _committeeMemberList;
    private Membership _membership1;
    private Membership _membership2;
    private Membership _membership3;
    private Membership _membership4;
    private Membership _membership5;

    private List<MembershipCandidate> _candidateListWithoutPerson;
    private List<MembershipCandidate> _candidateListWithPerson;
    private MembershipCandidate _candidate1;
    private MembershipCandidate _candidate2;
    private MembershipCandidate _candidate3;

    private EiamAssignment _eiamAssignment;

    private Committee _committee;
    private Guid _committeeId;
    private Guid _personId1;
    private Guid _personId2;
    private Guid _personId3;

    private Person _person1;
    private Person _person2;
    private Person _person3;

    private readonly Guid _zeroGuid = Guid.Empty;

    [SetUp]
    public void SetUp()
    {
        _committeeRepository = Substitute.For<ICommitteeRepository>();
        _personRepository = Substitute.For<IPersonRepository>();
        _cultureService = Substitute.For<ICultureService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _eiamAssignmentService = Substitute.For<IEiamAssignmentService>();
        _masterDataRepository = Substitute.For<IMasterDataRepository>();
        _generalMeasureRepository = Substitute.For<IGeneralMeasureRepository>();
        _membershipRepository = Substitute.For<IMembershipRepository>();
        _termOfOfficeDateService = Substitute.For<ITermOfOfficeDateService>();
        _worklistTaskService = Substitute.For<IWorklistTaskService>();
        _membershipMirrorService = Substitute.For<IMembershipMirrorService>();
        _worklistTaskRepository = Substitute.For<IWorklistTaskRepository>();
        _generalElectionCommitteeRepository = Substitute.For<IGeneralElectionCommitteeRepository>();
        _eiamAssignmentRepository = Substitute.For<IEiamAssignmentRepository>();

        _cultureService.GetCurrentUiCulture().Returns(new CultureInfo("de"));

        _committeeId = Guid.Parse("17FEBC36-0837-4AD3-AB92-0594777FBC1E");

        _personId1 = Guid.Parse("9df96395-bd65-4235-a4fa-fb87689df11f");
        _personId2 = Guid.Parse("0738EA6D-69D9-4780-9EBC-5BCD1231A573");
        _personId3 = Guid.Parse("FBEFEF07-CB51-4F6A-9911-FF1AC997554C");

        _committeeMemberList = [];
        _candidateListWithoutPerson = [];
        _candidateListWithPerson = [];

        _person1 = new PersonBuilder()
            .WithId(_personId1)
            .WithGender(new GenderBuilder().Build())
            .WithLanguage(new LanguageBuilder().Build())
            .WithLegislaturePeriods([
                new LegislaturePeriodBuilder()
                    .WithId(Guid.NewGuid())
                    .WithStartDate(new DateOnly(2020, 1, 1))
                    .WithEndDate(new DateOnly(2024, 12, 31))
                    .Build()
            ])
            .Build();

        _person2 = new PersonBuilder()
            .WithId(_personId2)
            .WithGender(new GenderBuilder().Build())
            .WithLanguage(new LanguageBuilder().Build())
            .Build();

        _person3 = new PersonBuilder()
            .WithId(_personId3)
            .WithGender(new GenderBuilder().Build())
            .WithLanguage(new LanguageBuilder().Build())
            .Build();

        _committee = new CommitteeBuilder()
            .WithId(_committeeId)
            .WithBeginDate(new DateOnly(1976, 1, 1))
            .WithEndDate(new DateOnly(2030, 12, 31))
            .WithMaximalMember(5)
            .WithDepartment(new DepartmentBuilder().Build())
            .WithGeneralElectionCommittee(new GeneralElectionCommitteeBuilder().WithIsFederalCouncilProposalDirty(true).Build())
            .Build();
        _committee.MembershipAdditionsInGeneralElection.Add(new MembershipAdditionBuilder().Build());

        _membership1 = new MembershipBuilder()
            .WithId(Guid.Parse("8697753c-f57a-4805-b1e9-bb1e546f4101"))
            .WithFunction(new FunctionBuilder().Build())
            .WithPersonId(_personId1)
            .WithPerson(_person1)
            .WithBeginDate(new DateOnly(2000, 1, 1))
            .WithEndDate(new DateOnly(2004, 12, 31))
            .WithMaximumEmploymentLevel(20)
            .Build();

        _membership2 = new MembershipBuilder()
            .WithId(Guid.Parse("0B745DA6-6B02-4053-81C9-4DE135C55CA3"))
            .WithFunction(new FunctionBuilder().Build())
            .WithPersonId(_personId1)
            .WithPerson(_person1)
            .WithBeginDate(new DateOnly(2005, 1, 1))
            .WithEndDate(new DateOnly(2009, 12, 31))
            .WithMaximumEmploymentLevel(20)
            .Build();

        _membership3 = new MembershipBuilder()
            .WithId(Guid.Parse("2F5F543E-1AE9-4B21-ACD0-B127A87D45ED"))
            .WithFunction(new FunctionBuilder().Build())
            .WithPersonId(_personId1)
            .WithPerson(_person1)
            .WithBeginDate(new DateOnly(2020, 1, 1))
            .WithEndDate(new DateOnly(2099, 12, 31))
            .WithMaximumEmploymentLevel(20)
            .Build();

        _membership4 = new MembershipBuilder()
            .WithId(Guid.Parse("ACD9B528-0FDC-442E-AA1D-73AA59B5A0E1"))
            .WithFunction(new FunctionBuilder().Build())
            .WithPersonId(_personId2)
            .WithPerson(_person2)
            .WithBeginDate(new DateOnly(2000, 1, 1))
            .WithEndDate(new DateOnly(2023, 12, 31))
            .WithMaximumEmploymentLevel(10)
            .Build();

        _membership5 = new MembershipBuilder()
            .WithId(Guid.Parse("dfd1770c-664a-4b63-9da1-f5c6008b55ed"))
            .WithFunction(new FunctionBuilder().Build())
            .WithPersonId(_personId3)
            .WithPerson(_person3)
            .WithBeginDate(new DateOnly(2000, 1, 1))
            .WithEndDate(new DateOnly(2001, 12, 31))
            .WithMaximumEmploymentLevel(10)
            .Build();

        _committee.Memberships.Add(_membership1);
        _committee.Memberships.Add(_membership2);
        _committee.Memberships.Add(_membership3);
        _committee.Memberships.Add(_membership4);
        _committee.Memberships.Add(_membership5);

        _committeeMemberList.Add(_membership1);
        _committeeMemberList.Add(_membership2);
        _committeeMemberList.Add(_membership3);
        _committeeMemberList.Add(_membership4);
        _committeeMemberList.Add(_membership5);

        _candidate1 = new MembershipCandidateBuilder()
            .WithId(Guid.Parse("3f1c2e1d-9a4b-4d6f-b8c2-5c7a2e9f1a01"))
            .WithFunction(new FunctionBuilder().Build())
            .WithElectionTypeId(ElectionType.NewElectionGuid)
            .WithBeginDate(new DateOnly(2000, 1, 1))
            .WithEndDate(new DateOnly(2004, 12, 31))
            .Build();

        _candidate2 = new MembershipCandidateBuilder()
            .WithId(Guid.Parse("a7d9c3b4-2f6e-4a91-8c5d-1e3b7f2d9c44"))
            .WithFunction(new FunctionBuilder().Build())
            .WithElectionTypeId(ElectionType.ReElectionGuid)
            .WithPersonId(_personId2)
            .WithPerson(_person2)
            .WithBeginDate(new DateOnly(2000, 1, 1))
            .WithEndDate(new DateOnly(2004, 12, 31))
            .Build();

        _candidate3 = new MembershipCandidateBuilder()
            .WithId(Guid.Parse("446c99f7-9ec4-4ac4-969b-22d3db77a627"))
            .WithFunction(new FunctionBuilder().Build())
            .WithElectionTypeId(ElectionType.NewElectionGuid)
            .WithPersonId(_personId3)
            .WithPerson(_person3)
            .WithBeginDate(new DateOnly(2000, 1, 1))
            .WithEndDate(new DateOnly(2004, 12, 31))
            .Build();

        _candidateListWithPerson.Add(_candidate2);
        _candidateListWithPerson.Add(_candidate3);
        _candidateListWithoutPerson.Add(_candidate1);

        _eiamAssignment = new EiamAssignmentBuilder().Build();

        _eiamAssignmentRepository.Create(Arg.Any<EiamAssignment>()).Returns(_eiamAssignment);

        _membershipRepository.GetAllByCommitteeId(_committeeId).Returns(_committeeMemberList);

        _authorizationService.IsAdmin.Returns(false);

        _personRepository.GetById(_personId1).Returns(_person1);
        _personRepository.GetById(_personId2).Returns(_person2);
        _personRepository.GetById(_personId3).Returns(_person3);

        _committeeRepository.GetById(_committeeId).Returns(_committee);
        _committeeRepository.GetByIdForUpdate(_committeeId).Returns(_committee);

        _committeeRepository.GetByIdForUpdate(_committee.Id, _committee.RowVersion).Returns(_committee);
        _committeeRepository.GetAllForGeneralElection(_zeroGuid, _zeroGuid, _zeroGuid).Returns(new List<Committee>().Append(_committee));
        _committeeRepository.GetAllForExport(_zeroGuid, _zeroGuid, _zeroGuid, Arg.Any<ReportFilterParametersDto>()).Returns(new List<Committee>().Append(_committee));
        _committeeRepository.GetByFilterForReport(_zeroGuid, _zeroGuid, _zeroGuid, Arg.Any<ReportFilterParametersDto>(), Arg.Any<DateOnly>()).Returns(new List<Committee>().Append(_committee));

        _committeeService = new CommitteeService(
            _committeeRepository,
            _personRepository,
            _cultureService,
            _authorizationService,
            _eiamAssignmentService,
            _termOfOfficeDateService,
            _worklistTaskService,
            _membershipMirrorService,
            _masterDataRepository,
            _generalMeasureRepository,
            _membershipRepository,
            _generalElectionCommitteeRepository,
            _worklistTaskRepository,
            _eiamAssignmentRepository,
            NullLogger<CommitteeService>.Instance);
    }

    [TearDown]
    public void TearDown()
    {
        _committeeRepository.ClearSubstitute();
        _cultureService.ClearSubstitute();
        _authorizationService.ClearSubstitute();
        _personRepository.ClearSubstitute();
        _masterDataRepository.ClearSubstitute();
        _generalMeasureRepository.ClearSubstitute();
        _membershipRepository.ClearSubstitute();
        _membershipMirrorService.ClearSubstitute();
    }

    [Test]
    public async Task GetCommitteeList_ShouldReturnCommitteeList()
    {
        var pagingDto = new PagingParametersDto { PageIndex = 42, PageSize = 2 };
        const string sortKey = "description";
        const SortDirection sortDirection = SortDirection.Asc;
        var resultFromRepository = new PagedResult<Committee>
        {
            Total = 276,
            Index = 42,
            Items =
            [
                new CommitteeBuilder()
                    .Build()
            ]
        };
        _committeeRepository
            .GetAll(Arg.Any<PagingParameters>(), null, Arg.Any<string?>(), Arg.Any<SortDirection?>())
            .Returns(resultFromRepository);

        var committees = await _committeeService.GetCommitteeList(pagingDto, null, sortKey, sortDirection);

        await _committeeRepository.Received(1).GetAll(
            Arg.Is<PagingParameters>(p => p.PageIndex == pagingDto.PageIndex && p.PageSize == pagingDto.PageSize),
            null,
            Arg.Is(sortKey),
            Arg.Is(sortDirection));

        Assert.That(committees, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(committees.Index, Is.EqualTo(resultFromRepository.Index));
            Assert.That(committees.Total, Is.EqualTo(resultFromRepository.Total));
            Assert.That(committees.Items.Count(), Is.EqualTo(resultFromRepository.Items.Count()));
        });
    }

    [Test]
    public async Task GetCommitteeListForVacanciesExport_ShouldReturnData()
    {
        var filterDto = new ReportFilterParametersDto
        {
            DocumentType = ReportType.Vacancies,
            DepartmentIds = new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid()
            }
        };

        var committees = await _committeeService.GetCommitteeListForExport(filterDto);

        await _committeeRepository.Received(1).GetAllForExport(_zeroGuid, _zeroGuid, _zeroGuid, Arg.Any<ReportFilterParametersDto>());

        Assert.That(committees, Is.Not.Null);
        Assert.That(committees.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetCommitteeListForCompareListExport_ShouldReturnData()
    {
        var filterDto = new ReportFilterParametersDto
        {
            DocumentType = ReportType.CompareListGeneralElection,
            AnalysisDate1 = new DateOnly(2024, 01, 01),
            AnalysisDate2 = new DateOnly(2026, 04, 01),
            DepartmentIds = new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid()
            }
        };

        var committees = await _committeeService.GetCommitteeListForExport(filterDto);

        await _committeeRepository.Received(2).GetByFilterForReport(_zeroGuid, _zeroGuid, _zeroGuid, Arg.Any<ReportFilterParametersDto>(), Arg.Any<DateOnly>());

        Assert.That(committees, Is.Not.Null);
        Assert.That(committees.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetCommitteesForGeneralElection_ShouldReturnData()
    {
        var committees = await _committeeService.GetCommitteesForGeneralElection();

        await _committeeRepository.Received(1).GetAllForGeneralElection(_zeroGuid, _zeroGuid, _zeroGuid);

        Assert.That(committees, Is.Not.Null);
        Assert.That(committees.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetCommitteeDetail_ForAdmin_ShouldReturnDto()
    {
        _authorizationService.HasAccessToCommittee(Arg.Any<Committee>()).Returns(true);

        var committeeDetail = await _committeeService.GetCommitteeDetail(_committee.Id);

        await _committeeRepository.Received(1).GetById(Arg.Any<Guid>());

        Assert.That(committeeDetail, Is.Not.Null);
        Assert.That(committeeDetail.CanEdit, Is.True);
    }

    [Test]
    public async Task GetCommitteeDetail_InGeneralElection_ShouldReturnDtoWithGeneralElectionFields()
    {
        var assignmentId = Guid.NewGuid();
        _authorizationService.HasAccessToCommittee(Arg.Any<Committee>()).Returns(true);
        _termOfOfficeDateService.CheckForRunningGeneralElection().Returns(true);

        var eiamAssignment = new EiamAssignment() { Id = assignmentId, ExternalId = "111", Role = Role.Department };

        _authorizationService.GetCurrentEiamAssignment().Returns(Task.FromResult(eiamAssignment));
        _worklistTaskRepository.GetAllByGeneralElectionCommitteeId(Arg.Any<Guid>())
            .Returns(Task.FromResult<IEnumerable<WorklistTask>>(new List<WorklistTask>() {
                new WorklistTaskBuilder()
                .WithAssignedTo(eiamAssignment)
                .WithWorklistTaskTypeId(WorklistTaskType.ReadyForFederalCouncilProposal)
                .WithWorklistTaskStateId(WorklistTaskState.Completed)
                .Build()
        }));

        var committeeDetail = await _committeeService.GetCommitteeDetail(_committee.Id, true);

        await _committeeRepository.Received(1).GetById(Arg.Any<Guid>());

        Assert.That(committeeDetail, Is.Not.Null);
        Assert.That(committeeDetail.CanEdit, Is.True);
        Assert.That(committeeDetail.IsFederalCouncilProposalDirty, Is.True);
        Assert.That(committeeDetail.IsReadyForProposalForCurrentRole, Is.True);
    }

    [Test]
    public async Task GetCommitteeForUpdate_ForAdmin_ShouldReturnDtoWithEditAllPermission()
    {
        _authorizationService.IsAdmin.Returns(true);
        _authorizationService.HasAccessToCommittee(Arg.Any<Committee>()).Returns(true);

        var committeeUpdateDto = await _committeeService.GetCommitteeForUpdate(_committee.Id);

        await _committeeRepository.Received(1).GetByIdForUpdate(Arg.Any<Guid>());

        Assert.That(committeeUpdateDto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(committeeUpdateDto.CanEditAll, Is.True);
            Assert.That(committeeUpdateDto.CanEditDepartment, Is.True);
            Assert.That(committeeUpdateDto.CanEditLegalbase, Is.True);
        });
    }

    [Test]
    public async Task GetCommitteeForUpdate_ForDepartment_WithoutEndDate_ShouldReturnDtoWithEditAllPermission()
    {
        _authorizationService.IsDepartment.Returns(true);
        _authorizationService.GetDepartment().Returns(new DepartmentBuilder().WithId(_committee.DepartmentId).Build());
        _committee.EndDate = null;

        var committeeUpdateDto = await _committeeService.GetCommitteeForUpdate(_committee.Id);

        Assert.That(committeeUpdateDto, Is.Not.Null);
        Assert.That(committeeUpdateDto.CanEditAll, Is.True);
    }

    [Test]
    public async Task GetCommitteeForUpdate_ForDepartment_WithEndDateToday_ShouldReturnDtoWithEditAllPermission()
    {
        _authorizationService.IsDepartment.Returns(true);
        _authorizationService.GetDepartment().Returns(new DepartmentBuilder().WithId(_committee.DepartmentId).Build());
        _committee.EndDate = DateOnly.FromDateTime(DateTime.Today);

        var committeeUpdateDto = await _committeeService.GetCommitteeForUpdate(_committee.Id);

        Assert.That(committeeUpdateDto, Is.Not.Null);
        Assert.That(committeeUpdateDto.CanEditAll, Is.True);
    }

    [Test]
    public async Task GetCommitteeForUpdate_ForDepartment_WithEndDateInThePast_ShouldReturnDtoWithoutEditAllPermission()
    {
        _authorizationService.IsDepartment.Returns(true);
        _authorizationService.GetDepartment().Returns(new DepartmentBuilder().WithId(_committee.DepartmentId).Build());
        _committee.EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

        var committeeUpdateDto = await _committeeService.GetCommitteeForUpdate(_committee.Id);

        Assert.That(committeeUpdateDto, Is.Not.Null);
        Assert.That(committeeUpdateDto.CanEditAll, Is.False);
    }

    [Test]
    public async Task GetCommitteeJustificationForUpdate_ShouldReturnDto()
    {
        _committeeRepository.GetByIdForUpdate(Arg.Any<Guid>()).Returns(_committee);

        var committeeJustificationUpdateDto = await _committeeService.GetCommitteeJustificationForUpdate(Guid.NewGuid());

        await _committeeRepository.Received(1).GetByIdForUpdate(Arg.Any<Guid>());

        Assert.That(committeeJustificationUpdateDto, Is.Not.Null);
    }

    [Test]
    public async Task UpdateCommittee_WithAdminRole_ShouldUpdatePropertiesAndCommitChanges()
    {
        _authorizationService.IsAdmin.Returns(true);

        var updateDto = BuildUpdateDto();
        _committeeRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_committee);
        _committeeRepository.GetById(updateDto.Id).Returns(_committee);

        var membershipAddition = new MembershipAdditionBuilder().Build();
        _masterDataRepository.GetById<MembershipAddition>(updateDto.MembershipAdditionsInGeneralElection![0]).Returns(membershipAddition);

        await _committeeService.UpdateCommittee(updateDto.Id, updateDto, true);

        await _committeeRepository.Received(1).GetByIdForUpdate(updateDto.Id, updateDto.RowVersion);

        Assert.Multiple(() =>
        {
            Assert.That(_committee.DescriptionRomansh, Is.EqualTo(updateDto.DescriptionRomansh));

            Assert.That(_committee.CommitteeLevelId, Is.EqualTo(updateDto.LevelId));
            Assert.That(_committee.OfficeId, Is.EqualTo(updateDto.OfficeId));
            Assert.That(_committee.DepartmentId, Is.EqualTo(updateDto.DepartmentId));
            Assert.That(_committee.CommitteeTypeId, Is.EqualTo(updateDto.CommitteeTypeId));

            Assert.That(_committee.FederalLawEstablishment, Is.EqualTo(updateDto.FederalLawEstablishment));
            Assert.That(_committee.SupervisionDuty, Is.EqualTo(updateDto.SupervisionDuty));
            Assert.That(_committee.MarketOrientated, Is.EqualTo(updateDto.MarketOrientated));

            Assert.That(_committee.LegalFormId, Is.EqualTo(updateDto.LegalFormId));
            Assert.That(_committee.LegalBase, Is.EqualTo(updateDto.LegalBase));

            Assert.That(_committee.TermOfOfficeId, Is.EqualTo(updateDto.TermOfOfficeId));
            Assert.That(_committee.MinimalMembers, Is.EqualTo(updateDto.MinimalMembers));
            Assert.That(_committee.MaximalMembers, Is.EqualTo(updateDto.MaximalMembers));
            Assert.That(_committee.AdditionalAuthorityMembers, Is.EqualTo(updateDto.AdditionalAuthorityMembers));
            Assert.That(_committee.LinkAuthorityWebsite, Is.EqualTo(updateDto.LinkAuthorityWebsite));
            Assert.That(_committee.LinkHomepageGerman, Is.EqualTo(updateDto.LinkHomepageGerman));
            Assert.That(_committee.LinkHomepageFrench, Is.EqualTo(updateDto.LinkHomepageFrench));
            Assert.That(_committee.LinkHomepageItalian, Is.EqualTo(updateDto.LinkHomepageItalian));
            Assert.That(_committee.LinkHomepageRomansh, Is.EqualTo(updateDto.LinkHomepageRomansh));

            Assert.That(_committee.VacanciesGeneralElection, Is.EqualTo(updateDto.VacanciesInGeneralElection));
            Assert.That(_committee.MembershipAdditionsInGeneralElection, Has.Count.EqualTo(1));
            Assert.That(_committee.MembershipAdditionsInGeneralElection.First(), Is.EqualTo(membershipAddition));

            Assert.That(_committee.EndDate, Is.EqualTo(updateDto.EndDate));
        });
    }

    [Test]
    public async Task UpdateCommittee_WithDepartmentRole_ShouldUpdatePropertiesAndCommitChanges()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(true);

        var updateDto = BuildUpdateDto();
        _committeeRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_committee);
        _committeeRepository.GetById(updateDto.Id).Returns(_committee);
        _authorizationService.GetDepartment().Returns(new DepartmentBuilder().WithId(_committee.DepartmentId).Build());

        await _committeeService.UpdateCommittee(updateDto.Id, updateDto, true);

        await _committeeRepository.Received(1).GetByIdForUpdate(updateDto.Id, updateDto.RowVersion);

        Assert.Multiple(() =>
        {
            Assert.That(_committee.DescriptionRomansh, Is.EqualTo(updateDto.DescriptionRomansh));

            Assert.That(_committee.CommitteeLevelId, Is.EqualTo(updateDto.LevelId));
            Assert.That(_committee.OfficeId, Is.EqualTo(updateDto.OfficeId));
            Assert.That(_committee.DepartmentId, Is.EqualTo(updateDto.DepartmentId));
            Assert.That(_committee.CommitteeTypeId, Is.EqualTo(updateDto.CommitteeTypeId));

            Assert.That(_committee.FederalLawEstablishment, Is.EqualTo(updateDto.FederalLawEstablishment));
            Assert.That(_committee.SupervisionDuty, Is.EqualTo(updateDto.SupervisionDuty));
            Assert.That(_committee.MarketOrientated, Is.EqualTo(updateDto.MarketOrientated));

            Assert.That(_committee.LegalFormId, Is.EqualTo(updateDto.LegalFormId));
            Assert.That(_committee.LegalBase, Is.EqualTo(updateDto.LegalBase));

            Assert.That(_committee.TermOfOfficeId, Is.EqualTo(updateDto.TermOfOfficeId));
            Assert.That(_committee.MinimalMembers, Is.EqualTo(updateDto.MinimalMembers));
            Assert.That(_committee.MaximalMembers, Is.EqualTo(updateDto.MaximalMembers));
            Assert.That(_committee.AdditionalAuthorityMembers, Is.EqualTo(updateDto.AdditionalAuthorityMembers));
            Assert.That(_committee.LinkAuthorityWebsite, Is.EqualTo(updateDto.LinkAuthorityWebsite));
            Assert.That(_committee.LinkHomepageGerman, Is.EqualTo(updateDto.LinkHomepageGerman));
            Assert.That(_committee.LinkHomepageFrench, Is.EqualTo(updateDto.LinkHomepageFrench));
            Assert.That(_committee.LinkHomepageItalian, Is.EqualTo(updateDto.LinkHomepageItalian));
            Assert.That(_committee.LinkHomepageRomansh, Is.EqualTo(updateDto.LinkHomepageRomansh));

            Assert.That(_committee.EndDate, Is.EqualTo(updateDto.EndDate));
        });
    }

    [Test]
    public async Task UpdateCommittee_WithCommitteeNotInDepartment_ShouldThrowAuthorizationException()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(true);

        var updateDto = BuildUpdateDto();
        _committeeRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_committee);
        _committeeRepository.GetById(updateDto.Id).Returns(_committee);
        _authorizationService.GetDepartment().Returns(new DepartmentBuilder().Build());

        Assert.That(async () => await _committeeService.UpdateCommittee(updateDto.Id, updateDto, true), Throws.Exception.InstanceOf<AuthorizationException>());

        await _committeeRepository.Received(1).GetById(updateDto.Id);
    }

    [TestCase(true, false)]
    [TestCase(false, true)]
    public async Task UpdateCommittee_ForOfficeOrSecretariatRoleInOwnCommittee_ShouldUpdatePropertiesAndCommitChanges(bool isOffice, bool isSecretariat)
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(false);
        _authorizationService.IsOffice.Returns(isOffice);
        _authorizationService.IsSecretariat.Returns(isSecretariat);

        var updateDto = BuildUpdateDto();
        _committeeRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_committee);
        _committeeRepository.GetById(updateDto.Id).Returns(_committee);
        _authorizationService.IsCommitteeAssigned(_committeeId).Returns(true);

        await _committeeService.UpdateCommittee(updateDto.Id, updateDto, true);

        await _committeeRepository.Received(1).GetByIdForUpdate(updateDto.Id, updateDto.RowVersion);

        Assert.Multiple(() =>
        {
            Assert.That(_committee.DescriptionRomansh, Is.EqualTo(updateDto.DescriptionRomansh));

            Assert.That(_committee.CommitteeLevelId, Is.EqualTo(updateDto.LevelId));
            Assert.That(_committee.OfficeId, Is.EqualTo(updateDto.OfficeId));
            Assert.That(_committee.DepartmentId, Is.EqualTo(updateDto.DepartmentId));
            Assert.That(_committee.CommitteeTypeId, Is.EqualTo(updateDto.CommitteeTypeId));

            Assert.That(_committee.FederalLawEstablishment, Is.EqualTo(updateDto.FederalLawEstablishment));
            Assert.That(_committee.SupervisionDuty, Is.EqualTo(updateDto.SupervisionDuty));
            Assert.That(_committee.MarketOrientated, Is.EqualTo(updateDto.MarketOrientated));

            Assert.That(_committee.LegalFormId, Is.EqualTo(updateDto.LegalFormId));
            Assert.That(_committee.LegalBase, Is.EqualTo(updateDto.LegalBase));

            Assert.That(_committee.TermOfOfficeId, Is.EqualTo(updateDto.TermOfOfficeId));
            Assert.That(_committee.MinimalMembers, Is.EqualTo(updateDto.MinimalMembers));
            Assert.That(_committee.MaximalMembers, Is.EqualTo(updateDto.MaximalMembers));
            Assert.That(_committee.AdditionalAuthorityMembers, Is.EqualTo(updateDto.AdditionalAuthorityMembers));
            Assert.That(_committee.LinkAuthorityWebsite, Is.EqualTo(updateDto.LinkAuthorityWebsite));
            Assert.That(_committee.LinkHomepageGerman, Is.EqualTo(updateDto.LinkHomepageGerman));
            Assert.That(_committee.LinkHomepageFrench, Is.EqualTo(updateDto.LinkHomepageFrench));
            Assert.That(_committee.LinkHomepageItalian, Is.EqualTo(updateDto.LinkHomepageItalian));
            Assert.That(_committee.LinkHomepageRomansh, Is.EqualTo(updateDto.LinkHomepageRomansh));
            Assert.That(_committee.EndDate, Is.EqualTo(updateDto.EndDate));
        });
    }

    [TestCase(true, false)]
    [TestCase(false, true)]
    public async Task UpdateCommittee_ForOfficeOrSecretariatRoleButCommitteeNotAllowed_ShouldThrowAuthorizationException(bool isOffice, bool isSecretariat)
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(false);
        _authorizationService.IsOffice.Returns(isOffice);
        _authorizationService.IsSecretariat.Returns(isSecretariat);

        var updateDto = BuildUpdateDto();
        _committeeRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_committee);
        _committeeRepository.GetById(updateDto.Id).Returns(_committee);
        _authorizationService.IsCommitteeAssigned(_committeeId).Returns(false);

        Assert.That(async () => await _committeeService.UpdateCommittee(updateDto.Id, updateDto, true), Throws.Exception.InstanceOf<AuthorizationException>());

        await _committeeRepository.Received(1).GetById(updateDto.Id);
    }

    [Test]
    public async Task UpdateCommittee_WithObserver_ShouldThrowAuthorizationException()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(false);
        _authorizationService.IsOffice.Returns(false);
        _authorizationService.IsSecretariat.Returns(false);
        _authorizationService.IsObserver.Returns(true);

        var updateDto = BuildUpdateDto();
        _committeeRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_committee);
        _committeeRepository.GetById(updateDto.Id).Returns(_committee);
        _authorizationService.IsCommitteeAssigned(_committeeId).Returns(false);

        Assert.That(async () => await _committeeService.UpdateCommittee(updateDto.Id, updateDto, true), Throws.Exception.InstanceOf<AuthorizationException>());

        await _committeeRepository.Received(1).GetById(updateDto.Id);
    }

    [TestCase(true, false, false, false)]
    [TestCase(false, true, false, false)]
    [TestCase(false, false, true, false)]
    [TestCase(false, false, false, true)]
    public async Task UpdateCommittee_WithInactiveCommitteeAnNonAdminRole_ShouldThrowAuthorizationException(bool isDepartment, bool isOffice, bool isSecretariat, bool isObserver)
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsOffice.Returns(isOffice);
        _authorizationService.IsSecretariat.Returns(isSecretariat);
        _authorizationService.IsObserver.Returns(isObserver);

        var committee = new CommitteeBuilder()
           .WithId(_committeeId)
           .WithBeginDate(new DateOnly(2028, 1, 1))
           .WithEndDate(new DateOnly(2030, 12, 31))
            .WithMaximalMember(5)
            .WithDepartment(new DepartmentBuilder().Build())
           .Build();

        var updateDto = BuildUpdateDto();
        _committeeRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(committee);
        _committeeRepository.GetById(updateDto.Id).Returns(_committee);

        Assert.That(async () => await _committeeService.UpdateCommittee(updateDto.Id, updateDto, true), Throws.Exception.InstanceOf<AuthorizationException>());

        await _committeeRepository.Received(1).GetById(updateDto.Id);
    }

    [Test]
    public async Task UpdateCommitteeJustification_ForAdmin_ShouldUpdatePropertiesAndCommitChanges()
    {
        _authorizationService.IsAdmin.Returns(true);

        var updateDto = BuildJustificationUpdateDto();
        _committeeRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_committee);
        _committeeRepository.GetById(updateDto.Id).Returns(_committee);

        await _committeeService.UpdateCommitteeJustifications(updateDto.Id, updateDto);

        await _committeeRepository.Received(1).GetByIdForUpdate(updateDto.Id, updateDto.RowVersion);
        await _committeeRepository.Received(1).CommitChanges();
        Assert.Multiple(() =>
        {
            Assert.That(_committee.JustificationMembers, Is.EqualTo(updateDto.JustificationMembers));
            Assert.That(_committee.JustificationGenders, Is.EqualTo(updateDto.JustificationGenders));
            Assert.That(_committee.MeasuresGenders, Is.EqualTo(updateDto.MeasuresGenders));
            Assert.That(_committee.JustificationLanguages, Is.EqualTo(updateDto.JustificationLanguages));
            Assert.That(_committee.MeasuresLanguages, Is.EqualTo(updateDto.MeasuresLanguages));
        });
    }

    [Test]
    public async Task UpdateCommitteeJustification_ForDepartmentInOwnDepartment_ShouldUpdatePropertiesAndCommitChanges()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(true);

        var updateDto = BuildJustificationUpdateDto();
        _committeeRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_committee);
        _committeeRepository.GetById(updateDto.Id).Returns(_committee);
        _authorizationService.GetDepartment().Returns(new DepartmentBuilder().WithId(_committee.DepartmentId).Build());

        await _committeeService.UpdateCommitteeJustifications(updateDto.Id, updateDto);

        await _committeeRepository.Received(1).GetByIdForUpdate(updateDto.Id, updateDto.RowVersion);
        await _committeeRepository.Received(1).CommitChanges();
        Assert.Multiple(() =>
        {
            Assert.That(_committee.JustificationMembers, Is.EqualTo(updateDto.JustificationMembers));
            Assert.That(_committee.JustificationGenders, Is.EqualTo(updateDto.JustificationGenders));
            Assert.That(_committee.MeasuresGenders, Is.EqualTo(updateDto.MeasuresGenders));
            Assert.That(_committee.JustificationLanguages, Is.EqualTo(updateDto.JustificationLanguages));
            Assert.That(_committee.MeasuresLanguages, Is.EqualTo(updateDto.MeasuresLanguages));
        });
    }

    [Test]
    public async Task UpdateCommitteeJustification_ForCommitteeNotInOwnDepartment_ShouldThrowAuthorizationException()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(true);

        var updateDto = BuildJustificationUpdateDto();
        _committeeRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_committee);
        _committeeRepository.GetById(updateDto.Id).Returns(_committee);
        _authorizationService.GetDepartment().Returns(new DepartmentBuilder().Build());

        Assert.That(async () => await _committeeService.UpdateCommitteeJustifications(updateDto.Id, updateDto), Throws.Exception.InstanceOf<AuthorizationException>());

        await _committeeRepository.Received(1).GetByIdForUpdate(updateDto.Id, updateDto.RowVersion);
        await _committeeRepository.DidNotReceiveWithAnyArgs().CommitChanges();
    }

    [TestCase(true, false)]
    [TestCase(false, true)]
    public async Task UpdateCommitteeJustification_ForOfficeOrSecretariat_ShouldUpdatePropertiesAndCommitChanges(bool isOffice, bool isSecretariat)
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(false);
        _authorizationService.IsOffice.Returns(isOffice);
        _authorizationService.IsSecretariat.Returns(isSecretariat);

        var updateDto = BuildJustificationUpdateDto();
        _committeeRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_committee);
        _committeeRepository.GetById(updateDto.Id).Returns(_committee);
        _authorizationService.IsCommitteeAssigned(_committeeId).Returns(true);

        await _committeeService.UpdateCommitteeJustifications(updateDto.Id, updateDto);

        await _committeeRepository.Received(1).GetByIdForUpdate(updateDto.Id, updateDto.RowVersion);
        await _committeeRepository.Received(1).CommitChanges();
        Assert.Multiple(() =>
        {
            Assert.That(_committee.JustificationMembers, Is.EqualTo(updateDto.JustificationMembers));
            Assert.That(_committee.JustificationGenders, Is.EqualTo(updateDto.JustificationGenders));
            Assert.That(_committee.MeasuresGenders, Is.EqualTo(updateDto.MeasuresGenders));
            Assert.That(_committee.JustificationLanguages, Is.EqualTo(updateDto.JustificationLanguages));
            Assert.That(_committee.MeasuresLanguages, Is.EqualTo(updateDto.MeasuresLanguages));
        });
    }

    [TestCase(true, false)]
    [TestCase(false, true)]
    public async Task UpdateCommitteeJustification_ForOfficeOrSecretariatWithoutPermissionForCommittee_ShouldThrowAuthorizationException(bool isOffice, bool isSecretariat)
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(false);
        _authorizationService.IsOffice.Returns(isOffice);
        _authorizationService.IsSecretariat.Returns(isSecretariat);

        var updateDto = BuildJustificationUpdateDto();
        _committeeRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_committee);
        _committeeRepository.GetById(updateDto.Id).Returns(_committee);
        _authorizationService.IsCommitteeAssigned(_committeeId).Returns(false);

        Assert.That(async () => await _committeeService.UpdateCommitteeJustifications(updateDto.Id, updateDto), Throws.Exception.InstanceOf<AuthorizationException>());

        await _committeeRepository.Received(1).GetByIdForUpdate(updateDto.Id, updateDto.RowVersion);
        await _committeeRepository.DidNotReceiveWithAnyArgs().CommitChanges();
    }

    [Test]
    public async Task UpdateCommitteeJustification_ForObserver_ShouldThrowAuthorizationException()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsObserver.Returns(true);

        var updateDto = BuildJustificationUpdateDto();
        _committeeRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_committee);
        _committeeRepository.GetById(updateDto.Id).Returns(_committee);

        Assert.That(async () => await _committeeService.UpdateCommitteeJustifications(updateDto.Id, updateDto), Throws.Exception.InstanceOf<AuthorizationException>());

        await _committeeRepository.Received(1).GetByIdForUpdate(updateDto.Id, updateDto.RowVersion);
        await _committeeRepository.DidNotReceiveWithAnyArgs().CommitChanges();
    }

    [TestCase(true, false, false, false)]
    [TestCase(false, true, false, false)]
    [TestCase(false, false, true, false)]
    [TestCase(false, false, false, true)]
    public async Task UpdateCommitteeJustification_WithInactiveCommitteeAndNonAdminRole_ShouldThrowAuthorizationException(bool isDepartment, bool isOffice, bool isSecretariat, bool isObserver)
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(isDepartment);
        _authorizationService.IsOffice.Returns(isObserver);
        _authorizationService.IsSecretariat.Returns(isSecretariat);
        _authorizationService.IsObserver.Returns(isObserver);

        var committee = new CommitteeBuilder()
           .WithId(_committeeId)
           .WithBeginDate(new DateOnly(2028, 1, 1))
           .WithEndDate(new DateOnly(2030, 12, 31))
            .WithMaximalMember(5)
            .WithDepartment(new DepartmentBuilder().Build())
           .Build();

        var updateDto = BuildJustificationUpdateDto();
        _committeeRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(committee);
        _committeeRepository.GetById(updateDto.Id).Returns(committee);

        Assert.That(async () => await _committeeService.UpdateCommitteeJustifications(updateDto.Id, updateDto), Throws.Exception.InstanceOf<AuthorizationException>());

        await _committeeRepository.Received(1).GetByIdForUpdate(updateDto.Id, updateDto.RowVersion);
        await _committeeRepository.DidNotReceiveWithAnyArgs().CommitChanges();
    }

    [Test]
    public async Task GetByDescription_ShouldCallRepository()
    {
        var committees = new List<Committee>
        {
            _committee
        };

        _committeeRepository.GetByDescription(Arg.Any<string>()).Returns(committees);

        var dtos = (await _committeeService.GetByDescription("test")).ToList();

        Assert.That(dtos, Is.Not.Null);
        Assert.That(dtos, Has.Count.EqualTo(committees.Count));
    }

    [Test]
    public async Task GetByDescription_WhenCalledWithNonPermittedDepartment_ShouldReduceResult()
    {
        var committee2Id = Guid.NewGuid();
        var committee2 = new CommitteeBuilder()
            .WithId(committee2Id)
            .WithBeginDate(new DateOnly(1976, 1, 1))
            .WithEndDate(new DateOnly(2030, 12, 31))
            .Build();

        var committees = new List<Committee>();

        committees.AddRange([_committee, committee2]);

        _committeeRepository.GetByDescription(Arg.Any<string>()).Returns(committees);
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(true);
        _authorizationService.IsCommitteeAssigned(_committeeId).Returns(false);
        _authorizationService.IsCommitteeAssigned(committee2Id).Returns(true);

        var dtos = (await _committeeService.GetByDescription("test")).ToList();

        Assert.That(dtos, Is.Not.Null);
        Assert.That(dtos, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task CreateCommittee_WithAdmin_ShouldCallRepository()
    {
        _authorizationService.IsAdmin.Returns(true);

        var membershipAddition = new MembershipAdditionBuilder().Build();
        _masterDataRepository.GetMembershipAdditionsByIds(Arg.Is<Guid[]>(x => x.Single() == membershipAddition.Id)).Returns([membershipAddition]);

        var departmentId = Guid.NewGuid();
        var beginDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-5));

        var createDto = new CommitteeCreateDto
        {
            DescriptionGerman = "foo",
            DescriptionFrench = "foo",
            DescriptionItalian = "foo",
            DescriptionRomansh = "foo",
            LevelId = Guid.NewGuid(),
            OfficeId = Guid.NewGuid(),
            DepartmentId = departmentId,
            CommitteeTypeId = Guid.NewGuid(),
            FederalLawEstablishment = true,
            SupervisionDuty = true,
            MarketOrientated = true,
            LegalFormId = Guid.NewGuid(),
            LegalBase = "foo",
            TermOfOfficeId = Guid.NewGuid(),
            TermOfOfficeDateId = Guid.NewGuid(),
            MinimalMembers = 1,
            MaximalMembers = 2,
            AdditionalAuthorityMembers = true,
            LinkAuthorityWebsite = "foo",
            BeginDate = beginDate,
            EndDate = DateOnly.FromDateTime(DateTime.Now),
            MembershipAdditionsInGeneralElection = [membershipAddition.Id]
        };
        _committeeRepository.GetById(Arg.Any<Guid>()).Returns(_committee);
        _committeeRepository.Create(Arg.Any<Committee>()).Returns(_committee);

        await _committeeService.CreateCommittee(createDto);

        _authorizationService.Received(2).GetCurrentUserName();
        await _committeeRepository.Received(1).Create(Arg.Is<Committee>(x => x.BeginDate == beginDate));
        await _committeeRepository.Received(1).GetByIdForUpdate(Arg.Any<Guid>());
        await _eiamAssignmentRepository.Received(1).Create(Arg.Is<EiamAssignment>(x => x.CommitteeId == _committeeId));
        await _masterDataRepository.Received(1).GetMembershipAdditionsByIds(Arg.Is<Guid[]>(x => x.Single() == membershipAddition.Id));
        _masterDataRepository.Received(1).AttachUnchanged(membershipAddition);
    }

    [Test]
    public async Task CreateCommittee_ForDepartmentWithCommitteeInOwnDepartment_ShouldCallRepository()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(true);

        var beginDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-5));

        var createDto = new CommitteeCreateDto
        {
            DescriptionGerman = "foo",
            DescriptionFrench = "foo",
            DescriptionItalian = "foo",
            DescriptionRomansh = "foo",
            LevelId = Guid.NewGuid(),
            OfficeId = Guid.NewGuid(),
            DepartmentId = _committee.DepartmentId,
            CommitteeTypeId = Guid.NewGuid(),
            FederalLawEstablishment = true,
            SupervisionDuty = true,
            MarketOrientated = true,
            LegalFormId = Guid.NewGuid(),
            LegalBase = "foo",
            TermOfOfficeId = Guid.NewGuid(),
            TermOfOfficeDateId = Guid.NewGuid(),
            MinimalMembers = 1,
            MaximalMembers = 2,
            AdditionalAuthorityMembers = true,
            LinkAuthorityWebsite = "foo",
            BeginDate = beginDate,
            EndDate = DateOnly.FromDateTime(DateTime.Now)
        };
        _committeeRepository.GetById(Arg.Any<Guid>()).Returns(_committee);
        _committeeRepository.Create(Arg.Any<Committee>()).Returns(_committee);
        _authorizationService.GetDepartment().Returns(_committee.Department);

        await _committeeService.CreateCommittee(createDto);

        _authorizationService.Received(2).GetCurrentUserName();
        await _committeeRepository.Received(1).Create(Arg.Is<Committee>(x => x.BeginDate == beginDate));
    }

    [Test]
    public async Task CreateCommittee_ForDepartmentRoleButCreateInOtherDepartment_ShouldThrowAuthorizationException()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(true);

        var departmentId = Guid.NewGuid();
        var beginDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-5));

        var createDto = new CommitteeCreateDto
        {
            DescriptionGerman = "foo",
            DescriptionFrench = "foo",
            DescriptionItalian = "foo",
            DescriptionRomansh = "foo",
            LevelId = Guid.NewGuid(),
            OfficeId = Guid.NewGuid(),
            DepartmentId = departmentId,
            CommitteeTypeId = Guid.NewGuid(),
            FederalLawEstablishment = true,
            SupervisionDuty = true,
            MarketOrientated = true,
            LegalFormId = Guid.NewGuid(),
            LegalBase = "foo",
            TermOfOfficeId = Guid.NewGuid(),
            TermOfOfficeDateId = Guid.NewGuid(),
            MinimalMembers = 1,
            MaximalMembers = 2,
            AdditionalAuthorityMembers = true,
            LinkAuthorityWebsite = "foo",
            BeginDate = beginDate,
            EndDate = DateOnly.FromDateTime(DateTime.Now)
        };

        Assert.That(async () => await _committeeService.CreateCommittee(createDto), Throws.Exception.InstanceOf<AuthorizationException>());

        await _committeeRepository.DidNotReceiveWithAnyArgs().Create(Arg.Any<Committee>());
    }

    [TestCase(true, false, false)]
    [TestCase(false, true, false)]
    [TestCase(false, true, true)]
    public async Task CreateCommittee_WithOtherRoles_ShouldThrowAuthorizationException(bool isOffice, bool isSecretariat, bool isObserver)
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(false);
        _authorizationService.IsOffice.Returns(isOffice);
        _authorizationService.IsSecretariat.Returns(isSecretariat);
        _authorizationService.IsObserver.Returns(isObserver);

        var departmentId = Guid.NewGuid();
        var beginDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-5));

        var createDto = new CommitteeCreateDto
        {
            DescriptionGerman = "foo",
            DescriptionFrench = "foo",
            DescriptionItalian = "foo",
            DescriptionRomansh = "foo",
            LevelId = Guid.NewGuid(),
            OfficeId = Guid.NewGuid(),
            DepartmentId = departmentId,
            CommitteeTypeId = Guid.NewGuid(),
            FederalLawEstablishment = true,
            SupervisionDuty = true,
            MarketOrientated = true,
            LegalFormId = Guid.NewGuid(),
            LegalBase = "foo",
            TermOfOfficeId = Guid.NewGuid(),
            TermOfOfficeDateId = Guid.NewGuid(),
            MinimalMembers = 1,
            MaximalMembers = 2,
            AdditionalAuthorityMembers = true,
            LinkAuthorityWebsite = "foo",
            BeginDate = beginDate,
            EndDate = DateOnly.FromDateTime(DateTime.Now)
        };

        Assert.That(async () => await _committeeService.CreateCommittee(createDto), Throws.Exception.InstanceOf<AuthorizationException>());

        await _committeeRepository.DidNotReceiveWithAnyArgs().Create(Arg.Any<Committee>());
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task ValidateCommittee_WithNoErrors_ShouldReturnEmptyResponseObject(bool isUpdateMode)
    {
        var beginYear = DateTime.Now.AddYears(-1).Year;
        var beginDate = new DateOnly(beginYear, 1, 1);

        var request = new CommitteeMembershipValidationRequestDto
        {
            IsUpdateMode = isUpdateMode,
            CommitteeId = _committeeId,
            PersonId = _personId3,
            BeginDate = beginDate,
            EndDate = new DateOnly(2027, 12, 31),
            InCorrelationWithFederalDuty = false
        };

        var result = await _committeeService.ValidateCommittee(_committeeId, request);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.HasErrors, Is.EqualTo(false));
            Assert.That(result.IsAlreadyActiveMember, Is.EqualTo(false));
            Assert.That(result.MaximumDurationExceeded, Is.EqualTo(false));
            Assert.That(result.TooManyMembers, Is.EqualTo(false));
            Assert.That(result.CurrentTermOfOffice, Is.EqualTo(2));
            Assert.That(result.EstimatedTermOfOffice, Is.EqualTo(5));
        });
    }

    [TestCase(true, false)]
    [TestCase(false, true)]
    public async Task ValidateCommittee_WithPersonAlreadyInCommittee_ShouldReturnErrorInObject(bool isUpdateMode, bool expectedIsAlreadyActiveMember)
    {
        var beginYear = DateTime.Now.AddYears(-1).Year;
        var beginDate = new DateOnly(beginYear, 1, 1);

        var request = new CommitteeMembershipValidationRequestDto { IsUpdateMode = isUpdateMode, CommitteeId = _committeeId, PersonId = _personId1, BeginDate = beginDate, EndDate = new DateOnly(2027, 12, 31), InCorrelationWithFederalDuty = false };

        var result = await _committeeService.ValidateCommittee(_committeeId, request);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.HasErrors, Is.EqualTo(expectedIsAlreadyActiveMember));
            Assert.That(result.IsAlreadyActiveMember, Is.EqualTo(expectedIsAlreadyActiveMember));
            Assert.That(result.MaximumDurationExceeded, Is.EqualTo(false));
            Assert.That(result.TooManyMembers, Is.EqualTo(false));
        });
    }

    [TestCase(true, false)]
    [TestCase(false, true)]
    public async Task ValidateCommittee_WithTooManyMembers_ShouldReturnErrorInObject(bool isUpdateMode, bool expectedTooManyMembers)
    {
        _committee.MaximalMembers = 1;

        var beginYear = DateTime.Now.AddYears(-1).Year;
        var beginDate = new DateOnly(beginYear, 1, 1);

        var request = new CommitteeMembershipValidationRequestDto
        {
            IsUpdateMode = isUpdateMode,
            CommitteeId = _committeeId,
            PersonId = _personId3,
            BeginDate = beginDate,
            EndDate = new DateOnly(2027, 12, 31),
            InCorrelationWithFederalDuty = false
        };

        var result = await _committeeService.ValidateCommittee(_committeeId, request);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.HasErrors, Is.EqualTo(expectedTooManyMembers));
            Assert.That(result.IsAlreadyActiveMember, Is.EqualTo(false));
            Assert.That(result.MaximumDurationExceeded, Is.EqualTo(false));
            Assert.That(result.TooManyMembers, Is.EqualTo(expectedTooManyMembers));
        });
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task ValidateCommittee_WithMaximumDurationExceededAndRelevantCommitteeType_ShouldReturnErrorInObject(bool isUpdateMode)
    {
        _committee.MaximalMembers = 2;
        _committee.CommitteeTypeId = Guid.Parse("0a4b7f1d-d8bf-4932-bece-dd2a51cc2d59");

        var beginYear = DateTime.Now.AddYears(-1).Year;
        var beginDate = new DateOnly(beginYear, 1, 1);

        var request = new CommitteeMembershipValidationRequestDto { IsUpdateMode = isUpdateMode, CommitteeId = _committeeId, PersonId = _personId2, BeginDate = beginDate, EndDate = new DateOnly(2047, 12, 31), InCorrelationWithFederalDuty = false };

        var result = await _committeeService.ValidateCommittee(_committeeId, request);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.HasErrors, Is.EqualTo(true));
        Assert.That(result.IsAlreadyActiveMember, Is.EqualTo(false));
        Assert.That(result.MaximumDurationExceeded, Is.EqualTo(true));
        Assert.That(result.TooManyMembers, Is.EqualTo(false));
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task ValidateCommittee_WithMaximumDurationExceededButNotRelevantCommitteeType_ShouldNotReturnErrorInObject(bool isUpdateMode)
    {
        _committee.MaximalMembers = 2;

        var beginYear = DateTime.Now.AddYears(-1).Year;
        var beginDate = new DateOnly(beginYear, 1, 1);

        var request = new CommitteeMembershipValidationRequestDto { IsUpdateMode = isUpdateMode, CommitteeId = _committeeId, PersonId = _personId2, BeginDate = beginDate, EndDate = new DateOnly(2027, 12, 31), InCorrelationWithFederalDuty = false };

        var result = await _committeeService.ValidateCommittee(_committeeId, request);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.HasErrors, Is.EqualTo(false));
            Assert.That(result.IsAlreadyActiveMember, Is.EqualTo(false));
            Assert.That(result.MaximumDurationExceeded, Is.EqualTo(false));
            Assert.That(result.TooManyMembers, Is.EqualTo(false));
        });
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task ValidateCommittee_WithMaximumDurationExceededAndSeveralMembershipsAndRelevantCommitteeType_ShouldReturnErrorInObject(bool isUpdateMode)
    {
        _committee.MaximalMembers = 2;
        _committee.CommitteeTypeId = Guid.Parse("f2e2af70-d1d4-42b5-b23a-793cbc220064");

        var request = new CommitteeMembershipValidationRequestDto { IsUpdateMode = isUpdateMode, CommitteeId = _committeeId, PersonId = _personId2, BeginDate = new DateOnly(2025, 1, 1), EndDate = new DateOnly(2029, 12, 31), InCorrelationWithFederalDuty = false };

        var result = await _committeeService.ValidateCommittee(_committeeId, request);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.HasErrors, Is.EqualTo(true));
            Assert.That(result.IsAlreadyActiveMember, Is.EqualTo(false));
            Assert.That(result.MaximumDurationExceeded, Is.EqualTo(true));
            Assert.That(result.TooManyMembers, Is.EqualTo(false));
        });
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task ValidateCommittee_WhenPersonIsFederalAssemblyAndCommitteeIsAuthoritiesCommission_ShouldSetTrue(bool isUpdateMode)
    {
        _person1.FederalAssembly = true;
        _committee.CommitteeTypeId = CommitteeType.AuthoritiesCommissionGuid;

        _membershipRepository.GetAllByCommitteeId(_committeeId).Returns([]);

        var request = new CommitteeMembershipValidationRequestDto
        {
            IsUpdateMode = isUpdateMode,
            CommitteeId = _committeeId,
            PersonId = _personId1,
            BeginDate = new DateOnly(2020, 1, 2),
            EndDate = new DateOnly(2023, 12, 30),
            InCorrelationWithFederalDuty = true
        };

        var result = await _committeeService.ValidateCommittee(_committeeId, request);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFederalAssemblyAndAuthoritiesCommission, Is.EqualTo(true));
            Assert.That(result.IsAlreadyActiveMember, Is.EqualTo(false));
            Assert.That(result.MaximumDurationExceeded, Is.EqualTo(false));
            Assert.That(result.TooManyMembers, Is.EqualTo(false));
            Assert.That(result.HasErrors, Is.EqualTo(true));
        });
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task ValidateCommittee_WhenPersonIsFederalAssemblyButCommitteeIsNotAuthoritiesCommission_ShouldSetFalse(bool isUpdateMode)
    {
        _person1.FederalAssembly = true;
        _committee.CommitteeTypeId = CommitteeType.AdministrationCommissionGuid;

        _membershipRepository.GetAllByCommitteeId(_committeeId).Returns([]);

        var request = new CommitteeMembershipValidationRequestDto
        {
            IsUpdateMode = isUpdateMode,
            CommitteeId = _committeeId,
            PersonId = _personId1,
            BeginDate = new DateOnly(2020, 1, 1),
            EndDate = new DateOnly(2024, 12, 31),
            InCorrelationWithFederalDuty = true
        };

        var result = await _committeeService.ValidateCommittee(_committeeId, request);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFederalAssemblyAndAuthoritiesCommission, Is.EqualTo(false));
            Assert.That(result.IsAlreadyActiveMember, Is.EqualTo(false));
            Assert.That(result.MaximumDurationExceeded, Is.EqualTo(false));
            Assert.That(result.TooManyMembers, Is.EqualTo(false));
            Assert.That(result.HasErrors, Is.EqualTo(false));
        });
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task ValidateCommittee_WhenPersonIsFederalAssemblyAndCommitteeIsAuthoritiesCommissionButNoOverlappingLegislaturePeriod_ShouldSetFalse(bool isUpdateMode)
    {
        _person1.FederalAssembly = true;
        _committee.CommitteeTypeId = CommitteeType.AuthoritiesCommissionGuid;

        var request = new CommitteeMembershipValidationRequestDto
        {
            IsUpdateMode = isUpdateMode,
            CommitteeId = _committeeId,
            PersonId = _personId1,
            BeginDate = new DateOnly(2016, 1, 1),
            EndDate = new DateOnly(2019, 12, 31),
            InCorrelationWithFederalDuty = true
        };

        var result = await _committeeService.ValidateCommittee(_committeeId, request);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFederalAssemblyAndAuthoritiesCommission, Is.EqualTo(false));
            Assert.That(result.IsAlreadyActiveMember, Is.EqualTo(false));
            Assert.That(result.MaximumDurationExceeded, Is.EqualTo(false));
            Assert.That(result.TooManyMembers, Is.EqualTo(false));
            Assert.That(result.HasErrors, Is.EqualTo(false));
        });
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task ValidateCommittee_WhenPersonIsNotFederalAssemblyButCommitteeIsAuthoritiesCommission_ShouldSetFalse(bool isUpdateMode)
    {
        _person1.FederalAssembly = false;
        _committee.CommitteeTypeId = CommitteeType.AuthoritiesCommissionGuid;

        _membershipRepository.GetAllByCommitteeId(_committeeId).Returns([]);

        var request = new CommitteeMembershipValidationRequestDto
        {
            IsUpdateMode = isUpdateMode,
            CommitteeId = _committeeId,
            PersonId = _personId1,
            BeginDate = new DateOnly(2020, 1, 1),
            EndDate = new DateOnly(2024, 12, 31),
            InCorrelationWithFederalDuty = true
        };

        var result = await _committeeService.ValidateCommittee(_committeeId, request);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsFederalAssemblyAndAuthoritiesCommission, Is.EqualTo(false));
            Assert.That(result.IsAlreadyActiveMember, Is.EqualTo(false));
            Assert.That(result.MaximumDurationExceeded, Is.EqualTo(false));
            Assert.That(result.TooManyMembers, Is.EqualTo(false));
            Assert.That(result.HasErrors, Is.EqualTo(false));
        });
    }

    [TestCase(true, false, true, true, true)]
    [TestCase(false, true, true, false, true)]
    public async Task GetEmpty_WhenCalledWithDepartmentRoleInOwnDepartment_ShouldCreateEmptyObjectWithDepIdAndEditPermission(bool isAdmin, bool isDepartment, bool expectedCanEditAll, bool expectedCanEditDepartment, bool expectedCanEditLegalbase)
    {
        var dep = new DepartmentBuilder().WithId(Guid.NewGuid()).Build();

        _authorizationService.GetDepartment().Returns(dep);

        var result = await _committeeService.GetEmpty();

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.BeginDate, Is.GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now)));
            Assert.That(result.DepartmentId, Is.EqualTo(dep.Id));
        });
    }

    [Test]
    public async Task UpdateCommitteeAfterGeneralElection_WhenCalled_ShouldUpdatePropertiesAndCommitChanges()
    {
        var updateDto = BuildUpdateDto();
        _committeeRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_committee);
        _committeeRepository.GetById(updateDto.Id).Returns(_committee);

        await _committeeService.UpdateCommitteeAfterGeneralElection(updateDto.Id, updateDto, _candidateListWithPerson);

        await _committeeRepository.Received(1).GetByIdForUpdate(updateDto.Id, updateDto.RowVersion);

        await _membershipMirrorService.Received(1).CreateNewMembershipFromCandidate(Arg.Any<MembershipCreateDto>(), Arg.Any<string>());

        await _membershipMirrorService.Received(1).UpdateMembershipFromCandidate(Arg.Any<Guid>(), Arg.Any<MembershipUpdateDto>(), Arg.Any<string>());
    }

    [Test]
    public async Task UpdateCommitteeAfterGeneralElection_WhenCalled_ShouldNotCreateMembersWithoutPersonRecord()
    {
        var updateDto = BuildUpdateDto();
        _committeeRepository.GetByIdForUpdate(updateDto.Id, updateDto.RowVersion).Returns(_committee);
        _committeeRepository.GetById(updateDto.Id).Returns(_committee);

        await _committeeService.UpdateCommitteeAfterGeneralElection(updateDto.Id, updateDto, _candidateListWithoutPerson);

        await _membershipMirrorService.Received(0).CreateNewMembershipFromCandidate(Arg.Any<MembershipCreateDto>(), Arg.Any<string>());
    }

    private static CommitteeUpdateDto BuildUpdateDto()
    {
        return new CommitteeUpdateDto
        {
            DescriptionGerman = "foo",
            DescriptionFrench = "foo",
            DescriptionItalian = "foo",
            DescriptionRomansh = "foo",
            LevelId = Guid.NewGuid(),
            OfficeId = Guid.NewGuid(),
            DepartmentId = Guid.NewGuid(),
            CommitteeTypeId = Guid.NewGuid(),
            FederalLawEstablishment = true,
            SupervisionDuty = true,
            MarketOrientated = true,
            LegalFormId = Guid.NewGuid(),
            LegalBase = "foo",
            TermOfOfficeId = Guid.NewGuid(),
            MinimalMembers = 1,
            MaximalMembers = 2,
            AdditionalAuthorityMembers = true,
            LinkAuthorityWebsite = "foo",
            BeginDate = DateOnly.FromDateTime(DateTime.Now),
            EndDate = DateOnly.FromDateTime(DateTime.Now),
            LinkHomepageGerman = "fooDE",
            LinkHomepageFrench = "fooFR",
            LinkHomepageItalian = "fooIT",
            LinkHomepageRomansh = "fooRM",
            RowVersion = 666,
            MembershipAdditionsInGeneralElection = [Guid.NewGuid()],
            VacanciesInGeneralElection = 3
        };
    }

    private static CommitteeJustificationUpdateDto BuildJustificationUpdateDto()
    {
        return new CommitteeJustificationUpdateDto
        {
            JustificationMembers = "member",
            JustificationGenders = "gender",
            MeasuresGenders = "measureGender",
            JustificationLanguages = "justificationLanguage",
            MeasuresLanguages = "measureLanguages",
            RowVersion = 666
        };
    }
}
