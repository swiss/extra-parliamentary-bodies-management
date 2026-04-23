using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Extensions;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting;
using Bk.APG.CrossCutting.Tests.Builders;
using Bogus;

namespace Bk.APG.Business.Tests.Mapper;

[TestFixture]
internal class MembershipMapperTests
{
    private readonly ICultureService _cultureService = Substitute.For<ICultureService>();

    [Test]
    public void MapToMembershipDetailDto_WithModel_ShouldMapToDto()
    {
        var membership = new MembershipBuilder()
            .WithId(Guid.Parse("8697753c-f57a-4805-b1e9-bb1e546f4101"))
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-3)))
            .WithEndDate(DateOnly.FromDateTime(DateTime.Now.AddDays(3)))
            .WithFunction(new FunctionBuilder().Build())
            .WithPersonId(Guid.Parse("9df96395-bd65-4235-a4fa-fb87689df11f"))
            .WithCommittee(new CommitteeBuilder().WithGermanDescription("Gremium de la Gremium").Build())
            .Build();

        var dto = MembershipMapper.ToMembershipDetailDto(membership);

        Assert.That(dto, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(dto.Id, Is.EqualTo(membership.Id));
            Assert.That(dto.Function, Is.EqualTo(membership.Function!.GetText()));
            Assert.That(dto.Committee, Is.EqualTo(membership.Committee!.GetDescription()));
            Assert.That(dto.BeginDate, Is.EqualTo(membership.BeginDate));
            Assert.That(dto.EndDate, Is.EqualTo(membership.EndDate));
            Assert.That(dto.IsActive, Is.True);
            Assert.That(dto.NeedsAttention, Is.False);
        }
    }

    [Test]
    public void ToPersonMembershipDto_WithModel_ShouldMapToDto()
    {
        var person = new PersonBuilder()
            .WithId(Guid.Parse("9df96395-bd65-4235-a4fa-fb87689df11f"))
            .WithGender(new GenderBuilder().WithUri(Gender.Female).Build())
            .Build();
        var membership = new MembershipBuilder()
            .WithId(Guid.Parse("8697753c-f57a-4805-b1e9-bb1e546f4101"))
            .WithBeginDate(DateOnly.FromDateTime(DateTime.Now.AddDays(-10)))
            .WithFunction(new FunctionBuilder().WithGermanFemaleText("Tätschmeisterin").Build())
            .WithPerson(person)
            .WithCommittee(new CommitteeBuilder().WithGermanDescription("Gremium de la Gremium").Build())
            .Build();

        var dto = MembershipMapper.ToPersonMembershipDto(membership);

        Assert.That(dto, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(dto.Id, Is.EqualTo(membership.Id));
            Assert.That(dto.Committee, Is.EqualTo(membership.Committee!.GetDescription()));
            Assert.That(dto.Department, Is.EqualTo(membership.Committee.Department!.GetText()));
            Assert.That(dto.Function, Is.EqualTo(membership.Function!.GetFemaleText()));
            Assert.That(dto.BeginDate, Is.EqualTo(membership.BeginDate));
            Assert.That(dto.EndDate, Is.EqualTo(membership.EndDate));
            Assert.That(dto.ElectionType, Is.EqualTo(membership.ElectionType!.GetText()));
            Assert.That(dto.IsActive, Is.True);
            Assert.That(dto.IsFuture, Is.EqualTo(membership.IsFuture));
        }
    }

    [Test]
    public void ToCommitteeMemberDto_WithModel_ShouldMapToDto()
    {
        var membership = new MembershipBuilder()
            .WithCommittee(new CommitteeBuilder().WithGermanDescription("Test DE").WithDepartment(new DepartmentBuilder().Build()).Build())
            .WithPerson(new PersonBuilder()
                .WithGender(new GenderBuilder().WithId(Gender.FemaleGuid).Build())
                .WithLanguage(new LanguageBuilder().Build())
                .Build())
            .Build();

        var dto = MembershipMapper.ToCommitteeMemberDto(membership);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(membership.Id));
            Assert.That(dto.Surname, Is.EqualTo(membership.Person!.Surname));
            Assert.That(dto.GivenName, Is.EqualTo(membership.Person!.GivenName));
            Assert.That(dto.PersonId, Is.EqualTo(membership.PersonId));
            Assert.That(dto.Gender, Is.EqualTo(membership.Person!.Gender!.GetText()));
            Assert.That(dto.Language, Is.EqualTo(membership.Person!.Language!.GetText()));
            Assert.That(dto.EmploymentLevel, Is.EqualTo(membership.GetEmploymentLevel()));
            Assert.That(dto.Function, Is.EqualTo(membership.Function!.GetFemaleText()));
            Assert.That(dto.BeginDate, Is.EqualTo(membership.BeginDate));
            Assert.That(dto.EndDate, Is.EqualTo(membership.EndDate));
            Assert.That(dto.ElectionType, Is.EqualTo(membership.ElectionType!.GetText()));
            Assert.That(dto.HasMembershipAddition, Is.EqualTo(membership.MembershipAddition is not null));
            Assert.That(dto.IsActive, Is.EqualTo(true));
            Assert.That(dto.IsFuture, Is.EqualTo(false));
        });
    }

    [Test]
    public void FromMembershipCreateDto_ShouldMapCorrectly()
    {
        var createDto = new Faker<MembershipCreateDto>().Generate();

        var membership = MembershipMapper.FromMembershipCreateDto(createDto, "foo bar");

        Assert.That(membership, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(membership.PersonId, Is.EqualTo(createDto.PersonId));
            Assert.That(membership.MaximumEmploymentLevel, Is.EqualTo(createDto.MaximumEmploymentLevel));
            Assert.That(membership.BeginDate, Is.EqualTo(createDto.BeginDate));
            Assert.That(membership.EndDate, Is.EqualTo(createDto.EndDate));
            Assert.That(membership.ElectionTypeId, Is.EqualTo(ElectionType.NewElectionGuid));
            Assert.That(membership.FunctionId, Is.EqualTo(createDto.FunctionId));
            Assert.That(membership.ElectionOfficeId, Is.EqualTo(createDto.ElectionOfficeId));
            Assert.That(membership.MembershipAdditionId, Is.EqualTo(createDto.MembershipAdditionId));
            Assert.That(membership.JustificationLongerDuty, Is.EqualTo(createDto.JustificationLongerDuty));
            Assert.That(membership.JustificationShorterDuty, Is.EqualTo(createDto.JustificationShorterDuty));
            Assert.That(membership.JustificationMemberInFederalDuty, Is.EqualTo(createDto.JustificationMemberInFederalDuty));
            Assert.That(membership.JustificationMemberInFederalAssembly, Is.EqualTo(createDto.JustificationMemberInFederalAssembly));
            Assert.That(membership.RequirementsProfile, Is.EqualTo(createDto.RequirementsProfile));
            Assert.That(membership.InCorrelationWithFederalDuty, Is.EqualTo(createDto.InCorrelationWithFederalDuty));
            Assert.That(membership.Remarks, Is.EqualTo(createDto.Remarks));
            Assert.That(membership.RemarksStatus, Is.EqualTo(createDto.RemarksStatus));
            Assert.That(membership.IsDeleted, Is.EqualTo(false));
            Assert.That(membership.Created, Is.GreaterThan(DateTime.UtcNow.AddSeconds(-1)));
            Assert.That(membership.CreatedBy, Is.EqualTo("foo bar"));
            Assert.That(membership.Modified, Is.GreaterThan(DateTime.UtcNow.AddSeconds(-1)));
            Assert.That(membership.ModifiedBy, Is.EqualTo("foo bar"));
        });
    }

    [Test]
    public void ToMembershipUpdateDto_ShouldMapCorrectly()
    {
        _cultureService.GetCurrentUiCulture().Returns(new CultureInfo("de"));

        var membership = new MembershipBuilder().Build();
        var membershipUpdateDto = MembershipMapper.ToMembershipUpdateDto(membership, _cultureService);

        Assert.That(membershipUpdateDto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(membershipUpdateDto.PersonId, Is.EqualTo(membership.PersonId));
            Assert.That(membershipUpdateDto.MaximumEmploymentLevel, Is.EqualTo(membership.MaximumEmploymentLevel));
            Assert.That(membershipUpdateDto.BeginDate, Is.EqualTo(membership.BeginDate));
            Assert.That(membershipUpdateDto.EndDate, Is.EqualTo(membership.EndDate));
            Assert.That(membershipUpdateDto.ElectionTypeId, Is.EqualTo(membership.ElectionTypeId));
            Assert.That(membershipUpdateDto.FunctionId, Is.EqualTo(membership.FunctionId));
            Assert.That(membershipUpdateDto.ElectionOfficeId, Is.EqualTo(membership.ElectionOfficeId));
            Assert.That(membershipUpdateDto.MembershipAdditionId, Is.EqualTo(membership.MembershipAdditionId));
            Assert.That(membershipUpdateDto.JustificationLongerDuty, Is.EqualTo(membership.JustificationLongerDuty));
            Assert.That(membershipUpdateDto.JustificationShorterDuty, Is.EqualTo(membership.JustificationShorterDuty));
            Assert.That(membershipUpdateDto.JustificationMemberInFederalDuty, Is.EqualTo(membership.JustificationMemberInFederalDuty));
            Assert.That(membershipUpdateDto.JustificationMemberInFederalAssembly, Is.EqualTo(membership.JustificationMemberInFederalAssembly));
            Assert.That(membershipUpdateDto.RequirementsProfile, Is.EqualTo(membership.RequirementsProfile));
            Assert.That(membershipUpdateDto.InCorrelationWithFederalDuty, Is.EqualTo(membership.InCorrelationWithFederalDuty));
            Assert.That(membershipUpdateDto.Remarks, Is.EqualTo(membership.Remarks));
            Assert.That(membershipUpdateDto.RemarksStatus, Is.EqualTo(membership.RemarksStatus));
            Assert.That(membershipUpdateDto.RowVersion, Is.EqualTo(membership.RowVersion));
        });
    }

    [Test]
    public void ToObservation_WithValidMembership_MapsCorrectly()
    {
        var membershipId = Guid.NewGuid();
        const int membershipOgdId = 1;

        var personId = Guid.NewGuid();
        const int personOgdId = 2;

        var committeeId = Guid.NewGuid();
        const int committeeOgdId = 3;

        var committeeTypeId = Guid.NewGuid();
        const int committeeTypeOgdId = 4;

        var membershipAdditionId = Guid.NewGuid();
        var electionOfficeId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();

        var gender =
            new GenderBuilder()
                .WithId(Gender.FemaleGuid)
                .WithUri(Gender.Female)
                .Build();

        var language =
            new LanguageBuilder()
                .WithUri("https://lang.de")
                .Build();

        var canton =
            new CantonBuilder()
                .WithUri("https://ld.admin.ch/canton/1")
                .Build();

        var address =
            new AddressBuilder()
                .WithCanton(canton)
                .Build();

        var person =
            new PersonBuilder()
                .WithId(personId)
                .WithOgdId(personOgdId)
                .WithGender(gender)
                .WithLanguage(language)
                .WithCorrespondenceAddress(address)
                .Build();

        var department =
            new DepartmentBuilder()
                .WithId(departmentId)
                .WithUri("https://ld.admin.ch/efd")
                .Build();

        var electionOffice =
            new ElectionOfficeBuilder()
                .WithId(electionOfficeId)
                .WithUri("https://register.ld.admin.ch/efd/bit")
                .Build();

        var membershipAddition =
            new MembershipAdditionBuilder()
                .WithId(membershipAdditionId)
                .WithGermanText("de")
                .WithFrenchText("fr")
                .WithItalianText("it")
                .WithRomanshText("rm")
                .WithGermanDescription("desc de")
                .WithFrenchDescription("desc fr")
                .WithItalianDescription("desc it")
                .WithRomanshDescription("desc rm")
                .Build();

        var committeeType =
            new CommitteeTypeBuilder()
                .WithId(committeeTypeId)
                .WithOgdId(committeeTypeOgdId)
                .Build();

        var committee =
            new CommitteeBuilder()
                .WithId(committeeId)
                .WithOgdId(committeeOgdId)
                .WithCommitteeType(committeeType)
                .WithDepartment(department)
                .Build();

        var membership =
            new MembershipBuilder()
                .WithId(membershipId)
                .WithOgdId(membershipOgdId)
                .WithBeginDate(new DateOnly(2000, 1, 1))
                .WithEndDate(new DateOnly(2001, 1, 1))
                .WithPersonId(personId)
                .WithPerson(person)
                .WithCommittee(committee)
                .WithMembershipAddition(membershipAddition)
                .WithElectionOffice(electionOffice)
                .Build();

        var result = MembershipMapper.ToObservation(membership);

        Assert.That(result, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.KeyUri, Is.EqualTo($"membership:{membershipOgdId}"));
            Assert.That(result.ValidFrom, Is.EqualTo(new DateTime(2000, 1, 1)));
            Assert.That(result.ValidTo, Is.EqualTo(new DateTime(2001, 1, 1)));
            Assert.That(result.KeyDimensionLinks, Has.Count.EqualTo(4));
            Assert.That(result.Values, Has.Count.EqualTo(8));

            Assert.That(result.KeyDimensionLinks[0].Predicate, Is.EqualTo("membership:hasPerson"));
            Assert.That(result.KeyDimensionLinks[0].Uri, Is.EqualTo($"person:{personOgdId}"));

            Assert.That(result.KeyDimensionLinks[1].Predicate, Is.EqualTo("membership:hasCommittee"));
            Assert.That(result.KeyDimensionLinks[1].Uri, Is.EqualTo($"committee:{committeeOgdId}"));

            Assert.That(result.KeyDimensionLinks[2].Predicate, Is.EqualTo("membership:hasElectionOffice"));
            Assert.That(result.KeyDimensionLinks[2].Uri, Is.EqualTo("rld:efd/bit"));

            Assert.That(result.KeyDimensionLinks[3].Predicate, Is.EqualTo("membership:hasFunction"));
            Assert.That(result.KeyDimensionLinks[3].Uri, Is.EqualTo($"function:{membership.Function!.OgdId}"));

            Assert.That(result.Values[0].Predicate, Is.EqualTo("membership:hasFunction"));
            Assert.That(result.Values[0].Object, Is.EqualTo(membership.Function.TextFemaleDe));
            Assert.That(result.Values[0].LanguageTag, Is.EqualTo("de"));

            Assert.That(result.Values[1].Predicate, Is.EqualTo("membership:hasFunction"));
            Assert.That(result.Values[1].Object, Is.EqualTo(membership.Function.TextFemaleFr));
            Assert.That(result.Values[1].LanguageTag, Is.EqualTo("fr"));

            Assert.That(result.Values[2].Predicate, Is.EqualTo("membership:hasFunction"));
            Assert.That(result.Values[2].Object, Is.EqualTo(membership.Function.TextFemaleIt));
            Assert.That(result.Values[2].LanguageTag, Is.EqualTo("it"));

            Assert.That(result.Values[3].Predicate, Is.EqualTo("membership:hasFunction"));
            Assert.That(result.Values[3].Object, Is.EqualTo(membership.Function.TextFemaleRm));
            Assert.That(result.Values[3].LanguageTag, Is.EqualTo("rm"));

            Assert.That(result.Values[4].Predicate, Is.EqualTo(OgdExportConstants.SchemaDescription));
            Assert.That(result.Values[4].Object, Is.EqualTo("de"));
            Assert.That(result.Values[4].LanguageTag, Is.EqualTo("de"));

            Assert.That(result.Values[5].Predicate, Is.EqualTo(OgdExportConstants.SchemaDescription));
            Assert.That(result.Values[5].Object, Is.EqualTo("fr"));
            Assert.That(result.Values[5].LanguageTag, Is.EqualTo("fr"));

            Assert.That(result.Values[6].Predicate, Is.EqualTo(OgdExportConstants.SchemaDescription));
            Assert.That(result.Values[6].Object, Is.EqualTo("it"));
            Assert.That(result.Values[6].LanguageTag, Is.EqualTo("it"));

            Assert.That(result.Values[7].Predicate, Is.EqualTo(OgdExportConstants.SchemaDescription));
            Assert.That(result.Values[7].Object, Is.EqualTo("rm"));
            Assert.That(result.Values[7].LanguageTag, Is.EqualTo("rm"));
        }
    }
}
