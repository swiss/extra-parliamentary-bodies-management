using Bk.APG.Api.Controllers;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting;
using Bk.APG.CrossCutting.Tests.Builders;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Tests.Controllers;

[TestFixture]
internal class GeneralElectionCommitteeControllerTests
{
    private readonly IMembershipCandidateService _membershipCandidateService = Substitute.For<IMembershipCandidateService>();
    private readonly IGeneralElectionCommitteeService _generalElectionCommitteeService = Substitute.For<IGeneralElectionCommitteeService>();
    private readonly IEiamAssignmentService _eiamAssignmentService = Substitute.For<IEiamAssignmentService>();

    private GeneralElectionCommitteeController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _controller = new GeneralElectionCommitteeController(_membershipCandidateService, _generalElectionCommitteeService, _eiamAssignmentService);
    }

    [TearDown]
    public void TearDown()
    {
        _membershipCandidateService.ClearSubstitute();
        _generalElectionCommitteeService.ClearSubstitute();
    }

    [Test]
    public async Task GetAssignmentsCandidateListForward_ShouldCallServiceAndReturnAssignments()
    {
        var assignments = new Faker<EiamAssignmentDto>().Generate(3);
        var committeeId = Guid.NewGuid();
        _eiamAssignmentService.GetAllForCandidateListForward(committeeId).Returns(assignments);

        var result = await _controller.GetAssignmentsCandidateListForward(committeeId) as OkObjectResult;

        await _eiamAssignmentService.Received(1).GetAllForCandidateListForward(committeeId);
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(assignments));
        }
    }

    [Test]
    public async Task ForwardCandidateList_ShouldCallServiceAndReturnOk()
    {
        var committeeId = Guid.NewGuid();
        var forwardDto = new Faker<CandidateListForwardDto>().Generate();
        _membershipCandidateService.ForwardCandidateList(committeeId, forwardDto).Returns(Task.CompletedTask);

        var result = await _controller.ForwardCandidateList(committeeId, forwardDto) as OkResult;

        await _membershipCandidateService.Received(1).ForwardCandidateList(committeeId, forwardDto);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task GetAssignmentsReadyForProposalForward_ShouldCallServiceAndReturnAssignments()
    {
        var assignments = new Faker<EiamAssignmentDto>().Generate(2);
        var committeeId = Guid.NewGuid();
        _eiamAssignmentService.GetAllForReadyForProposalForward(committeeId).Returns(assignments);

        var result = await _controller.GetAssignmentsReadyForProposalForward(committeeId) as OkObjectResult;

        await _eiamAssignmentService.Received(1).GetAllForReadyForProposalForward(committeeId);
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(assignments));
        }
    }

    [Test]
    public async Task ForwardReadyForProposal_ShouldCallServiceAndReturnOk()
    {
        var committeeId = Guid.NewGuid();
        var forwardDto = new Faker<ReadyForProposalForwardDto>().Generate();
        _membershipCandidateService.ForwardReadyForProposal(committeeId, forwardDto).Returns(Task.CompletedTask);

        var result = await _controller.ForwardReadyForProposal(committeeId, forwardDto) as OkResult;

        await _membershipCandidateService.Received(1).ForwardReadyForProposal(committeeId, forwardDto);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task FinalizeReadyForProposal_ShouldCallServiceAndReturnOk()
    {
        var committeeId = Guid.NewGuid();
        var validationResult = new CandidateListValidationResultDto();
        _membershipCandidateService.FinalizeReadyForProposal(committeeId).Returns(validationResult);

        var result = await _controller.FinalizeReadyForProposal(committeeId) as OkObjectResult;

        await _membershipCandidateService.Received(1).FinalizeReadyForProposal(committeeId);
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(validationResult));
        }
    }

    [Test]
    public async Task ValidateCandidateList_ShouldCallServiceAndReturnOk()
    {
        var committeeId = Guid.NewGuid();
        var validationRequest = new CandidateListValidationRequest
        {
            SelectedCandidateIds = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()],
            DuplicateCheckConfirmed = false
        };
        var validationResult = new CandidateListValidationResultDto();
        _membershipCandidateService.ValidateCandidateList(committeeId, validationRequest.SelectedCandidateIds, validationRequest.DuplicateCheckConfirmed).Returns(validationResult);

        var result = await _controller.ValidateCandidateList(committeeId, validationRequest) as OkObjectResult;

        await _membershipCandidateService.Received(1).ValidateCandidateList(committeeId, validationRequest.SelectedCandidateIds, validationRequest.DuplicateCheckConfirmed);
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(validationResult));
        }
    }

    [Test]
    public async Task SaveCandidateList_ShouldCallServiceAndReturnOk()
    {
        var committeeId = Guid.NewGuid();
        var candidateIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        _membershipCandidateService.SaveCandidateList(committeeId, candidateIds).Returns(Task.CompletedTask);

        var result = await _controller.SaveCandidateList(committeeId, candidateIds) as OkResult;

        await _membershipCandidateService.Received(1).SaveCandidateList(committeeId, candidateIds);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task GetGeneralElectionCommitteesGetAll_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var committees = new PagedResultDto<GeneralElectionCommitteeListDto>
        {
            Index = 0,
            Total = 100,
            Items = new Faker<GeneralElectionCommitteeListDto>().Generate(10)
        };
        _generalElectionCommitteeService
            .GetGeneralElectionCommitteeList(Arg.Any<PagingParametersDto>(), Arg.Any<GeneralElectionCommitteeFilterParametersDto>(), Arg.Any<string?>(), Arg.Any<SortDirection?>())
            .Returns(committees);

        var response = await _controller.GetGeneralElectionCommitteesGetAll(new PagingParametersDto(), null, new SortParametersDto());

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(responseObject.StatusCode, Is.EqualTo(200));
            Assert.That(responseObject.Value, Is.EqualTo(committees));
        }
    }

    [Test]
    public async Task GetByIdForJustificationUpdate_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var committeeJustificationUpdateDto = new Faker<GeneralElectionCommitteeJustificationUpdateDto>().Generate();

        _generalElectionCommitteeService.GetGeneralElectionCommitteeJustificationForUpdate(Arg.Any<Guid>()).Returns(committeeJustificationUpdateDto);

        var result = await _controller.GetByIdForJustificationUpdate(Guid.NewGuid()) as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(committeeJustificationUpdateDto));
        }
    }

    [Test]
    public async Task GetGeneralElectionCommitteesForRecipientExport_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var filterDto = new GeneralElectionCommitteeExportFilterParametersDto();
        var committeeList = new List<GeneralElectionCommitteeListDto>();

        var committee = new Faker<GeneralElectionCommitteeListDto>().Generate();
        committeeList.Add(committee);

        _generalElectionCommitteeService.GetGeneralElectionCommitteeListForRecipientExport(filterDto).Returns(committeeList);

        var result = await _controller.GetGeneralElectionCommitteesForRecipientExport(filterDto) as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(committeeList));
        }
    }

    [Test]
    public async Task UpdateVacancies_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var committeeJustificationUpdateDto = new Faker<GeneralElectionCommitteeUpdateDto>().Generate();

        _generalElectionCommitteeService.UpdateGeneralElectionCommitteeVacancies(Arg.Any<Guid>(), Arg.Any<int>()).Returns(committeeJustificationUpdateDto);

        var result = await _controller.UpdateVacancies(Guid.NewGuid(), 3) as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(committeeJustificationUpdateDto));
        }
    }

    [Test]
    public async Task GetDuplicateMembershipCandidate_ShouldCallServiceAndReturnSimilarMembershipCandidate()
    {
        var candidateToCheck = new MembershipCandidateCreateDto
        {
            CommitteeId = Guid.NewGuid(),
            Surname = "Clark",
            GivenName = "Jim",
            BirthYear = 1936,
            GenderId = Guid.NewGuid(),
            LanguageId = Guid.NewGuid(),
            FunctionId = Guid.NewGuid(),
        };

        var candidate = MembershipCandidateMapper.ToMembershipCandidateDetailDto(new MembershipCandidateBuilder().Build());

        _membershipCandidateService.GetDuplicateMembershipCandidateForList(candidateToCheck.CommitteeId, candidateToCheck.Surname, candidateToCheck.GivenName, candidateToCheck.BirthYear, candidateToCheck.GenderId, candidateToCheck.LanguageId).Returns(candidate);

        var result = await _controller.GetDuplicateMembershipCandidate(candidateToCheck) as OkObjectResult;

        await _membershipCandidateService.Received(1).GetDuplicateMembershipCandidateForList(candidateToCheck.CommitteeId, candidateToCheck.Surname, candidateToCheck.GivenName, candidateToCheck.BirthYear, candidateToCheck.GenderId, candidateToCheck.LanguageId);
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(candidate));
        }
    }

    [Test]
    public async Task CheckUnfinishedCommittees_ShouldCallServiceAndReturnOk()
    {
        var list = new Faker<GeneralElectionCommitteeListDto>().Generate(3);

        _generalElectionCommitteeService.GetAllUnfinishedCommittees().Returns(list);

        var result = await _controller.CheckUnfinishedCommittees() as OkObjectResult;

        await _generalElectionCommitteeService.Received(1).GetAllUnfinishedCommittees();
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(200));
    }
}
