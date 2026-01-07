using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting.Exception;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
internal class TermOfOfficeDateServiceTests
{
    private readonly ITermOfOfficeDateRepository _termOfOfficeDateRepository = Substitute.For<ITermOfOfficeDateRepository>();

    private TermOfOfficeDateService _service = null!;
    private TermOfOfficeDate _termOfOfficeData1;
    private TermOfOfficeDate _termOfOfficeData2;
    private readonly Guid _termOfOfficeId1 = Guid.Parse("3f5d1a28-9c3d-4cb3-a5ec-d9d8c8e6b7f2");
    private readonly Guid _termOfOfficeId2 = Guid.Parse("f9c47d60-2f48-4a0f-baa9-10f1f8c71b8b");

    [SetUp]
    public void SetUp()
    {
        _service = new TermOfOfficeDateService(_termOfOfficeDateRepository);

        _termOfOfficeData1 = new TermOfOfficeDateBuilder().WithId(_termOfOfficeId1).WithIsGeneralElection(true).WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(200))).Build();
        _termOfOfficeData2 = new TermOfOfficeDateBuilder().WithId(_termOfOfficeId2).WithIsGeneralElection(false).WithBeginDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-200))).Build();

        var termOfOfficeDates = new List<TermOfOfficeDate>
        {
            _termOfOfficeData1,
            _termOfOfficeData2
        };

        _termOfOfficeDateRepository.GetAll().Returns(termOfOfficeDates);
    }

    [TearDown]
    public void TearDown()
    {
        _termOfOfficeDateRepository.ClearSubstitute();
    }

    [Test]
    public async Task CheckForRunningGeneralElection_ShouldCallRepositoryData()
    {
        var result = await _service.CheckForRunningGeneralElection();

        Assert.That(result, Is.EqualTo(true));
    }

    [Test]
    public async Task GetNextTermOfOfficeDate_ShouldCallRepositoryData()
    {
        var result = await _service.GetNextTermOfOfficeDate();

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(_termOfOfficeData1.Id));
    }

    [Test]
    public async Task GetGeneralElectionTermOfOfficeDate_ShouldCallRepositoryData()
    {
        var result = await _service.GetGeneralElectionTermOfOfficeDate();

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(_termOfOfficeData1.Id));
    }

    [Test]
    public void GetNextTermOfOfficeDate_WithNoData_ShouldThrowException()
    {
        _termOfOfficeDateRepository.GetAll().Returns(new List<TermOfOfficeDate>());

        Assert.That(async () => await _service.GetNextTermOfOfficeDate(), Throws.Exception.InstanceOf<EntityNotFoundException>());
    }

    [Test]
    public async Task GetCurrentTermOfOfficeDate_ShouldCallRepositoryData()
    {
        var result = await _service.GetCurrentTermOfOfficeDate();

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(_termOfOfficeData2.Id));
    }

    [Test]
    public void GetCurrentTermOfOfficeDate_WithNoData_ShouldThrowException()
    {
        _termOfOfficeDateRepository.GetAll().Returns(new List<TermOfOfficeDate>());

        Assert.That(async () => await _service.GetCurrentTermOfOfficeDate(), Throws.Exception.InstanceOf<EntityNotFoundException>());
    }

    [Test]
    public async Task Update_ShouldUpdatePropertiesAndCommitChanges()
    {
        _termOfOfficeDateRepository.GetById(_termOfOfficeId1).Returns(_termOfOfficeData1);
        _termOfOfficeDateRepository.Update(_termOfOfficeData1, Arg.Any<TermOfOfficeDate>()).Returns(_termOfOfficeData2);

        var result = await _service.Update(_termOfOfficeData1);

        await _termOfOfficeDateRepository.Received(1).GetById(_termOfOfficeId1);

        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(_termOfOfficeData2.Id));
            Assert.That(result.BeginDate, Is.EqualTo(_termOfOfficeData2.BeginDate));
            Assert.That(result.EndDate, Is.EqualTo(_termOfOfficeData2.EndDate));
            Assert.That(result.IsGeneralElection, Is.EqualTo(_termOfOfficeData2.IsGeneralElection));
            Assert.That(result.Id, Is.EqualTo(_termOfOfficeData2.Id));
            Assert.That(result.Id, Is.EqualTo(_termOfOfficeData2.Id));
        });
    }
}
