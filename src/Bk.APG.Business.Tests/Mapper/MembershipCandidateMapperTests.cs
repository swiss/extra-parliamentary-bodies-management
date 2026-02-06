using Bk.APG.Business.Mapper;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Mapper;

[TestFixture]
internal class MembershipCandidateMapperTests
{
    [Test]
    public void MapToMembershipCandidateDetailDto_ShouldMapToDto()
    {
        var membership = new MembershipCandidateBuilder()
            .WithId(Guid.Parse("8697753c-f57a-4805-b1e9-bb1e546f4101"))
            .WithFunction(new FunctionBuilder().Build())
            .WithGeneralElectionCommittee(new GeneralElectionCommitteeBuilder().WithGermanDescription("Test").Build())
            .WithLanguage(new LanguageBuilder().Build())
            .WithGender(new GenderBuilder().Build())
            .Build();

        var dto = MembershipCandidateMapper.ToMembershipCandidateDetailDto(membership);

        Assert.That(dto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(membership.Id));
            Assert.That(dto.Function, Is.EqualTo(membership.FunctionName));
            Assert.That(dto.FunctionId, Is.EqualTo(membership.FunctionId));
            Assert.That(dto.BeginDate, Is.EqualTo(membership.BeginDate));
            Assert.That(dto.EndDate, Is.EqualTo(membership.EndDate));
            Assert.That(dto.Language, Is.EqualTo(membership.Language!.GetText()));
            Assert.That(dto.ElectionType, Is.EqualTo(membership.ElectionType!.GetText()));
            Assert.That(dto.ElectionTypeId, Is.EqualTo(membership.ElectionTypeId));
            Assert.That(dto.Gender, Is.EqualTo(membership.Gender!.GetText()));
            Assert.That(dto.Remarks, Is.EqualTo(membership.Remarks));
            Assert.That(dto.RemarksStatus, Is.EqualTo(membership.RemarksStatus));
            Assert.That(dto.NeedsAttention, Is.EqualTo(membership.NeedsAttention));
            Assert.That(dto.IsSelected, Is.EqualTo(membership.IsSelected));
        });
    }

    [Test]
    public void MapToMembershipCandidateUpdateDto_ShouldMapToDto()
    {
        var person = new PersonBuilder().Build();
        var generalElectionCommittee = new GeneralElectionCommitteeBuilder().Build();

        var membership = new MembershipCandidateBuilder()
            .WithId(Guid.Parse("70de08ef-2bb6-4d98-8db9-5b2d7b44c4b1"))
            .WithPerson(person)
            .WithGeneralElectionCommittee(generalElectionCommittee)
            .WithSurname("Doe")
            .WithGivenName("Jane")
            .WithBirthYear(1984)
            .WithLanguageId(Guid.Parse("e7d2d12a-95a1-43ad-9d7e-16e8a8c0ce6f"))
            .WithGenderId(Guid.Parse("b87094c1-6f65-4d54-9d6a-0a1f020b01ae"))
            .WithBeginDate(new DateOnly(2020, 1, 1))
            .WithEndDate(new DateOnly(2024, 12, 31))
            .WithElectionTypeId(Guid.Parse("3f8d3d1a-2a48-45b4-8d8a-6b58cd0a7e02"))
            .WithFunctionId(Guid.Parse("c1044bf3-4e4c-4b09-ae5b-0a89df14f86d"))
            .Build();

        var dto = MembershipCandidateMapper.ToMembershipCandidateUpdateDto(membership);

        Assert.That(dto, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(dto.Id, Is.EqualTo(membership.Id));
            Assert.That(dto.PersonId, Is.EqualTo(membership.PersonId));
            Assert.That(dto.Surname, Is.EqualTo(membership.Surname));
            Assert.That(dto.GivenName, Is.EqualTo(membership.GivenName));
            Assert.That(dto.BirthYear, Is.EqualTo(membership.BirthYear));
            Assert.That(dto.GenderId, Is.EqualTo(membership.GenderId));
            Assert.That(dto.LanguageId, Is.EqualTo(membership.LanguageId));
            Assert.That(dto.MaximumEmploymentLevel, Is.EqualTo(membership.MaximumEmploymentLevel));
            Assert.That(dto.BeginDate, Is.EqualTo(membership.BeginDate));
            Assert.That(dto.EndDate, Is.EqualTo(membership.EndDate));
            Assert.That(dto.ElectionTypeId, Is.EqualTo(membership.ElectionTypeId));
            Assert.That(dto.FunctionId, Is.EqualTo(membership.FunctionId));
            Assert.That(dto.ElectionOfficeId, Is.EqualTo(membership.ElectionOfficeId));
            Assert.That(dto.MembershipAdditionId, Is.EqualTo(membership.MembershipAdditionId));
            Assert.That(dto.InCorrelationWithFederalDuty, Is.EqualTo(membership.InCorrelationWithFederalDuty));
            Assert.That(dto.JustificationLongerDuty, Is.EqualTo(membership.JustificationLongerDuty));
            Assert.That(dto.JustificationShorterDuty, Is.EqualTo(membership.JustificationShorterDuty));
            Assert.That(dto.JustificationMemberInFederalDuty, Is.EqualTo(membership.JustificationMemberInFederalDuty));
            Assert.That(dto.JustificationMemberInFederalAssembly, Is.EqualTo(membership.JustificationMemberInFederalAssembly));
            Assert.That(dto.RequirementsProfile, Is.EqualTo(membership.RequirementsProfile));
            Assert.That(dto.RowVersion, Is.EqualTo(membership.RowVersion));
            Assert.That(dto.EstimatedTermOfOffice, Is.EqualTo(membership.EstimatedTermOfOffice));
            Assert.That(dto.CurrentTermOfOffice, Is.EqualTo(membership.CurrentTermOfOffice));
            Assert.That(dto.NeedsLongerDutyJustification, Is.EqualTo(membership.NeedsLongerDutyJustification));
            Assert.That(dto.NeedsShorterDutyJustification, Is.EqualTo(membership.NeedsShorterDutyJustification));
            Assert.That(dto.NeedsFederalDutyJustification, Is.EqualTo(membership.NeedsFederalDutyJustification));
            Assert.That(dto.NeedsFederalAssemblyJustification, Is.EqualTo(membership.NeedsFederalAssemblyJustification));
            Assert.That(dto.NeedsRequirementsProfile, Is.EqualTo(membership.NeedsRequirementsProfile));
            Assert.That(dto.MaximumDurationExceeded, Is.EqualTo(membership.MaximumDurationExceeded));
            Assert.That(dto.HasFederalAssemblyAuthoritiesCommissionConflict, Is.EqualTo(membership.HasFederalAssemblyAuthoritiesCommissionConflict));
        }
    }
}
