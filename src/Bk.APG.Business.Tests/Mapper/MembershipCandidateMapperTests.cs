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
}
