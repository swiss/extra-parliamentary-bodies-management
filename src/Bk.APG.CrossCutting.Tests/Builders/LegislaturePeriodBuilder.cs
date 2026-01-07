using Bk.APG.Business.Models;
using Bogus;

namespace Bk.APG.CrossCutting.Tests.Builders;

public class LegislaturePeriodBuilder
{
    private readonly Faker _faker = new();
    private Guid _id;
    private DateOnly _electionDate;
    private DateOnly _startDate;
    private DateOnly _endDate;
    private string _textDe;

    public LegislaturePeriodBuilder()
    {
        _id = _faker.Random.Guid();
        _electionDate = _faker.Date.PastDateOnly();
        _startDate = _faker.Date.PastDateOnly();
        _endDate = _faker.Date.FutureDateOnly();
        _textDe = _faker.Random.String();
    }

    public LegislaturePeriodBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public LegislaturePeriodBuilder WithElectionDate(DateOnly electionDate)
    {
        _electionDate = electionDate;
        return this;
    }

    public LegislaturePeriodBuilder WithStartDate(DateOnly startDate)
    {
        _startDate = startDate;
        return this;
    }

    public LegislaturePeriodBuilder WithEndDate(DateOnly endDate)
    {
        _endDate = endDate;
        return this;
    }

    public LegislaturePeriodBuilder WithText(string text)
    {
        _textDe = text;
        return this;
    }

    public LegislaturePeriod Build()
    {
        return new LegislaturePeriod
        {
            Id = _id,
            ElectionDate = _electionDate,
            StartDate = _startDate,
            EndDate = _endDate,
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
