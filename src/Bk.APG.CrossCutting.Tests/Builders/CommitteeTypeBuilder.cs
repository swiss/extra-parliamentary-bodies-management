using Bk.APG.Business.Models;
using Bogus;

namespace Bk.APG.CrossCutting.Tests.Builders;

public class CommitteeTypeBuilder
{
    private readonly Faker _faker = new();

    private Guid _id;
    private int _ogdId;
    private readonly DateTime _created;
    private readonly string _createdBy;
    private readonly DateTime _modified;
    private readonly string _modifiedBy;
    private string _uri;
    private readonly bool _isDeleted;
    private string _textDe;
    private readonly string _textFr;
    private readonly string _textIt;
    private readonly string _textRm;
    private string _descriptionDe;
    private string _descriptionFr;
    private string _descriptionIt;
    private string _descriptionRm;
    private readonly int _sort;
    private readonly int _oldId;
    private double _femaleThreshold;
    private double _maleThreshold;
    private int? _germanMinimalThreshold;
    private int? _frenchMinimalThreshold;
    private int? _italianMinimalThreshold;
    private int? _romanshMinimalThreshold;
    private double? _germanThresholdPercentage;
    private double? _frenchThresholdPercentage;
    private double? _italianThresholdPercentage;
    private double? _romanshThresholdPercentage;

    public CommitteeTypeBuilder()
    {
        _id = _faker.Random.Guid();
        _ogdId = _faker.Random.Int();
        _created = _faker.Date.Past();
        _createdBy = _faker.Person.Email;
        _modified = _faker.Date.Past();
        _modifiedBy = _faker.Person.Email;
        _uri = _faker.Internet.Url();
        _isDeleted = false;
        _textDe = _faker.Lorem.Text();
        _textFr = _faker.Lorem.Text();
        _textIt = _faker.Lorem.Text();
        _textRm = _faker.Lorem.Text();
        _descriptionDe = _faker.Lorem.Text();
        _descriptionFr = _faker.Lorem.Text();
        _descriptionIt = _faker.Lorem.Text();
        _descriptionRm = _faker.Lorem.Text();
        _sort = _faker.Random.Int(0);
        _oldId = _faker.Random.Int(1);
        _femaleThreshold = _faker.Random.Double();
        _maleThreshold = _faker.Random.Double();
        _germanMinimalThreshold = _faker.Random.Int().OrNull(_faker);
        _frenchMinimalThreshold = _faker.Random.Int().OrNull(_faker);
        _italianMinimalThreshold = _faker.Random.Int().OrNull(_faker);
        _romanshMinimalThreshold = _faker.Random.Int().OrNull(_faker);
        _germanThresholdPercentage = _faker.Random.Double().OrNull(_faker);
        _frenchThresholdPercentage = _faker.Random.Double().OrNull(_faker);
        _italianThresholdPercentage = _faker.Random.Double().OrNull(_faker);
        _romanshThresholdPercentage = _faker.Random.Double().OrNull(_faker);
    }

    public CommitteeTypeBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public CommitteeTypeBuilder WithOgdId(int id)
    {
        _ogdId = id;
        return this;
    }

    public CommitteeTypeBuilder WithUri(string uri)
    {
        _uri = uri;
        return this;
    }

    public CommitteeTypeBuilder WithGermanText(string text)
    {
        _textDe = text;
        return this;
    }

    public CommitteeTypeBuilder WithGermanDescription(string description)
    {
        _descriptionDe = description;
        return this;
    }

    public CommitteeTypeBuilder WithFrenchDescription(string description)
    {
        _descriptionFr = description;
        return this;
    }

    public CommitteeTypeBuilder WithItalianDescription(string description)
    {
        _descriptionIt = description;
        return this;
    }

    public CommitteeTypeBuilder WithRomanshDescription(string description)
    {
        _descriptionRm = description;
        return this;
    }

    public CommitteeTypeBuilder WithFemaleAndMaleThreshold(double female, double male)
    {
        _femaleThreshold = female;
        _maleThreshold = male;
        return this;
    }

    public CommitteeTypeBuilder WithLanguagesThreshold(int? german, int? french, int? italian, int? romansh)
    {
        _germanMinimalThreshold = german;
        _frenchMinimalThreshold = french;
        _italianMinimalThreshold = italian;
        _romanshMinimalThreshold = romansh;
        return this;
    }

    public CommitteeTypeBuilder WithLanguagesPercentageThreshold(double? german, double? french, double? italian, double? romansh)
    {
        _germanThresholdPercentage = german;
        _frenchThresholdPercentage = french;
        _italianThresholdPercentage = italian;
        _romanshThresholdPercentage = romansh;
        return this;
    }

    public CommitteeType Build()
    {
        return new CommitteeType
        {
            Id = _id,
            OgdId = _ogdId,
            Created = _created,
            CreatedBy = _createdBy,
            Modified = _modified,
            ModifiedBy = _modifiedBy,
            Uri = _uri,
            IsDeleted = _isDeleted,
            TextDe = _textDe,
            TextFr = _textFr,
            TextIt = _textIt,
            TextRm = _textRm,
            DescriptionDe = _descriptionDe,
            DescriptionFr = _descriptionFr,
            DescriptionIt = _descriptionIt,
            DescriptionRm = _descriptionRm,
            Sort = _sort,
            OldId = _oldId,
            FemaleThreshold = _femaleThreshold,
            MaleThreshold = _maleThreshold,
            GermanMinimalThreshold = _germanMinimalThreshold,
            FrenchMinimalThreshold = _frenchMinimalThreshold,
            ItalianMinimalThreshold = _italianMinimalThreshold,
            RomanshMinimalThreshold = _romanshMinimalThreshold,
            GermanThresholdPercentage = _germanThresholdPercentage,
            FrenchThresholdPercentage = _frenchThresholdPercentage,
            ItalianThresholdPercentage = _italianThresholdPercentage,
            RomanshThresholdPercentage = _romanshThresholdPercentage
        };
    }
}
