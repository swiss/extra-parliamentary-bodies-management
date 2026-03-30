using System.Security.Claims;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting;
using Bk.APG.CrossCutting.Configuration;
using Bk.APG.CrossCutting.Exception;
using Bk.APG.CrossCutting.Tests.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Bk.APG.Business.Tests.Services;

[TestFixture]
internal class AuthorizationServiceTests
{
    private AuthorizationService _authorizationService = null!;
    private readonly IHttpContextAccessor _httpContextAccessorMock = Substitute.For<IHttpContextAccessor>();
    private readonly IEiamAssignmentRepository _eiamAssignmentRepository = Substitute.For<IEiamAssignmentRepository>();
    private readonly IOptions<AuthorizationOptions> _authorizationOptionsMock = Substitute.For<IOptions<AuthorizationOptions>>();
    private readonly ILogger<AuthorizationService> _logger = NullLogger<AuthorizationService>.Instance;

    private readonly AuthorizationOptions _authorizationOptions = new()
    {
        Allow = nameof(AuthorizationOptions.Allow),
        Admin = nameof(AuthorizationOptions.Admin),
        Department = nameof(AuthorizationOptions.Department),
        Office = nameof(AuthorizationOptions.Office),
        Observer = nameof(AuthorizationOptions.Observer),
        Secretariat = nameof(AuthorizationOptions.Secretariat)
    };

    [SetUp]
    public void SetUp()
    {
        _authorizationOptionsMock.Value.Returns(_authorizationOptions);

        _authorizationService = new AuthorizationService(_httpContextAccessorMock, _eiamAssignmentRepository, _authorizationOptionsMock, _logger);
        _httpContextAccessorMock.HttpContext?.User.IsInRole(_authorizationOptions.Allow).Returns(true);
    }

    [TearDown]
    public void TearDown()
    {
        _httpContextAccessorMock.ClearSubstitute();
    }

    [Test]
    public void GetCurrentUserName_WhenClaimExists_ReturnsCurrentUserNameClaim()
    {
        var httpContext = new DefaultHttpContext();
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.Email, "Foo")]);
        httpContext.User.AddIdentity(identity);

        _httpContextAccessorMock.HttpContext.Returns(httpContext);

        var actual = _authorizationService.GetCurrentUserName();

        Assert.That(actual, Is.EqualTo("Foo"));
    }

    [Test]
    public void GetCurrentUserName_WhenNoClaimExists_ThrowsOnNoCurrentUserNameClaim()
    {
        var httpContext = new DefaultHttpContext();

        _httpContextAccessorMock.HttpContext.Returns(httpContext);

        Assert.That(() => _authorizationService.GetCurrentUserName(), Throws.InstanceOf<InvalidOperationException>());
    }

    [TestCase(false, false, false, false, false, false, false)]
    [TestCase(false, true, false, false, false, false, false)]
    [TestCase(true, true, false, false, false, false, true)]
    [TestCase(true, false, true, false, false, false, false)]
    [TestCase(true, false, false, true, false, false, false)]
    [TestCase(true, false, false, false, true, false, false)]
    [TestCase(true, false, false, false, false, true, false)]
    public void IsAdmin(bool hasAllowRole, bool hasAdminRole, bool hasSecretariatRole, bool hasDepartmentRole, bool hasOfficeRole, bool hasObserverRole, bool expected)
    {
        _httpContextAccessorMock.HttpContext!.User.IsInRole(_authorizationOptions.Allow).Returns(hasAllowRole);
        _httpContextAccessorMock.HttpContext.User.IsInRole(_authorizationOptions.Admin).Returns(hasAdminRole);
        _httpContextAccessorMock.HttpContext.User.IsInRole(_authorizationOptions.Secretariat).Returns(hasSecretariatRole);
        _httpContextAccessorMock.HttpContext.User.IsInRole(_authorizationOptions.Department).Returns(hasDepartmentRole);
        _httpContextAccessorMock.HttpContext.User.IsInRole(_authorizationOptions.Office).Returns(hasOfficeRole);
        _httpContextAccessorMock.HttpContext.User.IsInRole(_authorizationOptions.Observer).Returns(hasObserverRole);

        var isAdmin = _authorizationService.IsAdmin;

        Assert.That(isAdmin, Is.EqualTo(expected));
    }

    [TestCase(false, false, false, false, false, false, false)]
    [TestCase(false, true, false, false, false, false, false)]
    [TestCase(false, false, true, false, false, false, false)]
    [TestCase(true, false, true, false, false, false, true)]
    [TestCase(true, false, false, true, false, false, false)]
    [TestCase(true, false, false, false, true, false, false)]
    [TestCase(true, false, false, false, false, true, false)]
    public void IsSecretariat(bool hasAllowRole, bool hasAdminRole, bool hasSecretariatRole, bool hasDepartmentRole, bool hasOfficeRole, bool hasObserverRole, bool expected)
    {
        _httpContextAccessorMock.HttpContext!.User.IsInRole(_authorizationOptions.Allow).Returns(hasAllowRole);
        _httpContextAccessorMock.HttpContext.User.IsInRole(_authorizationOptions.Admin).Returns(hasAdminRole);
        _httpContextAccessorMock.HttpContext.User.IsInRole(_authorizationOptions.Secretariat).Returns(hasSecretariatRole);
        _httpContextAccessorMock.HttpContext.User.IsInRole(_authorizationOptions.Department).Returns(hasDepartmentRole);
        _httpContextAccessorMock.HttpContext.User.IsInRole(_authorizationOptions.Office).Returns(hasOfficeRole);
        _httpContextAccessorMock.HttpContext.User.IsInRole(_authorizationOptions.Observer).Returns(hasObserverRole);

        var isSecretariat = _authorizationService.IsSecretariat;

        Assert.That(isSecretariat, Is.EqualTo(expected));
    }

    [TestCase(false, false, false, false, false, false, false)]
    [TestCase(false, true, false, false, false, false, false)]
    [TestCase(false, false, true, false, false, false, false)]
    [TestCase(false, false, false, true, false, false, false)]
    [TestCase(true, false, false, true, false, false, true)]
    [TestCase(true, false, false, false, true, false, false)]
    [TestCase(true, false, false, false, false, true, false)]
    public void IsDepartment(bool hasAllowRole, bool hasAdminRole, bool hasSecretariatRole, bool hasDepartmentRole, bool hasOfficeRole, bool hasObserverRole, bool expected)
    {
        _httpContextAccessorMock.HttpContext!.User.IsInRole(_authorizationOptions.Allow).Returns(hasAllowRole);
        _httpContextAccessorMock.HttpContext.User.IsInRole(_authorizationOptions.Admin).Returns(hasAdminRole);
        _httpContextAccessorMock.HttpContext.User.IsInRole(_authorizationOptions.Secretariat).Returns(hasSecretariatRole);
        _httpContextAccessorMock.HttpContext.User.IsInRole(_authorizationOptions.Department).Returns(hasDepartmentRole);
        _httpContextAccessorMock.HttpContext.User.IsInRole(_authorizationOptions.Office).Returns(hasOfficeRole);
        _httpContextAccessorMock.HttpContext.User.IsInRole(_authorizationOptions.Observer).Returns(hasObserverRole);

        var isDepartment = _authorizationService.IsDepartment;

        Assert.That(isDepartment, Is.EqualTo(expected));
    }

    [TestCase(false, false, false, false, false, false, false)]
    [TestCase(false, true, false, false, false, false, false)]
    [TestCase(false, false, true, false, false, false, false)]
    [TestCase(false, false, false, true, false, false, false)]
    [TestCase(false, false, false, false, true, false, false)]
    [TestCase(true, false, false, false, true, false, true)]
    [TestCase(true, false, false, false, false, true, false)]
    public void IsOffice(bool hasAllowRole, bool hasAdminRole, bool hasSecretariatRole, bool hasDepartmentRole, bool hasOfficeRole, bool hasObserverRole, bool expected)
    {
        _httpContextAccessorMock.HttpContext!.User.IsInRole(_authorizationOptions.Allow).Returns(hasAllowRole);
        _httpContextAccessorMock.HttpContext.User.IsInRole(_authorizationOptions.Admin).Returns(hasAdminRole);
        _httpContextAccessorMock.HttpContext.User.IsInRole(_authorizationOptions.Secretariat).Returns(hasSecretariatRole);
        _httpContextAccessorMock.HttpContext.User.IsInRole(_authorizationOptions.Department).Returns(hasDepartmentRole);
        _httpContextAccessorMock.HttpContext.User.IsInRole(_authorizationOptions.Office).Returns(hasOfficeRole);
        _httpContextAccessorMock.HttpContext.User.IsInRole(_authorizationOptions.Observer).Returns(hasObserverRole);

        var isOffice = _authorizationService.IsOffice;

        Assert.That(isOffice, Is.EqualTo(expected));
    }

    [TestCase(false, false, false, false, false, false, false)]
    [TestCase(false, true, false, false, false, false, false)]
    [TestCase(false, false, true, false, false, false, false)]
    [TestCase(false, false, false, true, false, false, false)]
    [TestCase(false, false, false, false, true, false, false)]
    [TestCase(false, false, false, false, false, true, false)]
    [TestCase(true, false, false, false, false, true, true)]
    public void IsObserver(bool hasAllowRole, bool hasAdminRole, bool hasSecretariatRole, bool hasDepartmentRole, bool hasOfficeRole, bool hasObserverRole, bool expected)
    {
        _httpContextAccessorMock.HttpContext!.User.IsInRole(_authorizationOptions.Allow).Returns(hasAllowRole);
        _httpContextAccessorMock.HttpContext.User.IsInRole(_authorizationOptions.Admin).Returns(hasAdminRole);
        _httpContextAccessorMock.HttpContext.User.IsInRole(_authorizationOptions.Secretariat).Returns(hasSecretariatRole);
        _httpContextAccessorMock.HttpContext.User.IsInRole(_authorizationOptions.Department).Returns(hasDepartmentRole);
        _httpContextAccessorMock.HttpContext.User.IsInRole(_authorizationOptions.Office).Returns(hasOfficeRole);
        _httpContextAccessorMock.HttpContext.User.IsInRole(_authorizationOptions.Observer).Returns(hasObserverRole);

        var isObserver = _authorizationService.IsObserver;

        Assert.That(isObserver, Is.EqualTo(expected));
    }

    [TestCase(@"789\\1234")]
    [TestCase(@"789\1234")]
    [TestCase(@"1234")]
    public async Task GetDepartment_WhenCalled_ReturnsAssignedDepartment(string claimValue)
    {
        var httpContext = new DefaultHttpContext();
        var identity = new ClaimsIdentity(
        [
            new Claim(CustomClaimTypes.ProfiledUnitExtId, claimValue),
            new Claim(ClaimTypes.Role, _authorizationOptions.Department),
            new Claim(ClaimTypes.Role, _authorizationOptions.Allow)
        ]);
        httpContext.User.AddIdentity(identity);

        var department = new DepartmentBuilder().Build();
        var committees = new List<Committee>
        {
            new CommitteeBuilder().WithId(new Guid("afbe0500-e75a-4ef9-8b5f-df1791276102")).WithDepartment(department).Build()
        };
        department.Committees = committees;

        var eiamAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "1234",
            Role = Role.Department,
            Department = department,
            DepartmentId = department.Id
        };

        _eiamAssignmentRepository.GetByExternalId("1234").Returns(eiamAssignment);

        _httpContextAccessorMock.HttpContext.Returns(httpContext);

        var result = await _authorizationService.GetDepartment();

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(department.Id));
    }

    [Test]
    public async Task IsCommitteeAssigned_WhenCalledForAdmin_ReturnsFalse()
    {
        var httpContext = new DefaultHttpContext();
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, _authorizationOptions.Admin),
            new Claim(ClaimTypes.Role, _authorizationOptions.Allow)
        ]);
        httpContext.User.AddIdentity(identity);

        _httpContextAccessorMock.HttpContext.Returns(httpContext);

        var committeeId = new Guid("afbe0500-e75a-4ef9-8b5f-df1791276102");
        var isAssigned = await _authorizationService.IsCommitteeAssigned(committeeId);

        Assert.That(isAssigned, Is.False);
    }

    [Test]
    public async Task IsCommitteeAssigned_WhenCalledForObserver_ReturnsFalse()
    {
        var httpContext = new DefaultHttpContext();
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, _authorizationOptions.Observer),
            new Claim(ClaimTypes.Role, _authorizationOptions.Allow)
        ]);
        httpContext.User.AddIdentity(identity);

        _httpContextAccessorMock.HttpContext.Returns(httpContext);

        var committeeId = new Guid("afbe0500-e75a-4ef9-8b5f-df1791276102");
        var isAssigned = await _authorizationService.IsCommitteeAssigned(committeeId);

        Assert.That(isAssigned, Is.False);
    }

    [TestCase("afbe0500-e75a-4ef9-8b5f-df1791276102", true)]
    [TestCase("e4d983fa-e50c-47a2-a4b8-2ac0660add3f", false)]
    public async Task IsCommitteeAssigned_WhenCalledForDepartment_ReturnsIsAssigned(string guidAsString, bool expected)
    {
        var claimValue = @"789\\1234";
        var httpContext = new DefaultHttpContext();
        var identity = new ClaimsIdentity(
        [
            new Claim(CustomClaimTypes.ProfiledUnitExtId, claimValue),
            new Claim(ClaimTypes.Role, _authorizationOptions.Department),
            new Claim(ClaimTypes.Role, _authorizationOptions.Allow)
        ]);
        httpContext.User.AddIdentity(identity);

        var department = new DepartmentBuilder().Build();
        var committees = new List<Committee>
        {
            new CommitteeBuilder().WithId(new Guid("afbe0500-e75a-4ef9-8b5f-df1791276102")).WithDepartment(department).Build()
        };
        department.Committees = committees;

        var eiamAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "1234",
            Role = Role.Department,
            Department = department,
            DepartmentId = department.Id
        };

        _eiamAssignmentRepository.GetByExternalId("1234").Returns(eiamAssignment);

        _httpContextAccessorMock.HttpContext.Returns(httpContext);

        var isAssigned = await _authorizationService.IsCommitteeAssigned(new Guid(guidAsString));

        Assert.That(isAssigned, Is.EqualTo(expected));
    }

    [TestCase("afbe0500-e75a-4ef9-8b5f-df1791276102", true)]
    [TestCase("e4d983fa-e50c-47a2-a4b8-2ac0660add3f", false)]
    public async Task IsCommitteeAssigned_WhenCalledForOffice_ReturnsIsAssigned(string guidAsString, bool expected)
    {
        var claimValue = @"789\\12345";
        var httpContext = new DefaultHttpContext();
        var identity = new ClaimsIdentity(
        [
            new Claim(CustomClaimTypes.ProfiledUnitExtId, claimValue),
            new Claim(ClaimTypes.Role, _authorizationOptions.Office),
            new Claim(ClaimTypes.Role, _authorizationOptions.Allow)
        ]);
        httpContext.User.AddIdentity(identity);

        var office = new OfficeBuilder().Build();
        var committees = new List<Committee>
        {
            new CommitteeBuilder().WithId(new Guid("afbe0500-e75a-4ef9-8b5f-df1791276102")).WithOffice(office).Build()
        };
        office.Committees = committees;

        var eiamAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "12345",
            Role = Role.Office,
            Office = office,
            OfficeId = office.Id
        };

        _eiamAssignmentRepository.GetByExternalId("12345").Returns(eiamAssignment);

        _httpContextAccessorMock.HttpContext.Returns(httpContext);

        var isAssigned = await _authorizationService.IsCommitteeAssigned(new Guid(guidAsString));

        Assert.That(isAssigned, Is.EqualTo(expected));
    }

    [TestCase("afbe0500-e75a-4ef9-8b5f-df1791276102", true)]
    [TestCase("e4d983fa-e50c-47a2-a4b8-2ac0660add3f", false)]
    public async Task IsCommitteeAssigned_WhenCalledForSecretariat_ReturnsIsAssigned(string guidAsString, bool expected)
    {
        var claimValue = @"789\\12345";
        var httpContext = new DefaultHttpContext();
        var identity = new ClaimsIdentity(
        [
            new Claim(CustomClaimTypes.ProfiledUnitExtId, claimValue),
            new Claim(ClaimTypes.Role, _authorizationOptions.Secretariat),
            new Claim(ClaimTypes.Role, _authorizationOptions.Allow)
        ]);
        httpContext.User.AddIdentity(identity);

        var committee = new CommitteeBuilder().WithId(new Guid("afbe0500-e75a-4ef9-8b5f-df1791276102")).Build();

        var eiamAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "12345",
            Role = Role.Secretariat,
            Committee = committee,
            CommitteeId = committee.Id
        };

        _eiamAssignmentRepository.GetByExternalId("12345").Returns(eiamAssignment);

        _httpContextAccessorMock.HttpContext.Returns(httpContext);

        var isAssigned = await _authorizationService.IsCommitteeAssigned(new Guid(guidAsString));

        Assert.That(isAssigned, Is.EqualTo(expected));
    }

    [TestCase(@"test\\1234", "1234")]
    [TestCase(@"test\1234", "1234")]
    [TestCase("1234", "1234")]
    public async Task GetCurrentEiamAssignment_WhenClaimExists_ResolvesExternalIdFromClaim(string claimValue, string expectedExternalId)
    {
        var httpContext = new DefaultHttpContext();
        var identity = new ClaimsIdentity(
        [
            new Claim(CustomClaimTypes.ProfiledUnitExtId, claimValue),
            new Claim(ClaimTypes.Role, _authorizationOptions.Department),
            new Claim(ClaimTypes.Role, _authorizationOptions.Allow)
        ]);
        httpContext.User.AddIdentity(identity);

        var eiamAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = expectedExternalId,
            Role = Role.Department
        };

        _eiamAssignmentRepository.GetByExternalId(expectedExternalId).Returns(eiamAssignment);
        _httpContextAccessorMock.HttpContext.Returns(httpContext);

        var actual = await _authorizationService.GetCurrentEiamAssignment();

        Assert.That(actual.ExternalId, Is.EqualTo(expectedExternalId));
    }

    [Test]
    public async Task GetCurrentEiamAssignment_WhenUserIsAdmin_ResolvesAdminExternalId()
    {
        var httpContext = new DefaultHttpContext();
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, _authorizationOptions.Admin),
            new Claim(ClaimTypes.Role, _authorizationOptions.Allow)
        ]);
        httpContext.User.AddIdentity(identity);

        var eiamAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "Admin",
            Role = Role.Admin
        };

        _eiamAssignmentRepository.GetByExternalId("Admin").Returns(eiamAssignment);
        _httpContextAccessorMock.HttpContext.Returns(httpContext);

        var actual = await _authorizationService.GetCurrentEiamAssignment();

        Assert.That(actual.ExternalId, Is.EqualTo("Admin"));
    }

    [Test]
    public void GetCurrentEiamAssignment_WhenNoClaimExists_ThrowsInvalidOperationException()
    {
        var httpContext = new DefaultHttpContext();
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, _authorizationOptions.Department),
            new Claim(ClaimTypes.Role, _authorizationOptions.Allow)
        ]);
        httpContext.User.AddIdentity(identity);

        _httpContextAccessorMock.HttpContext.Returns(httpContext);

        Assert.That(async () => await _authorizationService.GetCurrentEiamAssignment(), Throws.InstanceOf<InvalidOperationException>());
    }

    [Test]
    public void GetCurrentEiamAssignment_WhenRoleMismatch_ThrowsAuthorizationException()
    {
        var httpContext = new DefaultHttpContext();
        var identity = new ClaimsIdentity(
        [
            new Claim(CustomClaimTypes.ProfiledUnitExtId, "1234"),
            new Claim(ClaimTypes.Role, _authorizationOptions.Secretariat),
            new Claim(ClaimTypes.Role, _authorizationOptions.Allow)
        ]);
        httpContext.User.AddIdentity(identity);

        var eiamAssignment = new EiamAssignment
        {
            Id = Guid.NewGuid(),
            ExternalId = "1234",
            Role = Role.Department
        };

        _eiamAssignmentRepository.GetByExternalId("1234").Returns(eiamAssignment);
        _httpContextAccessorMock.HttpContext.Returns(httpContext);

        Assert.That(async () => await _authorizationService.GetCurrentEiamAssignment(), Throws.InstanceOf<AuthorizationException>());
    }
}
