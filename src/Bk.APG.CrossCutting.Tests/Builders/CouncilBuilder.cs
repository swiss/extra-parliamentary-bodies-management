using Bk.APG.Business.Models;
using Bogus;

namespace Bk.APG.CrossCutting.Tests.Builders;

public class CouncilBuilder
{
    private readonly Faker _faker = new();
    private Guid _id;
    private string _textDe;

    public CouncilBuilder()
    {
        _id = _faker.Random.Guid();
        _textDe = _faker.Random.String();
    }

    public CouncilBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public CouncilBuilder WithText(string text)
    {
        _textDe = text;
        return this;
    }

    public Council Build()
    {
        return new Council
        {
            Id = _id,
            TextDe = _textDe,
            TextFr = string.Empty,
            TextIt = string.Empty,
            TextRm = string.Empty,
            DescriptionDe = string.Empty,
            DescriptionFr = string.Empty,
            DescriptionIt = string.Empty,
            DescriptionRm = string.Empty,
            Uri = string.Empty,
            Created = default,
            CreatedBy = string.Empty,
            Modified = default,
            ModifiedBy = string.Empty,
            IsDeleted = false,
            Sort = 0
        };
    }
}
