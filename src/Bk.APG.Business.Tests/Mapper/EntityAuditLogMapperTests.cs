using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Tests.Mapper;

[TestFixture]
internal class EntityAuditLogMapperTests
{
    [Test]
    public void MapToDto_WithChangesHavingBothOriginalAndNewValues_ShouldMapToBothBeforeAndAfter()
    {
        const string auditData = """[{"newValue": "foo", "columnName": "column", "originalValue": "bar"}]""";
        var entityAuditLog = new EntityAuditLog
        {
            AuditDate = DateTime.UtcNow,
            AuditAction = "UPDATE",
            AuditUser = "test@example.com",
            AuditData = auditData,
            EntityPrimaryKey = "123",
            EntityType = "TestEntity",
            EntitySnapshot = "{}"
        };

        var dto = EntityAuditLogMapper.MapToDto(entityAuditLog);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.AuditDate, Is.EqualTo(entityAuditLog.AuditDate));
            Assert.That(dto.AuditAction, Is.EqualTo(entityAuditLog.AuditAction));
            Assert.That(dto.AuditUser, Is.EqualTo(entityAuditLog.AuditUser));
            Assert.That(dto.AuditDataBefore.Single().ColumnName, Is.EqualTo("column"));
            Assert.That(dto.AuditDataBefore.Single().Data, Is.EqualTo("bar"));
            Assert.That(dto.AuditDataAfter.Single().ColumnName, Is.EqualTo("column"));
            Assert.That(dto.AuditDataAfter.Single().Data, Is.EqualTo("foo"));
        });
    }

    [Test]
    public void MapToDto_WithChangesHavingOnlyNewValues_ShouldMapOnlyToAfter()
    {
        const string auditData = """[{"newValue": "foo", "columnName": "column"}]""";
        var entityAuditLog = new EntityAuditLog
        {
            AuditDate = DateTime.UtcNow,
            AuditAction = "INSERT",
            AuditUser = "test@example.com",
            AuditData = auditData,
            EntityPrimaryKey = "123",
            EntityType = "TestEntity",
            EntitySnapshot = "{}"
        };

        var dto = EntityAuditLogMapper.MapToDto(entityAuditLog);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.AuditDataBefore, Is.Empty);
            Assert.That(dto.AuditDataAfter.Single().ColumnName, Is.EqualTo("column"));
            Assert.That(dto.AuditDataAfter.Single().Data, Is.EqualTo("foo"));
        });
    }

    [Test]
    public void MapToDto_WithChangesHavingOnlyOriginalValues_ShouldMapOnlyToBefore()
    {
        const string auditData = """[{"originalValue": "foo", "columnName": "column"}]""";
        var entityAuditLog = new EntityAuditLog
        {
            AuditDate = DateTime.UtcNow,
            AuditAction = "DELETE",
            AuditUser = "test@example.com",
            AuditData = auditData,
            EntityPrimaryKey = "123",
            EntityType = "TestEntity",
            EntitySnapshot = "{}"
        };

        var dto = EntityAuditLogMapper.MapToDto(entityAuditLog);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.AuditDataBefore.Single().ColumnName, Is.EqualTo("column"));
            Assert.That(dto.AuditDataBefore.Single().Data, Is.EqualTo("foo"));
            Assert.That(dto.AuditDataAfter, Is.Empty);
        });
    }

    [Test]
    public void MapToDto_WithNullAuditData_ShouldReturnEmptyStrings()
    {
        var entityAuditLog = new EntityAuditLog
        {
            AuditDate = DateTime.UtcNow,
            AuditAction = "UPDATE",
            AuditUser = "test@example.com",
            AuditData = null,
            EntityPrimaryKey = "123",
            EntityType = "TestEntity",
            EntitySnapshot = "{}"
        };

        var dto = EntityAuditLogMapper.MapToDto(entityAuditLog);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.AuditDataBefore, Is.Empty);
            Assert.That(dto.AuditDataAfter, Is.Empty);
        });
    }

    [Test]
    public void MapToDto_WithEmptyAuditData_ShouldReturnEmptyStrings()
    {
        var entityAuditLog = new EntityAuditLog
        {
            AuditDate = DateTime.UtcNow,
            AuditAction = "UPDATE",
            AuditUser = "test@example.com",
            AuditData = string.Empty,
            EntityPrimaryKey = "123",
            EntityType = "TestEntity",
            EntitySnapshot = "{}"
        };

        var dto = EntityAuditLogMapper.MapToDto(entityAuditLog);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.AuditDataBefore, Is.Empty);
            Assert.That(dto.AuditDataAfter, Is.Empty);
        });
    }

    [Test]
    public void MapToDto_WithWhitespaceAuditData_ShouldReturnEmptyStrings()
    {
        var entityAuditLog = new EntityAuditLog
        {
            AuditDate = DateTime.UtcNow,
            AuditAction = "UPDATE",
            AuditUser = "test@example.com",
            AuditData = "   ",
            EntityPrimaryKey = "123",
            EntityType = "TestEntity",
            EntitySnapshot = "{}"
        };

        var dto = EntityAuditLogMapper.MapToDto(entityAuditLog);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.AuditDataBefore, Is.Empty);
            Assert.That(dto.AuditDataAfter, Is.Empty);
        });
    }

    [Test]
    public void MapToDto_WithInvalidJson_ShouldReturnEmptyStrings()
    {
        var entityAuditLog = new EntityAuditLog
        {
            AuditDate = DateTime.UtcNow,
            AuditAction = "UPDATE",
            AuditUser = "test@example.com",
            AuditData = "invalid json",
            EntityPrimaryKey = "123",
            EntityType = "TestEntity",
            EntitySnapshot = "{}"
        };

        var dto = EntityAuditLogMapper.MapToDto(entityAuditLog);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.AuditDataBefore, Is.Empty);
            Assert.That(dto.AuditDataAfter, Is.Empty);
        });
    }

    [Test]
    public void MapToDto_WithEmptyJsonArray_ShouldReturnEmptyStrings()
    {
        var entityAuditLog = new EntityAuditLog
        {
            AuditDate = DateTime.UtcNow,
            AuditAction = "UPDATE",
            AuditUser = "test@example.com",
            AuditData = "[]",
            EntityPrimaryKey = "123",
            EntityType = "TestEntity",
            EntitySnapshot = "{}"
        };

        var dto = EntityAuditLogMapper.MapToDto(entityAuditLog);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.AuditDataBefore, Is.Empty);
            Assert.That(dto.AuditDataAfter, Is.Empty);
        });
    }

    [Test]
    public void MapToDto_ShouldMapBasicFieldsCorrectly()
    {
        var expectedDate = new DateTime(2023, 11, 15, 10, 30, 0, DateTimeKind.Utc);
        var entityAuditLog = new EntityAuditLog
        {
            AuditDate = expectedDate,
            AuditAction = "INSERT",
            AuditUser = "user@example.com",
            AuditData = null,
            EntityPrimaryKey = "456",
            EntityType = "TestEntity",
            EntitySnapshot = "{}"
        };

        var dto = EntityAuditLogMapper.MapToDto(entityAuditLog);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.AuditDate, Is.EqualTo(expectedDate));
            Assert.That(dto.AuditAction, Is.EqualTo("INSERT"));
            Assert.That(dto.AuditUser, Is.EqualTo("user@example.com"));
        });
    }

    [Test]
    public void MapToDto_WithInsertAction_ShouldMapEntitySnapshotToAuditDataAfter()
    {
        const string entitySnapshot = """{"Id": "123", "Name": "Test Name", "Email": "test@example.com"}""";
        var entityAuditLog = new EntityAuditLog
        {
            AuditDate = DateTime.UtcNow,
            AuditAction = EntityAuditLog.InsertAction,
            AuditUser = "test@example.com",
            AuditData = null,
            EntityPrimaryKey = "123",
            EntityType = "TestEntity",
            EntitySnapshot = entitySnapshot
        };

        var dto = EntityAuditLogMapper.MapToDto(entityAuditLog);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.AuditDataBefore, Is.Empty);
            Assert.That(dto.AuditDataAfter.Count(), Is.EqualTo(3));
            Assert.That(dto.AuditDataAfter.Any(d => d.ColumnName == "Id" && d.Data == "123"), Is.True);
            Assert.That(dto.AuditDataAfter.Any(d => d.ColumnName == "Name" && d.Data == "Test Name"), Is.True);
            Assert.That(dto.AuditDataAfter.Any(d => d.ColumnName == "Email" && d.Data == "test@example.com"), Is.True);
        });
    }

    [Test]
    public void MapToDto_WithDeleteAction_ShouldMapEntitySnapshotToAuditDataBefore()
    {
        const string entitySnapshot = """{"Id": "456", "Name": "Deleted Item", "Status": "Active"}""";
        var entityAuditLog = new EntityAuditLog
        {
            AuditDate = DateTime.UtcNow,
            AuditAction = EntityAuditLog.DeleteAction,
            AuditUser = "admin@example.com",
            AuditData = null,
            EntityPrimaryKey = "456",
            EntityType = "TestEntity",
            EntitySnapshot = entitySnapshot
        };

        var dto = EntityAuditLogMapper.MapToDto(entityAuditLog);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.AuditDataAfter, Is.Empty);
            Assert.That(dto.AuditDataBefore.Count(), Is.EqualTo(3));
            Assert.That(dto.AuditDataBefore.Any(d => d.ColumnName == "Id" && d.Data == "456"), Is.True);
            Assert.That(dto.AuditDataBefore.Any(d => d.ColumnName == "Name" && d.Data == "Deleted Item"), Is.True);
            Assert.That(dto.AuditDataBefore.Any(d => d.ColumnName == "Status" && d.Data == "Active"), Is.True);
        });
    }

    [Test]
    public void MapToDto_WithInsertActionAndEmptySnapshot_ShouldReturnEmptyAuditDataAfter()
    {
        var entityAuditLog = new EntityAuditLog
        {
            AuditDate = DateTime.UtcNow,
            AuditAction = EntityAuditLog.InsertAction,
            AuditUser = "test@example.com",
            AuditData = null,
            EntityPrimaryKey = "123",
            EntityType = "TestEntity",
            EntitySnapshot = "{}"
        };

        var dto = EntityAuditLogMapper.MapToDto(entityAuditLog);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.AuditDataBefore, Is.Empty);
            Assert.That(dto.AuditDataAfter, Is.Empty);
        });
    }

    [Test]
    public void MapToDto_WithDeleteActionAndNullSnapshot_ShouldReturnEmptyAuditDataBefore()
    {
        var entityAuditLog = new EntityAuditLog
        {
            AuditDate = DateTime.UtcNow,
            AuditAction = EntityAuditLog.DeleteAction,
            AuditUser = "test@example.com",
            AuditData = null,
            EntityPrimaryKey = "123",
            EntityType = "TestEntity",
            EntitySnapshot = string.Empty
        };

        var dto = EntityAuditLogMapper.MapToDto(entityAuditLog);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.AuditDataBefore, Is.Empty);
            Assert.That(dto.AuditDataAfter, Is.Empty);
        });
    }

    [Test]
    public void MapToDto_WithInsertActionAndInvalidSnapshot_ShouldReturnEmptyAuditDataAfter()
    {
        var entityAuditLog = new EntityAuditLog
        {
            AuditDate = DateTime.UtcNow,
            AuditAction = EntityAuditLog.InsertAction,
            AuditUser = "test@example.com",
            AuditData = null,
            EntityPrimaryKey = "123",
            EntityType = "TestEntity",
            EntitySnapshot = "invalid json"
        };

        var dto = EntityAuditLogMapper.MapToDto(entityAuditLog);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.AuditDataBefore, Is.Empty);
            Assert.That(dto.AuditDataAfter, Is.Empty);
        });
    }

    [Test]
    public void MapToDto_WithDeleteActionAndInvalidSnapshot_ShouldReturnEmptyAuditDataBefore()
    {
        var entityAuditLog = new EntityAuditLog
        {
            AuditDate = DateTime.UtcNow,
            AuditAction = EntityAuditLog.DeleteAction,
            AuditUser = "test@example.com",
            AuditData = null,
            EntityPrimaryKey = "123",
            EntityType = "TestEntity",
            EntitySnapshot = "not a valid json object"
        };

        var dto = EntityAuditLogMapper.MapToDto(entityAuditLog);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.AuditDataBefore, Is.Empty);
            Assert.That(dto.AuditDataAfter, Is.Empty);
        });
    }
}
