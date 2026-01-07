using System.Globalization;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
internal class OccupationServiceTests
{
    private readonly IOccupationRepository _occupationRepository = Substitute.For<IOccupationRepository>();
    private readonly ICultureService _cultureService = Substitute.For<ICultureService>();

    private OccupationService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _service = new OccupationService(_occupationRepository, _cultureService);
    }

    [TearDown]
    public void TearDown()
    {
        _occupationRepository.ClearSubstitute();
        _cultureService.ClearSubstitute();
    }

    [Test]
    public async Task GetByName_ShouldReturnPersonList()
    {
        List<Occupation> occupations =
        [
            new OccupationBuilder()
                .WithGermanText("Koch")
                .WithGermanFemaleText("Köchin")
                .Build(),
            new OccupationBuilder()
                .WithGermanText("Suppenkoch")
                .WithGermanFemaleText("Suppenköchin")
                .Build(),
        ];

        _cultureService.GetCurrentUiCulture().Returns(new CultureInfo("de"));

        _occupationRepository.GetBySearchString(Arg.Any<string>()).Returns(occupations);

        var personDetails = (await _service.GetBySearchString("koch"))?.ToList();

        Assert.That(personDetails, Is.Not.Null);
        Assert.That(personDetails, Has.Count.EqualTo(2));
    }
}
