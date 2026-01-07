using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
internal class InterestServiceTests
{
    private readonly IInterestRepository _interestRepository = Substitute.For<IInterestRepository>();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();

    private InterestService _service = null!;
    private Interest _interestToUpdate = null!;
    private Interest _interestToUpdate2 = null!;
    private Interest _interestToUpdate3 = null!;
    private readonly List<Interest> _interestList = [];
    private Guid _interestId;
    private Guid _interestId2;
    private Guid _interestId3;
    private Guid _personId;

    [SetUp]
    public void SetUp()
    {
        _service = new InterestService(_interestRepository, _authorizationService);

        var interestCommittee = new InterestCommitteeBuilder()
            .WithId(Guid.NewGuid())
            .Build();

        var interestFunction = new InterestFunctionBuilder()
            .WithId(Guid.NewGuid())
            .Build();

        var interestLegalForm = new InterestLegalFormBuilder()
            .WithId(Guid.NewGuid())
            .Build();

        _interestId = Guid.NewGuid();
        _interestId2 = Guid.NewGuid();
        _interestId3 = Guid.NewGuid();

        _personId = Guid.NewGuid();

        _interestToUpdate = new InterestBuilder()
            .WithId(_interestId)
            .WithPersonId(_personId)
            .WithInterestCommittee(interestCommittee)
            .WithInterestFunction(interestFunction)
            .WithInterestLegalForm(interestLegalForm)
            .Build();

        _interestToUpdate2 = new InterestBuilder()
            .WithId(_interestId2)
            .WithPersonId(_personId)
            .Build();

        _interestToUpdate3 = new InterestBuilder()
            .WithId(_interestId3)
            .WithPersonId(_personId)
            .Build();

        _interestList.Add(_interestToUpdate);
        _interestList.Add(_interestToUpdate2);
        _interestList.Add(_interestToUpdate3);

        _interestRepository.GetByIdForUpdate(_interestToUpdate.Id).Returns(_interestToUpdate);
        _interestRepository.GetByIdForUpdate(_interestToUpdate3.Id).Returns(_interestToUpdate3);
        _interestRepository.Create(Arg.Any<Interest>()).Returns(_interestToUpdate);
        _interestRepository.Update(Arg.Any<Interest>(), Arg.Any<Interest>()).Returns(_interestToUpdate);
        _interestRepository.GetAllByPersonId(Arg.Is(_personId)).Returns(_interestList);

        _authorizationService.GetCurrentUserName().Returns("currentUser");
    }

    [TearDown]
    public void TearDown()
    {
        _interestRepository.ClearSubstitute();
        _authorizationService.ClearSubstitute();
    }

    [Test]
    public async Task GetInterestsForUpdateByPersonId_WhenCalled_ShouldCallService()
    {
        var beginDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10));

        var resultFromRepository = new List<Interest>
        {
            new InterestBuilder()
                .WithPersonId(_personId)
                .WithText("CD")
                .WithBeginDate(null)
                .WithInterestCommittee(new InterestCommitteeBuilder().Build())
                .WithInterestLegalForm(new InterestLegalFormBuilder().Build())
                .WithInterestFunction(new InterestFunctionBuilder().Build())
                .Build(),
            new InterestBuilder()
                .WithPersonId(_personId)
                .WithText("AD")
                .WithBeginDate(beginDate)
                .WithInterestCommittee(new InterestCommitteeBuilder().Build())
                .WithInterestLegalForm(new InterestLegalFormBuilder().Build())
                .WithInterestFunction(new InterestFunctionBuilder().Build())
                .Build(),
            new InterestBuilder()
                .WithPersonId(_personId)
                .WithText("AC")
                .WithBeginDate(beginDate)
                .WithInterestCommittee(new InterestCommitteeBuilder().Build())
                .WithInterestLegalForm(new InterestLegalFormBuilder().Build())
                .WithInterestFunction(new InterestFunctionBuilder().Build())
                .Build()
        };

        _interestRepository.GetAllByPersonId(Arg.Any<Guid>()).Returns(resultFromRepository);

        var interests = (await _service.GetInterestsForUpdateByPersonId(_personId)).ToList();

        await _interestRepository.Received(1).GetAllByPersonId(Arg.Is(_personId));

        Assert.That(interests, Is.Not.Null);
        Assert.That(interests, Has.Count.EqualTo(resultFromRepository.Count));

        Assert.That(interests.First().Text, Is.EqualTo("CD"));
        Assert.That(interests.Last().Text, Is.EqualTo("AD"));
    }

    [Test]
    public async Task UpdateInterests_WhenCalledWithValidData_ShouldCreateUpdateAndDeleteOnRepository()
    {
        var updateDto = new InterestUpdateDto
        {
            Id = _interestToUpdate.Id,
            PersonId = _interestToUpdate.PersonId,
            Text = "my text",
            InterestText = "my new text",
            InterestCommitteeId = Guid.NewGuid(),
            InterestFunctionId = Guid.NewGuid(),
            InterestLegalFormId = Guid.NewGuid(),
            LegalFormId = Guid.NewGuid(),
            RowVersion = _interestToUpdate.RowVersion
        };

        var updateDto2 = new InterestUpdateDto
        {
            Id = _interestToUpdate2.Id,
            PersonId = _interestToUpdate.PersonId,
            Text = "my text",
            InterestText = "my new text",
            InterestCommitteeId = Guid.NewGuid(),
            InterestFunctionId = Guid.NewGuid(),
            InterestLegalFormId = Guid.NewGuid(),
            LegalFormId = Guid.NewGuid(),
            RowVersion = _interestToUpdate2.RowVersion
        };

        var updateDto4 = new InterestUpdateDto
        {
            // new record, no guid to be set
            PersonId = _interestToUpdate.PersonId,
            Text = "my text",
            InterestText = "my new text",
            InterestCommitteeId = Guid.NewGuid(),
            InterestFunctionId = Guid.NewGuid(),
            InterestLegalFormId = Guid.NewGuid(),
            LegalFormId = Guid.NewGuid(),
            RowVersion = 0
        };

        var updateList = new List<InterestUpdateDto>
        {
            updateDto,
            updateDto2,
            updateDto4
        };

        await _service.UpdateInterests(_personId, updateList.ToArray());

        _authorizationService.Received(1).GetCurrentUserName();

        await _interestRepository.Received(1).GetByIdForUpdate(_interestToUpdate.Id, _interestToUpdate.RowVersion);
        await _interestRepository.Received(1).GetByIdForUpdate(_interestToUpdate2.Id, _interestToUpdate2.RowVersion);

        await _interestRepository.Received(1).Create(
            Arg.Is<Interest>(i => i.Created >= DateTime.UtcNow.AddSeconds(-1)
                                && i.CreatedBy == "currentUser"
                                && i.Modified >= DateTime.UtcNow.AddSeconds(-1)
                                && i.ModifiedBy == "currentUser"));

        _interestRepository.Received(1).Delete(_interestToUpdate3);
    }
}
