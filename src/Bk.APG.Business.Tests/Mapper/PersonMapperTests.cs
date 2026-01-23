using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.CrossCutting;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Mapper;

[TestFixture]
internal class PersonMapperTests
{
    [Test]
    public void MapToPersonList_WithoutCorrespondenceAddress_ShouldThrowArgumentNullException()
    {
        var person = new PersonBuilder().Build();

        Assert.That(() => PersonMapper.ToPersonListDto(person), Throws.Exception.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void MapToPersonList_WithoutLanguage_ShouldThrowArgumentNullException()
    {
        var person = new PersonBuilder()
            .WithCorrespondenceAddress(new AddressBuilder().Build())
            .Build();

        Assert.That(() => PersonMapper.ToPersonListDto(person), Throws.Exception.InstanceOf<ArgumentNullException>());
    }

    [Test]
    [SetCulture("de-CH")]
    public void MapToPersonList_ShouldMapToPersonList()
    {
        var person = new PersonBuilder()
            .WithCorrespondenceAddress(new AddressBuilder().WithCanton(new CantonBuilder().Build()).WithCity("Bern").Build())
            .WithLanguage(new LanguageBuilder().Build())
            .Build();

        var dto = PersonMapper.ToPersonListDto(person);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(person.Id));
            Assert.That(dto.Surname, Is.EqualTo(person.Surname));
            Assert.That(dto.GivenName, Is.EqualTo(person.GivenName));
            Assert.That(dto.BirthYear, Is.EqualTo(person.BirthYear));
            Assert.That(dto.Canton, Is.EqualTo(person.CorrespondenceAddress!.Canton!.TextDe));
            Assert.That(dto.City, Is.EqualTo(person.CorrespondenceAddress.City));
            Assert.That(dto.Language, Is.EqualTo(person.Language!.TextDe));
            Assert.That(dto.HasActiveMembership, Is.EqualTo(false));
            Assert.That(dto.NeedsAttention, Is.EqualTo(person.NeedsAttention));
        });
    }

    [Test]
    [SetCulture("de-CH")]
    public void MapToPersonList_WithDifferentMembershipStatus_ShouldMapToPersonListCorrectly()
    {
        var person = new PersonBuilder()
            .WithCorrespondenceAddress(new AddressBuilder().WithCanton(new CantonBuilder().Build()).WithCity("Bern").Build())
            .WithLanguage(new LanguageBuilder().Build())
            .Build();

        var activeMembership = new MembershipBuilder().Build();
        activeMembership.BeginDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-10));
        activeMembership.EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10));

        var inactiveMembership = new MembershipBuilder().Build();
        inactiveMembership.BeginDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-100));
        inactiveMembership.EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-10));

        person.Memberships.Add(activeMembership);
        person.Memberships.Add(inactiveMembership);

        var dto = PersonMapper.ToPersonListDto(person);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(person.Id));
            Assert.That(dto.Surname, Is.EqualTo(person.Surname));
            Assert.That(dto.GivenName, Is.EqualTo(person.GivenName));
            Assert.That(dto.BirthYear, Is.EqualTo(person.BirthYear));
            Assert.That(dto.Canton, Is.EqualTo(person.CorrespondenceAddress!.Canton!.TextDe));
            Assert.That(dto.City, Is.EqualTo(person.CorrespondenceAddress.City));
            Assert.That(dto.Language, Is.EqualTo(person.Language!.TextDe));
            Assert.That(dto.HasActiveMembership, Is.EqualTo(true));
        });

        person.Memberships.Clear();
        person.Memberships.Add(inactiveMembership);

        dto = PersonMapper.ToPersonListDto(person);

        Assert.That(dto, Is.Not.Null);
        Assert.That(dto.HasActiveMembership, Is.EqualTo(false));
    }

    [Test]
    public void MapToPersonFilterParameters_WithoutDto_ShouldReturnNull()
    {
        var result = PersonMapper.ToPersonFilterParameters(null);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void MapToPersonDetailDto_ShouldMapCorrectly()
    {
        var occupation1 = new OccupationBuilder().WithGermanText("Trax-Führer").Build();
        var occupation2 = new OccupationBuilder().WithGermanText("Bergführer").Build();
        var occupations = new List<Occupation> { occupation1, occupation2 };

        var interests = new List<Interest>();
        var interest1 = new InterestBuilder()
            .WithText("Nur Text")
            .WithInterestText(string.Empty)
            .WithLegalForm(new LegalFormBuilder().Build())
            .WithInterestCommittee(new InterestCommitteeBuilder().Build())
            .WithInterestFunction(new InterestFunctionBuilder().Build())
            .Build();
        var interest2 = new InterestBuilder()
            .WithText(string.Empty)
            .WithInterestText("Nur InterestText")
            .WithLegalForm(new LegalFormBuilder().Build())
            .WithInterestCommittee(new InterestCommitteeBuilder().Build())
            .WithInterestFunction(new InterestFunctionBuilder().Build())
            .Build();
        interests.Add(interest1);
        interests.Add(interest2);

        var committee = new CommitteeBuilder()
            .WithCommitteeLevel(new CommitteeLevelBuilder().Build())
            .WithCommitteeType(new CommitteeTypeBuilder().Build())
            .WithTermOfOffice(new TermOfOfficeBuilder().Build())
            .WithDepartment(new DepartmentBuilder().Build())
            .WithOffice(new OfficeBuilder().Build())
            .Build();

        var membership = new MembershipBuilder()
            .WithCommittee(committee)
            .WithFunction(new FunctionBuilder().Build())
            .Build();

        var person = new PersonBuilder()
            .WithCorrespondenceAddress(new AddressBuilder().WithCanton(new CantonBuilder().Build()).Build())
            .WithCorrespondenceLanguage(new LanguageBuilder().Build())
            .WithLanguage(new LanguageBuilder().Build())
            .WithGender(new GenderBuilder().Build())
            .WithInterests(interests)
            .WithOccupations(occupations)
            .Build();

        person.Memberships.Add(membership);

        var mappedPerson = PersonMapper.ToPersonDetailDto(person, false);

        Assert.That(mappedPerson, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(mappedPerson.Id, Is.EqualTo(person.Id));
            Assert.That(mappedPerson.Surname, Is.EqualTo(person.Surname));
            Assert.That(mappedPerson.GivenName, Is.EqualTo(person.GivenName));
            Assert.That(mappedPerson.BirthYear, Is.EqualTo(person.BirthYear));

            Assert.That(mappedPerson.Occupations, Is.EqualTo("Bergführer / Trax-Führer"));

            Assert.That(mappedPerson.Memberships.First().Id, Is.EqualTo(membership.Id));

            Assert.That(mappedPerson.NeedsAttentionShorterDuty, Is.EqualTo(person.NeedsAttentionShorterDuty));
            Assert.That(mappedPerson.NeedsAttentionLongerDuty, Is.EqualTo(person.NeedsAttentionLongerDuty));
            Assert.That(mappedPerson.NeedsAttentionFederalDuty, Is.EqualTo(person.NeedsAttentionFederalDuty));
            Assert.That(mappedPerson.NeedsAttentionFederalAssemblyAuthoritiesCommission, Is.EqualTo(person.NeedsAttentionFederalAssemblyAuthoritiesCommission));
            Assert.That(mappedPerson.NeedsAttentionFederalAssemblyAdministrationCommission, Is.EqualTo(person.NeedsAttentionFederalAssemblyAdministrationCommission));
            Assert.That(mappedPerson.NeedsAttentionBasicData, Is.EqualTo(person.NeedsAttentionBasicData));
            Assert.That(mappedPerson.NeedsAttentionMembershipExpired, Is.EqualTo(person.NeedsAttentionMembershipExpired));
            Assert.That(mappedPerson.NeedsAttentionOccupation, Is.EqualTo(person.NeedsAttentionOccupation));
            Assert.That(mappedPerson.Interests.Count, Is.EqualTo(2));

            Assert.That(mappedPerson.Interests.First().Text, Is.EqualTo(interest2.InterestText));
            Assert.That(mappedPerson.Interests.Last().Text, Is.EqualTo(interest1.Text));
        });
    }

    [Test]
    public void MapToPersonDetailDto_ShouldMaskAddress()
    {
        var person = new PersonBuilder()
            .WithGender(new GenderBuilder().Build())
            .WithLanguage(new LanguageBuilder().Build())
            .WithCorrespondenceLanguage(new LanguageBuilder().Build())
            .Build();

        var mappedPerson = PersonMapper.ToPersonDetailDto(person, true);

        Assert.That(mappedPerson, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(mappedPerson.PrivateAddress, Is.Null);
            Assert.That(mappedPerson.OfficeAddress, Is.Null);
            Assert.That(mappedPerson.MaskAddress, Is.True);
        });
    }

    [Test]
    public void MapToPersonUpdateDto_ShouldMapCorrectly()
    {
        var committee = new CommitteeBuilder()
            .WithCommitteeLevel(new CommitteeLevelBuilder().Build())
            .WithCommitteeType(new CommitteeTypeBuilder().Build())
            .WithTermOfOffice(new TermOfOfficeBuilder().Build())
            .WithDepartment(new DepartmentBuilder().Build())
            .WithOffice(new OfficeBuilder().Build())
            .Build();

        var membership = new MembershipBuilder()
            .WithCommittee(committee)
            .WithFunction(new FunctionBuilder().Build())
            .Build();

        var person = new PersonBuilder()
            .WithCorrespondenceAddress(new AddressBuilder().WithCanton(new CantonBuilder().Build()).Build())
            .WithLanguage(new LanguageBuilder().Build())
            .WithGender(new GenderBuilder().Build())
            .Build();

        var interest = new InterestBuilder()
            .WithId(Guid.NewGuid())
            .Build();

        person.Memberships.Add(membership);
        person.Interests.Add(interest);

        var mappedPerson = PersonMapper.ToPersonUpdateDto(person, false, false);

        Assert.That(mappedPerson, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(mappedPerson.Id, Is.EqualTo(person.Id));
            Assert.That(mappedPerson.Surname, Is.EqualTo(person.Surname));
            Assert.That(mappedPerson.GivenName, Is.EqualTo(person.GivenName));
            Assert.That(mappedPerson.BirthYear, Is.EqualTo(person.BirthYear));
            Assert.That(mappedPerson.HasInterests, Is.True);
            Assert.That(mappedPerson.LegislaturePeriodIds, Is.EqualTo(person.LegislaturePeriods.Select(x => x.Id)));
            Assert.That(mappedPerson.IsMissingJustificationFederalAssembly, Is.EqualTo(false));
            Assert.That(mappedPerson.CanDelete, Is.False);
        });
    }

    [Test]
    public void MapToPersonUpdateDto_ShouldMaskAddress()
    {
        var person = new PersonBuilder()
            .WithGender(new GenderBuilder().Build())
            .WithLanguage(new LanguageBuilder().Build())
            .WithCorrespondenceLanguage(new LanguageBuilder().Build())
            .Build();

        var mappedPerson = PersonMapper.ToPersonUpdateDto(person, true, false);

        Assert.That(mappedPerson, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(mappedPerson.PrivateAddress, Is.Null);
            Assert.That(mappedPerson.OfficeAddress, Is.Null);
            Assert.That(mappedPerson.MaskAddress, Is.True);
        });
    }

    [Test]
    public void MapToPersonUpdateDto_ShouldMapCanDeleteFlag()
    {
        var person = new PersonBuilder()
            .WithCorrespondenceAddress(new AddressBuilder().Build())
            .WithLanguage(new LanguageBuilder().Build())
            .WithCorrespondenceLanguage(new LanguageBuilder().Build())
            .WithGender(new GenderBuilder().Build())
            .Build();

        var mappedPerson = PersonMapper.ToPersonUpdateDto(person, false, true);

        Assert.That(mappedPerson.CanDelete, Is.True);
    }

    [Test]
    public void MapFromPersonCreateDto_ShouldMapCorrectly()
    {
        var personCreateDto = new PersonCreateDto
        {
            Surname = "surname",
            GivenName = "givenName",
            BirthYear = 2000,
            OfficeAddress = new AddressUpdateDto
            {
                Id = Guid.NewGuid(),
                CantonId = Guid.NewGuid(),
                City = "city_office",
                Street = "street_office",
                CompanyName = "companyname_office",
                CountryCode = "countrycode_office",
                Email = "email_office",
                Mobile = "mobile_office",
                Phone = "phone_office",
                PoBox = "pobox_office",
                Zip = "zip_office",
            },
            LanguageId = Guid.NewGuid(),
            CorrespondenceLanguageId = Guid.NewGuid(),
            Title = "title",
            Occupation = "occupation",
            GenderId = Guid.NewGuid(),
            SalutationId = Guid.NewGuid(),
            FederalAssembly = true,
            FederalDuty = true,
        };

        var mappedPerson = PersonMapper.FromPersonCreateDto(personCreateDto);

        Assert.That(mappedPerson, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(mappedPerson.Surname, Is.EqualTo(personCreateDto.Surname));
            Assert.That(mappedPerson.GivenName, Is.EqualTo(personCreateDto.GivenName));
            Assert.That(mappedPerson.BirthYear, Is.EqualTo(personCreateDto.BirthYear));
            Assert.That(mappedPerson.Title, Is.EqualTo(personCreateDto.Title));
            Assert.That(mappedPerson.FederalAssembly, Is.EqualTo(personCreateDto.FederalAssembly));
            Assert.That(mappedPerson.FederalDuty, Is.EqualTo(personCreateDto.FederalDuty));
            Assert.That(mappedPerson.LanguageId, Is.EqualTo(personCreateDto.LanguageId));
            Assert.That(mappedPerson.GenderId, Is.EqualTo(personCreateDto.GenderId));
            Assert.That(mappedPerson.SalutationId, Is.EqualTo(personCreateDto.SalutationId));
            Assert.That(mappedPerson.CorrespondenceLanguageId, Is.EqualTo(personCreateDto.CorrespondenceLanguageId));
        });
    }

    [Test]
    public void ToDimensionItem_WithValidPerson_ShouldMapCorrectly()
    {
        var personId = Guid.NewGuid();
        var personOgdId = 1;

        var occupations = new List<Occupation>();

        var occupation1 = new OccupationBuilder().Build();
        var occupation2 = new OccupationBuilder().Build();
        occupations.Add(occupation1);
        occupations.Add(occupation2);

        var person =
            new PersonBuilder()
                .WithId(personId)
                .WithOgdId(personOgdId)
                .WithSurname("surname")
                .WithGivenName("givenname")
                .WithTitle("Dr.")
                .WithOccupations(occupations)
                .WithEmployer("employer")
                .Build();

        var result = PersonMapper.ToDimensionItem(person);

        Assert.That(result, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(result.Key, Is.EqualTo(personOgdId));
            Assert.That(result.Name.Text, Is.EqualTo($"surname givenname {person.BirthYear}"));
            Assert.That(result.AdditionalLiteralProperties, Has.Count.EqualTo(5));
            Assert.That(result.AdditionalUriProperties, Has.Count.EqualTo(2));
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.AdditionalUriProperties[0].Predicate, Is.EqualTo(OgdExportConstants.PersonHasOccupation));
            Assert.That(result.AdditionalUriProperties[1].Predicate, Is.EqualTo(OgdExportConstants.PersonHasOccupation));
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.AdditionalLiteralProperties[0].Predicate, Is.EqualTo(OgdExportConstants.SchemaGivenName));
            Assert.That(result.AdditionalLiteralProperties[0].Object.Text, Is.EqualTo("givenname"));

            Assert.That(result.AdditionalLiteralProperties[1].Predicate, Is.EqualTo(OgdExportConstants.SchemaFamilyName));
            Assert.That(result.AdditionalLiteralProperties[1].Object.Text, Is.EqualTo("surname"));

            Assert.That(result.AdditionalLiteralProperties[2].Predicate, Is.EqualTo(OgdExportConstants.SchemaBirthDate));
            Assert.That(result.AdditionalLiteralProperties[2].Object.Text, Is.EqualTo(person.BirthYear.ToString()));

            Assert.That(result.AdditionalLiteralProperties[3].Predicate, Is.EqualTo(OgdExportConstants.SchemaHonorificPrefix));
            Assert.That(result.AdditionalLiteralProperties[3].Object.Text, Is.EqualTo("Dr."));

            Assert.That(result.AdditionalLiteralProperties[4].Predicate, Is.EqualTo(OgdExportConstants.SchemaWorksFor));
            Assert.That(result.AdditionalLiteralProperties[4].Object.Text, Is.EqualTo("employer"));
        });
    }
}
