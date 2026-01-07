using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting.Exception;
using Bk.APG.CrossCutting.Tests.Builders;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
internal class CommitteeTypeServiceTests
{
    private readonly ICommitteeTypeRepository _committeeTypeRepository = Substitute.For<ICommitteeTypeRepository>();
    private readonly ICultureService _cultureService = Substitute.For<ICultureService>();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();

    private CommitteeTypeService _committeeTypeService;

    private CommitteeType _committeeType1;
    private CommitteeType _committeeType2;
    private List<CommitteeType> _committeeTypeList;
    private CommitteeTypeUpdateDto _committeeTypeUpdateDto;
    private Guid _committeeTypeId;

    [SetUp]
    public void SetUp()
    {
        _cultureService.GetCurrentUiCulture().Returns(new CultureInfo("de"));
        _committeeTypeService = new CommitteeTypeService(_committeeTypeRepository,
            _cultureService,
            _authorizationService,
            NullLogger<CommitteeTypeService>.Instance);

        _committeeTypeId = Guid.Parse("17FEBC36-0837-4AD3-AB92-0594777FBC1E");

        _committeeType1 = new CommitteeTypeBuilder()
            .WithId(_committeeTypeId)
            .WithFemaleAndMaleThreshold(20.0, 19.9)
            .WithLanguagesPercentageThreshold(20.5, 19.5, 18.5, 17.5)
            .WithLanguagesThreshold(0, 0, 0, 0)
            .WithGermanText("Deutscher Gremium Typ mit Prozentwerten")
            .WithGermanDescription("Deutscher langer Gremium Typ, prozentuale Werte")
            .Build();

        _committeeType2 = new CommitteeTypeBuilder()
            .WithId(_committeeTypeId)
            .WithFemaleAndMaleThreshold(20.0, 19.9)
            .WithLanguagesPercentageThreshold(0, 0, 0, 0)
            .WithLanguagesThreshold(2, 2, 2, 1)
            .WithGermanText("Deutscher Gremium Typ mit minimal Werten")
            .WithGermanDescription("Deutscher langer Gremium Typ, minimale Werte")
            .Build();

        _committeeTypeUpdateDto = new CommitteeTypeUpdateDto() { Id = _committeeTypeId, FemaleThreshold = 11, MaleThreshold = 22, GermanMinimalThreshold = 33, FrenchMinimalThreshold = 44 };

        _committeeTypeList = new List<CommitteeType>();
        _committeeTypeList.Add(_committeeType1);
        _committeeTypeList.Add(_committeeType2);

        _authorizationService.IsAdmin.Returns(false);
    }

    [TearDown]
    public void TearDown()
    {
        _committeeTypeRepository.ClearSubstitute();
        _cultureService.ClearSubstitute();
        _authorizationService.ClearSubstitute();
    }

    [Test]
    public async Task GetCommitteeList_ShouldReturnCommitteeList()
    {
        _committeeTypeRepository
            .GetList()
            .Returns(_committeeTypeList);

        var committeesTypes = await _committeeTypeService.GetCommitteeTypeList();

        await _committeeTypeRepository.Received(1).GetList();

        Assert.That(committeesTypes, Is.Not.Null);
        Assert.That(committeesTypes.Count, Is.EqualTo(_committeeTypeList.Count));
    }

    [Test]
    public async Task GetCommitteeForUpdate_ShouldReturnUpdateDto()
    {
        _committeeTypeRepository.GetByIdForUpdate(Arg.Is(_committeeType1.Id)).Returns(_committeeType1);
        _authorizationService.IsAdmin.Returns(true);

        var committeeTypeUpdateDto = await _committeeTypeService.GetCommitteeTypeForUpdate(_committeeType1.Id);

        await _committeeTypeRepository.Received(1).GetByIdForUpdate(Arg.Any<Guid>());

        Assert.That(committeeTypeUpdateDto, Is.Not.Null);
        Assert.That(committeeTypeUpdateDto.Id, Is.EqualTo(_committeeType1.Id));
    }

    [Test]
    public async Task UpdateCommittee_WithAdminRole_ShouldUpdatePropertiesAndCommitChanges()
    {
        _authorizationService.IsAdmin.Returns(true);

        _committeeTypeRepository.GetByIdForUpdate(Arg.Any<Guid>(), Arg.Any<uint>()).Returns(_committeeType1);

        _committeeTypeRepository.GetByIdForUpdate(Arg.Any<Guid>()).Returns(_committeeType1);

        await _committeeTypeService.UpdateCommitteeType(_committeeTypeUpdateDto.Id, _committeeTypeUpdateDto);

        await _committeeTypeRepository.Received(1).GetByIdForUpdate(_committeeTypeUpdateDto.Id, _committeeTypeUpdateDto.RowVersion);

        Assert.Multiple(() =>
        {
            Assert.That(_committeeType1.FemaleThreshold, Is.EqualTo(_committeeTypeUpdateDto.FemaleThreshold));
            Assert.That(_committeeType1.MaleThreshold, Is.EqualTo(_committeeTypeUpdateDto.MaleThreshold));
            Assert.That(_committeeType1.GermanMinimalThreshold, Is.EqualTo(_committeeTypeUpdateDto.GermanMinimalThreshold));
            Assert.That(_committeeType1.FrenchMinimalThreshold, Is.EqualTo(_committeeTypeUpdateDto.FrenchMinimalThreshold));
        });
    }

    [Test]
    public async Task UpdateCommittee_WithoutAdminRole_ShouldThrowAuthorizationException()
    {
        _authorizationService.IsAdmin.Returns(false);
        _authorizationService.IsDepartment.Returns(true);

        _committeeTypeRepository.GetByIdForUpdate(_committeeTypeUpdateDto.Id, _committeeTypeUpdateDto.RowVersion).Returns(_committeeType1);

        Assert.That(async () => await _committeeTypeService.UpdateCommitteeType(_committeeTypeUpdateDto.Id, _committeeTypeUpdateDto), Throws.Exception.InstanceOf<AuthorizationException>());

        await _committeeTypeRepository.Received(1).GetByIdForUpdate(_committeeTypeUpdateDto.Id, _committeeTypeUpdateDto.RowVersion);
    }

    [Test]
    public async Task UpdateCommittee_WithoutMinimalAndPercentageValues_ShouldThrowBusinessValidationException()
    {
        _authorizationService.IsAdmin.Returns(true);

        _committeeTypeUpdateDto.GermanMinimalThreshold = 5;
        _committeeTypeUpdateDto.GermanThresholdPercentage = 20;

        _committeeTypeRepository.GetByIdForUpdate(_committeeTypeUpdateDto.Id, _committeeTypeUpdateDto.RowVersion).Returns(_committeeType1);

        Assert.That(async () => await _committeeTypeService.UpdateCommitteeType(_committeeTypeUpdateDto.Id, _committeeTypeUpdateDto), Throws.Exception.InstanceOf<BusinessValidationException>());

        await _committeeTypeRepository.Received(1).GetByIdForUpdate(_committeeTypeUpdateDto.Id, _committeeTypeUpdateDto.RowVersion);
    }

    [Test]
    public async Task UpdateCommittee_WithMoreThan100PercentOnLanguageValues_ShouldThrowBusinessValidationException()
    {
        _authorizationService.IsAdmin.Returns(true);

        _committeeTypeUpdateDto.GermanThresholdPercentage = 20;
        _committeeTypeUpdateDto.FrenchThresholdPercentage = 60;
        _committeeTypeUpdateDto.ItalianThresholdPercentage = 40;
        _committeeTypeUpdateDto.RomanshThresholdPercentage = 30;

        _committeeTypeRepository.GetByIdForUpdate(_committeeTypeUpdateDto.Id, _committeeTypeUpdateDto.RowVersion).Returns(_committeeType1);

        Assert.That(async () => await _committeeTypeService.UpdateCommitteeType(_committeeTypeUpdateDto.Id, _committeeTypeUpdateDto), Throws.Exception.InstanceOf<BusinessValidationException>());

        await _committeeTypeRepository.Received(1).GetByIdForUpdate(_committeeTypeUpdateDto.Id, _committeeTypeUpdateDto.RowVersion);
    }

    [Test]
    public async Task UpdateCommittee_WithMoreThan100PercentOnGenderValues_ShouldThrowBusinessValidationException()
    {
        _authorizationService.IsAdmin.Returns(true);

        _committeeTypeUpdateDto.FemaleThreshold = 50;
        _committeeTypeUpdateDto.MaleThreshold = 60;

        _committeeTypeRepository.GetByIdForUpdate(_committeeTypeUpdateDto.Id, _committeeTypeUpdateDto.RowVersion).Returns(_committeeType1);

        Assert.That(async () => await _committeeTypeService.UpdateCommitteeType(_committeeTypeUpdateDto.Id, _committeeTypeUpdateDto), Throws.Exception.InstanceOf<BusinessValidationException>());

        await _committeeTypeRepository.Received(1).GetByIdForUpdate(_committeeTypeUpdateDto.Id, _committeeTypeUpdateDto.RowVersion);
    }
}
