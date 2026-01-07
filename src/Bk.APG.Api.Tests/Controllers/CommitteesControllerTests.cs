using Bk.APG.Api.Controllers;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Tests.Controllers;

[TestFixture]
internal class CommitteesControllerTests
{
    private readonly ICommitteeService _committeeService = Substitute.For<ICommitteeService>();
    private readonly IMembershipService _membershipService = Substitute.For<IMembershipService>();

    private CommitteesController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _controller = new CommitteesController(_committeeService, _membershipService);
    }

    [TearDown]
    public void TearDown()
    {
        _committeeService.ClearSubstitute();
        _membershipService.ClearSubstitute();
    }

    [Test]
    public async Task GetAll_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var committees = new PagedResultDto<CommitteeListDto>
        {
            Index = 0,
            Total = 100,
            Items = new Faker<CommitteeListDto>().Generate(10)
        };
        _committeeService
            .GetCommitteeList(Arg.Any<PagingParametersDto>(), Arg.Any<CommitteeFilterParametersDto>(), Arg.Any<string?>(), Arg.Any<SortDirection?>())
            .Returns(committees);

        var response = await _controller.GetAll(new PagingParametersDto(), null, new SortParametersDto());

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseObject.StatusCode, Is.EqualTo(200));
            Assert.That(responseObject.Value, Is.EqualTo(committees));
        });
    }

    [Test]
    public async Task GetById_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var committeeDetailDto = new CommitteeDetailDto
        {
            Id = Guid.NewGuid(),
            Description = "desc",
            DescriptionDe = "desc",
            DescriptionFr = "desc",
            DescriptionIt = "desc",
            DescriptionRm = "desc",
            CommitteeLevel = "level",
            Department = "dep",
            Office = "off",
            CommitteeType = "type",
            CommitteeTypeId = Guid.NewGuid(),
            TermOfOffice = "term",
            Period4YearsInGeneralElection = true
        };

        _committeeService.GetCommitteeDetail(Arg.Any<Guid>()).Returns(committeeDetailDto);

        var result = await _controller.GetById(Guid.NewGuid()) as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(committeeDetailDto));
        });
    }

    [Test]
    public async Task GetByIdForUpdate_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var committeeUpdateDto = new Faker<CommitteeUpdateDto>().Generate();

        _committeeService.GetCommitteeForUpdate(Arg.Any<Guid>()).Returns(committeeUpdateDto);

        var result = await _controller.GetByIdForUpdate(Guid.NewGuid()) as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(committeeUpdateDto));
        });
    }

    [Test]
    public async Task GetByIdForJustificationUpdate_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var committeeJustificationUpdateDto = new Faker<CommitteeJustificationUpdateDto>().Generate();

        _committeeService.GetCommitteeJustificationForUpdate(Arg.Any<Guid>()).Returns(committeeJustificationUpdateDto);

        var result = await _controller.GetByIdForJustificationUpdate(Guid.NewGuid()) as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(committeeJustificationUpdateDto));
        });
    }

    [Test]
    public async Task Update_WhenCalledWithValidData_ShouldCallServiceAndReturnResult()
    {
        var updateDto = new Faker<CommitteeUpdateDto>().Generate();
        var detailDto = new Faker<CommitteeDetailDto>().Generate();
        _committeeService.UpdateCommittee(updateDto.Id, updateDto).Returns(detailDto);

        var result = await _controller.Update(updateDto.Id, updateDto) as OkObjectResult;

        await _committeeService.Received(1).UpdateCommittee(updateDto.Id, updateDto);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(detailDto));
        });
    }

    [Test]
    public async Task UpdateJustifications_WhenCalledWithValidData_ShouldCallServiceAndReturnResult()
    {
        var updateDto = new Faker<CommitteeJustificationUpdateDto>().Generate();
        _committeeService.UpdateCommitteeJustifications(updateDto.Id, updateDto).Returns(updateDto);

        var result = await _controller.UpdateJustifications(updateDto.Id, updateDto) as OkObjectResult;

        await _committeeService.Received(1).UpdateCommitteeJustifications(updateDto.Id, updateDto);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(updateDto));
        });
    }

    [Test]
    public async Task Update_WhenCalledWithDifferentIds_ShouldNotCallServiceAndReturnBadRequest()
    {
        var updateDto = new Faker<CommitteeUpdateDto>().Generate();

        var result = await _controller.Update(Guid.NewGuid(), updateDto);

        await _committeeService.DidNotReceive().UpdateCommittee(Arg.Any<Guid>(), Arg.Any<CommitteeUpdateDto>());
        Assert.That(result, Is.InstanceOf<BadRequestResult>());
    }

    [Test]
    public async Task Create_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var createDto = new Faker<CommitteeCreateDto>().Generate();
        var detailDto = new Faker<CommitteeDetailDto>().Generate();
        _committeeService.CreateCommittee(createDto).Returns(detailDto);

        var result = await _controller.Create(createDto) as OkObjectResult;

        await _committeeService.Received(1).CreateCommittee(createDto);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(detailDto));
        });
    }

    [Test]
    public async Task GetMembers_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var membershipList = new MembershipListDto
        {
            CommitteeQuotas = new Faker<CommitteeQuotasDto>(),
            ActiveMemberships = new List<CommitteeMemberDto>
            {
                new Faker<CommitteeMemberDto>().Generate(),
                new Faker<CommitteeMemberDto>().Generate()
            }
        };
        _membershipService.GetAllByCommitteeId(Arg.Any<Guid>()).Returns(membershipList);

        var result = await _controller.GetMembers(Guid.NewGuid()) as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(membershipList));
        });
    }

    [Test]
    public async Task GetByDescription_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var members = new List<CommitteeDetailDto>
        {
            new Faker<CommitteeDetailDto>().Generate(),
            new Faker<CommitteeDetailDto>().Generate()
        };

        _committeeService.GetByDescription(Arg.Any<string>()).Returns(members);

        var result = await _controller.GetByDescription("desc") as OkObjectResult;

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(members));
        });
    }

    [Test]
    public async Task CreateMembership_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var createDto = new Faker<MembershipCreateDto>().Generate();
        var listDto = new Faker<MembershipDetailDto>().Generate();
        _membershipService.CreateMembership(createDto).Returns(listDto);

        var result = await _controller.CreateMember(createDto) as OkObjectResult;

        await _membershipService.Received(1).CreateMembership(createDto);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(listDto));
        });
    }

    [Test]
    public async Task CheckMemberships_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var validationRequest = new Faker<CommitteeMembershipValidationRequestDto>().Generate();
        var validationResult = new Faker<CommitteeMembershipValidationResultDto>().Generate();
        _committeeService.ValidateCommittee(validationRequest.CommitteeId, validationRequest).Returns(validationResult);

        var result = await _controller.CheckMemberships(validationRequest.CommitteeId, validationRequest) as OkObjectResult;

        await _committeeService.Received(1).ValidateCommittee(validationRequest.CommitteeId, validationRequest);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(validationResult));
        });
    }

    [Test]
    public async Task GetEmpty_WhenCalled_ShouldCallServiceAndReturnResult()
    {
        var response = await _controller.GetEmpty();

        await _committeeService.ReceivedWithAnyArgs().GetEmpty();

        Assert.That(response, Is.Not.Null);
        var responseObject = response as OkObjectResult;

        Assert.That(responseObject, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(responseObject.StatusCode, Is.EqualTo(200));
        });
    }
}
