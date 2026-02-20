using Audit.EntityFramework;
using Bk.APG.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Bk.APG.Infrastructure.DataSource;

public class DataContext : AuditDbContext
{
    public const string Schema = "data";

    public DbSet<Person> Persons { get; set; }
    public DbSet<Committee> Committees { get; set; }
    public DbSet<Membership> Memberships { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Canton> Cantons { get; set; }
    public DbSet<CommitteeLevel> CommitteeLevels { get; set; }
    public DbSet<CommitteeType> CommitteeTypes { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<ElectionOffice> ElectionOffices { get; set; }
    public DbSet<ElectionType> ElectionTypes { get; set; }
    public DbSet<Function> Functions { get; set; }
    public DbSet<Gender> Genders { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<Office> Offices { get; set; }
    public DbSet<Salutation> Salutations { get; set; }
    public DbSet<TermOfOffice> TermsOfOffice { get; set; }
    public DbSet<Interest> Interests { get; set; }
    public DbSet<InterestCommittee> InterestCommittees { get; set; }
    public DbSet<InterestFunction> InterestFunctions { get; set; }
    public DbSet<InterestLegalForm> InterestLegalForms { get; set; }
    public DbSet<LegalForm> LegalForms { get; set; }
    public DbSet<ContactPoint> ContactPoints { get; set; }
    public DbSet<ContactPointType> ContactPointTypes { get; set; }
    public DbSet<MembershipAddition> MembershipAdditions { get; set; }
    public DbSet<GeneralGenderMeasure> GeneralGenderMeasures { get; set; }
    public DbSet<GeneralLanguageMeasure> GeneralLanguageMeasures { get; set; }
    public DbSet<AppointmentDecision> AppointmentDecisions { get; set; }
    public DbSet<AppointmentDecisionLinkType> AppointmentDecisionLinkTypes { get; set; }
    public DbSet<AppointmentDecisionType> AppointmentDecisionTypes { get; set; }
    public DbSet<LegislaturePeriod> LegislaturePeriods { get; set; }
    public DbSet<DocumentStorage> DocumentStorages { get; set; }
    public DbSet<TermOfOfficeDate> TermOfOfficeDates { get; set; }
    public DbSet<Council> Councils { get; set; }
    public DbSet<GeneralElectionCommittee> GeneralElectionCommittees { get; set; }
    public DbSet<MembershipCandidate> MembershipCandidates { get; set; }
    public DbSet<CandidateListState> CandidateListStates { get; set; }
    public DbSet<WorklistTask> WorklistTasks { get; set; }
    public DbSet<WorklistTaskType> WorklistTaskTypes { get; set; }
    public DbSet<WorklistTaskState> WorklistTaskStates { get; set; }
    public DbSet<MembershipCandidateLogMessage> MembershipCandidateLogMessages { get; set; }
    public DbSet<Occupation> Occupations { get; set; }
    public DbSet<EiamAssignment> EiamAssignments { get; set; }
    public DbSet<EntityAuditLog> EntityAuditLog { get; set; }
    public DbSet<PersonOccupation> PersonOccupations { get; set; }
    public DbSet<ApgGeneralSettings> ApgGeneralSettings { get; set; }
    public DbSet<FormLetterSenderFunction> FormLetterSenderFunctions { get; set; }
    public DbSet<FormLetterSender> FormLetterSenders { get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
        optionsBuilder.ConfigureWarnings(w => w.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning));

        base.OnConfiguring(optionsBuilder);
    }
}
