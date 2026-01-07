using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.Common.Resources;
using Bk.APG.CrossCutting;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
internal class EntityAuditLogServiceTests
{
    private EntityAuditLogService _service = null!;

    private readonly IEntityAuditLogRepository _entityAuditLogRepository = Substitute.For<IEntityAuditLogRepository>();
    private readonly IMasterDataRepository _masterDataRepository = Substitute.For<IMasterDataRepository>();
    private readonly IOccupationRepository _occupationRepository = Substitute.For<IOccupationRepository>();
    private readonly ICantonRepository _cantonRepository = Substitute.For<ICantonRepository>();
    private readonly IPersonRepository _personRepository = Substitute.For<IPersonRepository>();
    private readonly IPersonService _personService = Substitute.For<IPersonService>();

    [SetUp]
    public void SetUp()
    {
        _service = new EntityAuditLogService(_entityAuditLogRepository, _masterDataRepository, _occupationRepository, _cantonRepository, _personRepository, _personService);
    }

    [TearDown]
    public void TearDown()
    {
        _entityAuditLogRepository.ClearSubstitute();
        _masterDataRepository.ClearSubstitute();
        _occupationRepository.ClearSubstitute();
        _cantonRepository.ClearSubstitute();
        _personRepository.ClearSubstitute();
        _personService.ClearSubstitute();
    }

    [Test]
    public async Task GetAuditLogsForEntity_ShouldReturnAuditLog()
    {
        var entityId = Guid.NewGuid().ToString();
        var auditLogs = new PagedResult<EntityAuditLog>
        {
            Index = 0,
            Total = 0,
            Items =
            [
                new EntityAuditLog
                {
                    EntityPrimaryKey = entityId,
                    AuditDate = DateTime.UtcNow,
                    AuditAction = "Update",
                    AuditUser = "Foo User",
                    AuditData = """
                                [
                                    {"newValue": "851f139f-be8c-4370-9532-a127a53201ea", "columnName": "committee_level_id", "originalValue": "4461cb7c-711a-405d-ba3a-a34183638518"}
                                ]
                                """,
                    EntityType = null!,
                    EntitySnapshot = null!
                },
                new EntityAuditLog
                {
                    EntityPrimaryKey = entityId,
                    AuditDate = DateTime.UtcNow,
                    AuditAction = "Insert",
                    AuditUser = "Foo User",
                    EntityType = null!,
                    EntitySnapshot = """{"persons_id": "79d2a16f-c23e-4a98-894d-d25e28b328b9", "occupations_id": "82517502-a472-4aee-bb3f-e4ebe967ecac"}""",
                },
                new EntityAuditLog
                {
                    EntityPrimaryKey = entityId,
                    AuditDate = DateTime.UtcNow,
                    AuditAction = "Insert",
                    AuditUser = "Foo User",
                    EntityType = null!,
                    EntitySnapshot = """{"canton_id": "950538d3-4f21-4e85-aeb6-bf0575c2a8f5"}""",
                },
                new EntityAuditLog
                {
                    EntityPrimaryKey = entityId,
                    AuditDate = DateTime.UtcNow,
                    AuditAction = "Update",
                    AuditUser = "Foo User",
                    AuditData = """
                                [
                                    {"newValue": "123 Main St", "columnName": "address", "originalValue": "456 Oak St"}
                                ]
                                """,
                    EntityType = nameof(Address),
                    EntitySnapshot = null!
                }
            ]
        };
        _entityAuditLogRepository.GetEntityAuditLogs(entityId, Arg.Any<IEnumerable<string>>(), Arg.Any<PagingParameters>(), Arg.Any<string>(), Arg.Any<SortDirection>()).Returns(auditLogs);
        _masterDataRepository.GetById<CommitteeLevel>(new Guid("851f139f-be8c-4370-9532-a127a53201ea")).Returns(new CommitteeLevelBuilder().WithGermanText("Foo").Build());
        _masterDataRepository.GetById<CommitteeLevel>(new Guid("4461cb7c-711a-405d-ba3a-a34183638518")).Returns(new CommitteeLevelBuilder().WithGermanText("Bar").Build());
        _occupationRepository.GetById(new Guid("82517502-a472-4aee-bb3f-e4ebe967ecac")).Returns(new OccupationBuilder().WithGermanText("Job").Build());
        _cantonRepository.GetById(new Guid("950538d3-4f21-4e85-aeb6-bf0575c2a8f5")).Returns(new CantonBuilder().WithTextDe("Kanton").Build());
        _personService.ShouldMaskAddress(Arg.Any<Person>()).Returns(true);

        var result = await _service.GetAuditLogsForEntity(entityId, nameof(Person), [], new PagingParametersDto { PageIndex = 1, PageSize = 1 }, "foo", SortDirection.Asc);

        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Items.Count(), Is.EqualTo(4));
            Assert.That(result.Items.ElementAt(0).AuditDataBefore.Single().Data, Is.EqualTo("Bar"));
            Assert.That(result.Items.ElementAt(0).AuditDataAfter.Single().Data, Is.EqualTo("Foo"));
            Assert.That(result.Items.ElementAt(1).AuditDataAfter.ElementAt(0).Data, Is.EqualTo("79d2a16f-c23e-4a98-894d-d25e28b328b9"));
            Assert.That(result.Items.ElementAt(1).AuditDataAfter.ElementAt(1).Data, Is.EqualTo("Job"));
            Assert.That(result.Items.ElementAt(2).AuditDataAfter.ElementAt(0).Data, Is.EqualTo("Kanton"));
            Assert.That(result.Items.ElementAt(3).AuditDataBefore.Single().Data, Is.EqualTo(BusinessTexts.Person_MaskedAddress));
            Assert.That(result.Items.ElementAt(3).AuditDataAfter.Single().Data, Is.EqualTo(BusinessTexts.Person_MaskedAddress));
        }
    }

    [Test]
    public async Task GetAuditLogsForEntity_ShouldFilterEmptyRelatedEntityIds()
    {
        const string entityId = "foo";
        _entityAuditLogRepository.GetEntityAuditLogs(entityId, Arg.Any<IEnumerable<string>>(), Arg.Any<PagingParameters>(), Arg.Any<string>(), Arg.Any<SortDirection>()).Returns(new PagedResult<EntityAuditLog> { Items = [], Index = 0, Total = 0 });

        await _service.GetAuditLogsForEntity(entityId, "entityType", [string.Empty, " ", null!], new PagingParametersDto { PageSize = 1, PageIndex = 1 }, "sort", SortDirection.Asc);

        await _entityAuditLogRepository.Received(1).GetEntityAuditLogs(entityId, Arg.Is<IEnumerable<string>>(x => !x.Any()), Arg.Any<PagingParameters>(), Arg.Any<string>(), Arg.Any<SortDirection>());
    }
}
