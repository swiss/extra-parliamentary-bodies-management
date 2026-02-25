using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
internal class CountryServiceTests
{
    private readonly ICountryRepository _countryRepository = Substitute.For<ICountryRepository>();

    private CountryService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _service = new CountryService(_countryRepository);
    }

    [TearDown]
    public void TearDown()
    {
        _countryRepository.ClearSubstitute();
    }

    [Test]
    public async Task GetAll_WhenCalled_ShouldCallService()
    {
        var result = await _service.GetAll();

        await _countryRepository.Received(1).GetAll();
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetCountries_WhenCalled_ShouldCallServiceAndReturnSortedResult()
    {
        var country1 = new CountryBuilder().WithTextDe("CH").Build();
        var country2 = new CountryBuilder().WithTextDe("DE").Build();
        var country3 = new CountryBuilder().WithTextDe("IT").Build();
        var country4 = new CountryBuilder().WithTextDe("FR").Build();

        country1.Sort = 0;
        country2.Sort = 2;
        country3.Sort = 1;
        country4.Sort = 3;

        var countryList = new List<Country>()
        {
            country1,
            country2,
            country3,
            country4
        };

        _countryRepository.GetAll().Returns(countryList);
        var result = await _service.GetAll();

        await _countryRepository.Received(1).GetAll();
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(4));

        Assert.That(result.First().Text, Is.EqualTo("CH"));
        Assert.That(result.Last().Text, Is.EqualTo("FR"));
    }

    [Test]
    public async Task CreateOrUpdate_ForUnknownCountry_ShouldCreateNewCountry()
    {
        var country = new CountryBuilder().Build();
        _countryRepository.GetByUri(Arg.Any<string>()).ReturnsNullForAnyArgs();
        Country? countryToCreate = null;
        _countryRepository.Create(Arg.Do<Country>(o => countryToCreate = o)).Returns(country);

        var newCountry = await _service.CreateOrUpdate(country);

        Assert.That(newCountry, Is.Not.Null);
        Assert.That(newCountry, Is.InstanceOf<Country>());

        Assert.Multiple(() =>
        {
            Assert.That(countryToCreate!.CreatedBy, Is.EqualTo(country.ModifiedBy));
            Assert.That(countryToCreate.ModifiedBy, Is.EqualTo(country.ModifiedBy));
            Assert.That(countryToCreate.IsDeleted, Is.False);
            Assert.That(countryToCreate.TextDe, Is.EqualTo(country.TextDe));
            Assert.That(countryToCreate.TextFr, Is.EqualTo(country.TextFr));
            Assert.That(countryToCreate.TextIt, Is.EqualTo(country.TextIt));
            Assert.That(countryToCreate.TextRm, Is.EqualTo(country.TextRm));
            Assert.That(countryToCreate.DescriptionDe, Is.EqualTo(country.DescriptionDe));
            Assert.That(countryToCreate.DescriptionFr, Is.EqualTo(country.DescriptionFr));
            Assert.That(countryToCreate.DescriptionIt, Is.EqualTo(country.DescriptionIt));
            Assert.That(countryToCreate.DescriptionRm, Is.EqualTo(country.DescriptionRm));
            Assert.That(countryToCreate.Uri, Is.EqualTo(country.Uri));
        });

        await _countryRepository.Received(1).GetByUri(Arg.Is(country.Uri));
        await _countryRepository.Received(1).Create(Arg.Any<Country>());
    }

    [Test]
    public async Task CreateOrUpdate_ForKnownCountry_ShouldUpdateCountry()
    {
        var countryUpdate = new CountryBuilder().Build();
        var countryFromDb = new CountryBuilder().Build();
        _countryRepository.GetByUri(Arg.Any<string>()).Returns(countryFromDb);
        _countryRepository.Update(Arg.Any<Country>(), Arg.Any<Country>()).Returns(countryFromDb);

        var newCountry = await _service.CreateOrUpdate(countryUpdate);

        Assert.That(newCountry, Is.Not.Null);
        Assert.That(newCountry, Is.InstanceOf<Country>());

        await _countryRepository.Received(1).GetByUri(Arg.Is(countryUpdate.Uri));
        await _countryRepository.Received(1).Update(Arg.Is(countryFromDb), Arg.Is(countryUpdate));
    }
}
