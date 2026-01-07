using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
internal class CantonServiceTests
{
    private readonly ICantonRepository _cantonRepository = Substitute.For<ICantonRepository>();

    private CantonService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _service = new CantonService(_cantonRepository);
    }

    [TearDown]
    public void TearDown()
    {
        _cantonRepository.ClearSubstitute();
    }

    [Test]
    public async Task GetCantons_WhenCalled_ShouldCallService()
    {
        var result = await _service.GetAll();

        await _cantonRepository.Received(1).GetAll();
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetCantons_WhenCalled_ShouldCallServiceAndReturnSortedResult()
    {
        var canton1 = new CantonBuilder().WithTextDe("ZH").Build();
        var canton2 = new CantonBuilder().WithTextDe("AG").Build();
        var canton3 = new CantonBuilder().WithTextDe("BS").Build();
        var canton4 = new CantonBuilder().WithTextDe("VD").Build();

        canton1.Sort = 0;
        canton2.Sort = 2;
        canton3.Sort = 1;
        canton4.Sort = 0;

        var cantonList = new List<Canton>()
        {
            canton1,
            canton2,
            canton3,
            canton4
        };

        _cantonRepository.GetAll().Returns(cantonList);
        var result = await _service.GetAll();

        await _cantonRepository.Received(1).GetAll();
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(4));

        Assert.That(result.First().Text, Is.EqualTo("VD"));
        Assert.That(result.Last().Text, Is.EqualTo("AG"));

        Assert.That(result, Is.Ordered.By("Sort").Then.By("Text"));
    }

    [Test]
    public async Task CreateOrUpdate_ForUnknownCanton_ShouldCreateNewCanton()
    {
        var canton = new CantonBuilder().Build();
        _cantonRepository.GetByUri(Arg.Any<string>()).ReturnsNullForAnyArgs();
        Canton? cantonToCreate = null;
        _cantonRepository.Create(Arg.Do<Canton>(o => cantonToCreate = o)).Returns(canton);

        var newCanton = await _service.CreateOrUpdate(canton);

        Assert.That(newCanton, Is.Not.Null);
        Assert.That(newCanton, Is.InstanceOf<Canton>());

        Assert.Multiple(() =>
        {
            Assert.That(cantonToCreate!.CreatedBy, Is.EqualTo(canton.ModifiedBy));
            Assert.That(cantonToCreate.ModifiedBy, Is.EqualTo(canton.ModifiedBy));
            Assert.That(cantonToCreate.IsDeleted, Is.False);
            Assert.That(cantonToCreate.TextDe, Is.EqualTo(canton.TextDe));
            Assert.That(cantonToCreate.TextFr, Is.EqualTo(canton.TextFr));
            Assert.That(cantonToCreate.TextIt, Is.EqualTo(canton.TextIt));
            Assert.That(cantonToCreate.TextRm, Is.EqualTo(canton.TextRm));
            Assert.That(cantonToCreate.DescriptionDe, Is.EqualTo(canton.DescriptionDe));
            Assert.That(cantonToCreate.DescriptionFr, Is.EqualTo(canton.DescriptionFr));
            Assert.That(cantonToCreate.DescriptionIt, Is.EqualTo(canton.DescriptionIt));
            Assert.That(cantonToCreate.DescriptionRm, Is.EqualTo(canton.DescriptionRm));
            Assert.That(cantonToCreate.Uri, Is.EqualTo(canton.Uri));
        });

        await _cantonRepository.Received(1).GetByUri(Arg.Is(canton.Uri));
        await _cantonRepository.Received(1).Create(Arg.Any<Canton>());
    }

    [Test]
    public async Task CreateOrUpdate_ForKnownCanton_ShouldUpdateCanton()
    {
        var cantonUpdate = new CantonBuilder().Build();
        var cantonFromDb = new CantonBuilder().Build();
        _cantonRepository.GetByUri(Arg.Any<string>()).Returns(cantonFromDb);
        _cantonRepository.Update(Arg.Any<Canton>(), Arg.Any<Canton>()).Returns(cantonFromDb);

        var newCanton = await _service.CreateOrUpdate(cantonUpdate);

        Assert.That(newCanton, Is.Not.Null);
        Assert.That(newCanton, Is.InstanceOf<Canton>());

        await _cantonRepository.Received(1).GetByUri(Arg.Is(cantonUpdate.Uri));
        await _cantonRepository.Received(1).Update(Arg.Is(cantonFromDb), Arg.Is(cantonUpdate));
    }
}
