using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.CrossCutting;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Mapper;

[TestFixture]
internal class MasterDataMapperTests
{
    [Test]
    public void MapToMasterDataDto_ShouldMapToMasterDataDto()
    {
        var masterData = new DepartmentBuilder().WithGermanText("foo").WithGermanDescription("bar").Build();
        masterData.IsDeleted = true;

        var masterDataDto = MasterDataMapper.MapToMasterDataDto<DepartmentDto>(masterData, new CultureInfo("de"));

        Assert.That(masterDataDto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(masterDataDto.Id, Is.EqualTo(masterData.Id));
            Assert.That(masterDataDto.Text, Is.EqualTo("foo"));
            Assert.That(masterDataDto.Description, Is.EqualTo("bar"));
            Assert.That(masterDataDto.IsDeleted, Is.EqualTo(true));
        });
    }

    [Test]
    public void MapFunctionToMasterDataDto_ShouldMapToMasterDataDto()
    {
        var masterData = new FunctionBuilder()
            .WithGermanText("foo")
            .WithGermanFemaleText("fooFemale")
            .WithGermanDescription("bar").Build();
        masterData.IsDeleted = true;

        var masterDataDto = MasterDataMapper.MapFunctionToMasterDataDto<FunctionDto>(masterData, new CultureInfo("de"));

        Assert.That(masterDataDto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(masterDataDto.Id, Is.EqualTo(masterData.Id));
            Assert.That(masterDataDto.Text, Is.EqualTo("foo"));
            Assert.That(masterDataDto.TextFemale, Is.EqualTo("fooFemale"));
            Assert.That(masterDataDto.Description, Is.EqualTo("bar"));
            Assert.That(masterDataDto.IsDeleted, Is.EqualTo(true));
        });
    }

    [Test]
    public void MapWorklistTaskTypeToMasterDataDto_ShouldMapToMasterDataDto()
    {
        var masterData = new WorklistTaskTypeBuilder()
            .WithGermanText("foo")
            .WithGermanDescription("bar")
            .WithCanBeCreatedManually(true)
            .Build();

        var masterDataDto = MasterDataMapper.MapWorklistTaskTypeToMasterDataDto<WorklistTaskTypeDto>(masterData, new CultureInfo("de"));

        Assert.That(masterDataDto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(masterDataDto.Id, Is.EqualTo(masterData.Id));
            Assert.That(masterDataDto.Text, Is.EqualTo("foo"));
            Assert.That(masterDataDto.CanBeCreatedManually, Is.True);
            Assert.That(masterDataDto.Description, Is.EqualTo("bar"));
        });
    }

    [Test]
    public void MapTermOfOfficeDateToMasterDataDto_ShouldMapToMasterDataDto()
    {
        var masterData = new TermOfOfficeDateBuilder()
            .WithGermanText("foo")
            .WithGermanDescription("bar")
            .WithBeginDate(new DateOnly(2024, 1, 1))
            .WithEndDate(new DateOnly(2024, 12, 31))
            .WithIsGeneralElection(true).Build();
        masterData.IsDeleted = true;

        var masterDataDto = MasterDataMapper.MapTermOfOfficeDateToMasterDataDto<TermDateDto>(masterData, new CultureInfo("de"));

        Assert.That(masterDataDto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(masterDataDto.Id, Is.EqualTo(masterData.Id));
            Assert.That(masterDataDto.Text, Is.EqualTo("foo"));
            Assert.That(masterDataDto.Description, Is.EqualTo("bar"));
            Assert.That(masterDataDto.IsDeleted, Is.EqualTo(true));
            Assert.That(masterDataDto.BeginDate, Is.EqualTo(masterData.BeginDate));
            Assert.That(masterDataDto.EndDate, Is.EqualTo(masterData.EndDate));
            Assert.That(masterDataDto.IsGeneralElection, Is.EqualTo(masterData.IsGeneralElection));
        });
    }

    [Test]
    public void MapToLegislaturePeriodDto_ShouldMapToLegislaturePeriodDto()
    {
        var legislaturePeriod = new LegislaturePeriodBuilder()
            .WithElectionDate(new DateOnly(2023, 1, 1))
            .WithStartDate(new DateOnly(2023, 2, 1))
            .WithEndDate(new DateOnly(2027, 1, 31))
            .WithText("Test Period")
            .Build();

        var legislaturePeriodDto = MasterDataMapper.MapToLegislaturePeriodDto(legislaturePeriod);

        Assert.That(legislaturePeriodDto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(legislaturePeriodDto.Id, Is.EqualTo(legislaturePeriod.Id));
            Assert.That(legislaturePeriodDto.Text, Is.EqualTo("Test Period"));
            Assert.That(legislaturePeriodDto.ElectionDate, Is.EqualTo(new DateOnly(2023, 1, 1)));
            Assert.That(legislaturePeriodDto.StartDate, Is.EqualTo(new DateOnly(2023, 2, 1)));
            Assert.That(legislaturePeriodDto.EndDate, Is.EqualTo(new DateOnly(2027, 1, 31)));
        });
    }

    [Test]
    public void ToDimensionItem_FromMasterDataBase_MapsCorrectly()
    {
        var committeeTypeId = Guid.NewGuid();
        var committeeTypeOgdId = 1;

        var committeeType =
            new CommitteeTypeBuilder()
                .WithId(committeeTypeId)
                .WithOgdId(committeeTypeOgdId)
                .WithGermanText("de")
                .WithGermanDescription("desc de")
                .WithFrenchDescription("desc fr")
                .WithItalianDescription("desc it")
                .WithRomanshDescription("desc rm")
                .Build();

        var result = MasterDataMapper.ToDimensionItem(committeeType);

        Assert.That(result, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Key, Is.EqualTo(committeeTypeOgdId));
            Assert.That(result.Name.Text, Is.EqualTo("de"));
            Assert.That(result.Name.LanguageTag, Is.EqualTo("de"));
            Assert.That(result.AdditionalLiteralProperties, Has.Count.EqualTo(8));
            Assert.That(result.AdditionalUriProperties, Is.Empty);
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.AdditionalLiteralProperties[0].Predicate, Is.EqualTo(OgdExportConstants.SchemaDescription));
            Assert.That(result.AdditionalLiteralProperties[0].Object.Text, Is.EqualTo("desc de"));
            Assert.That(result.AdditionalLiteralProperties[0].Object.LanguageTag, Is.EqualTo("de"));

            Assert.That(result.AdditionalLiteralProperties[1].Predicate, Is.EqualTo(OgdExportConstants.SchemaDescription));
            Assert.That(result.AdditionalLiteralProperties[1].Object.Text, Is.EqualTo("desc fr"));
            Assert.That(result.AdditionalLiteralProperties[1].Object.LanguageTag, Is.EqualTo("fr"));

            Assert.That(result.AdditionalLiteralProperties[2].Predicate, Is.EqualTo(OgdExportConstants.SchemaDescription));
            Assert.That(result.AdditionalLiteralProperties[2].Object.Text, Is.EqualTo("desc it"));
            Assert.That(result.AdditionalLiteralProperties[2].Object.LanguageTag, Is.EqualTo("it"));

            Assert.That(result.AdditionalLiteralProperties[3].Predicate, Is.EqualTo(OgdExportConstants.SchemaDescription));
            Assert.That(result.AdditionalLiteralProperties[3].Object.Text, Is.EqualTo("desc rm"));
            Assert.That(result.AdditionalLiteralProperties[3].Object.LanguageTag, Is.EqualTo("rm"));

            Assert.That(result.AdditionalLiteralProperties[4].Predicate, Is.EqualTo(OgdExportConstants.SchemaName));
            Assert.That(result.AdditionalLiteralProperties[4].Object.Text, Is.EqualTo(committeeType.TextFr));
            Assert.That(result.AdditionalLiteralProperties[4].Object.LanguageTag, Is.EqualTo("fr"));

            Assert.That(result.AdditionalLiteralProperties[5].Predicate, Is.EqualTo(OgdExportConstants.SchemaName));
            Assert.That(result.AdditionalLiteralProperties[5].Object.Text, Is.EqualTo(committeeType.TextIt));
            Assert.That(result.AdditionalLiteralProperties[5].Object.LanguageTag, Is.EqualTo("it"));

            Assert.That(result.AdditionalLiteralProperties[6].Predicate, Is.EqualTo(OgdExportConstants.SchemaName));
            Assert.That(result.AdditionalLiteralProperties[6].Object.Text, Is.EqualTo(committeeType.TextRm));
            Assert.That(result.AdditionalLiteralProperties[6].Object.LanguageTag, Is.EqualTo("rm"));
        }
    }

    [Test]
    public void MapToDepartmentDto_ShouldMapToDepartmentDto()
    {
        var department = new DepartmentBuilder().Build();

        var departmentDto = MasterDataMapper.MapToDepartmentDto(department);

        Assert.That(departmentDto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(departmentDto.Id, Is.EqualTo(department.Id));
            Assert.That(departmentDto.Text, Is.EqualTo(department.GetText()));
            Assert.That(departmentDto.Description, Is.EqualTo(department.GetDescription()));
            Assert.That(departmentDto.IsDeleted, Is.EqualTo(department.IsDeleted));
            Assert.That(departmentDto.Uri, Is.EqualTo(department.Uri));
        });
    }

    [Test]
    public void MapToOfficeDto_ShouldMapToOfficeDto()
    {
        var office = new OfficeBuilder().Build();

        var officeDto = MasterDataMapper.MapToOfficeDto(office, new CultureInfo("de"));

        Assert.That(officeDto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(officeDto.Id, Is.EqualTo(office.Id));
            Assert.That(officeDto.Text, Is.EqualTo(office.GetText()));
            Assert.That(officeDto.Description, Is.EqualTo(office.GetDescription()));
            Assert.That(officeDto.IsDeleted, Is.EqualTo(office.IsDeleted));
            Assert.That(officeDto.Uri, Is.EqualTo(office.Uri));
            Assert.That(officeDto.DepartmentId, Is.EqualTo(office.DepartmentId));
        });
    }

    [Test]
    public void MapToOccupationDto_ShouldMapToOccupationDto()
    {
        var occupation = new OccupationBuilder().Build();

        var occupationDto = MasterDataMapper.MapToOccupationDto(occupation, new CultureInfo("de"));

        Assert.That(occupationDto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(occupationDto.Id, Is.EqualTo(occupation.Id));
            Assert.That(occupationDto.Text, Is.EqualTo(occupation.GetText()));
            Assert.That(occupationDto.TextFemale, Is.EqualTo(occupation.GetFemaleText(new CultureInfo("de"))));
            Assert.That(occupationDto.Description, Is.EqualTo(occupation.GetDescription()));
            Assert.That(occupationDto.IsDeleted, Is.EqualTo(occupation.IsDeleted));
            Assert.That(occupationDto.Uri, Is.EqualTo(occupation.Uri));
        });
    }
}
