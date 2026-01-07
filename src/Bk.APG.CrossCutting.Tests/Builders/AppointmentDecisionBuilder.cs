using Bk.APG.Business.Models;
using Bogus;

namespace Bk.APG.CrossCutting.Tests.Builders;

public class AppointmentDecisionBuilder
{
    private readonly Faker _faker = new();
    private Guid _id;
    private readonly DateTime _created;
    private readonly string _createdBy;
    private readonly DateTime _modified;
    private readonly string _modifiedBy;
    private readonly DateOnly _appointmentDecisionDate;
    private readonly string? _text;
    private readonly string? _link;
    private DocumentStorage? _fileReferenceGerman;
    private Guid? _fileReferenceGermanId;
    private DocumentStorage? _fileReferenceFrench;
    private Guid? _fileReferenceFrenchId;
    private DocumentStorage? _fileReferenceItalian;
    private Guid? _fileReferenceItalianId;
    private DocumentStorage? _fileReferenceRomansh;
    private Guid? _fileReferenceRomanshId;
    private DocumentStorage? _originalDocument;
    private Guid? _originalDocumentId;
    private Guid _committeeId;
    private Committee? _committee;
    private Guid? _appointmentDecisionTypeId;
    private AppointmentDecisionType? _appointmentDecisionType;
    private Guid? _appointmentDecisionLinkTypeId;
    private AppointmentDecisionLinkType? _appointmentDecisionLinkType;
    private readonly int _oldId;

    public AppointmentDecisionBuilder()
    {
        _id = _faker.Random.Guid();
        _created = _faker.Date.Past();
        _createdBy = _faker.Person.Email;
        _modified = _faker.Date.Past();
        _modifiedBy = _faker.Person.Email;
        _appointmentDecisionDate = DateOnly.FromDateTime(_faker.Date.Past());
        _text = _faker.Company.CompanyName().OrNull(_faker);
        _link = _faker.Address.StreetName().OrNull(_faker);
        _fileReferenceGermanId = _faker.Random.Guid();
        _fileReferenceGerman = new DocumentStorageBuilder().WithId((Guid)_fileReferenceGermanId).Build();
        _fileReferenceFrenchId = _faker.Random.Guid().OrNull(_faker);
        _fileReferenceItalianId = _faker.Random.Guid().OrNull(_faker);
        _fileReferenceRomanshId = _faker.Random.Guid().OrNull(_faker);
        _committeeId = _faker.Random.Guid();
        _appointmentDecisionTypeId = _faker.Random.Guid().OrNull(_faker);
        _appointmentDecisionLinkTypeId = _faker.Random.Guid().OrNull(_faker);
        _oldId = _faker.Random.Int(1);
    }

    public AppointmentDecisionBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public AppointmentDecisionBuilder WithCommitteeId(Guid committeeId)
    {
        _committeeId = committeeId;
        return this;
    }

    public AppointmentDecisionBuilder WithCommittee(Committee committee)
    {
        _committeeId = committee.Id;
        _committee = committee;
        return this;
    }

    public AppointmentDecisionBuilder WithAppointmentDecisionType(AppointmentDecisionType? appointmentDecisionType)
    {
        _appointmentDecisionTypeId = appointmentDecisionType?.Id;
        _appointmentDecisionType = appointmentDecisionType;
        return this;
    }

    public AppointmentDecisionBuilder WithAppointmentDecisionLinkType(AppointmentDecisionLinkType? appointmentDecisionLinkType)
    {
        _appointmentDecisionLinkTypeId = appointmentDecisionLinkType?.Id;
        _appointmentDecisionLinkType = appointmentDecisionLinkType;
        return this;
    }

    public AppointmentDecisionBuilder WithFileReferenceGerman(DocumentStorage? document)
    {
        _fileReferenceGerman = document;
        _fileReferenceGermanId = document?.Id;
        return this;
    }

    public AppointmentDecisionBuilder WithFileReferenceFrench(DocumentStorage? document)
    {
        _fileReferenceFrench = document;
        _fileReferenceFrenchId = document?.Id;
        return this;
    }

    public AppointmentDecisionBuilder WithFileReferenceItalian(DocumentStorage? document)
    {
        _fileReferenceItalian = document;
        _fileReferenceItalianId = document?.Id;
        return this;
    }

    public AppointmentDecisionBuilder WithFileReferenceRomansh(DocumentStorage? document)
    {
        _fileReferenceRomansh = document;
        _fileReferenceRomanshId = document?.Id;
        return this;
    }

    public AppointmentDecisionBuilder WithOriginalDocument(DocumentStorage document)
    {
        _originalDocument = document;
        _originalDocumentId = document.Id;
        return this;
    }

    public AppointmentDecision Build()
    {
        return new AppointmentDecision
        {
            Id = _id,
            Created = _created,
            CreatedBy = _createdBy,
            Modified = _modified,
            ModifiedBy = _modifiedBy,
            Committee = _committee!,
            CommitteeId = _committeeId,
            AppointmentDecisionTypeId = _appointmentDecisionTypeId,
            AppointmentDecisionType = _appointmentDecisionType,
            AppointmentDecisionLinkTypeId = _appointmentDecisionLinkTypeId,
            AppointmentDecisionLinkType = _appointmentDecisionLinkType,
            AppointmentDecisionDate = _appointmentDecisionDate,
            Text = _text,
            Link = _link,
            FileReferenceGermanId = _fileReferenceGermanId,
            FileReferenceGerman = _fileReferenceGerman,
            FileReferenceFrenchId = _fileReferenceFrenchId,
            FileReferenceFrench = _fileReferenceFrench,
            FileReferenceItalianId = _fileReferenceItalianId,
            FileReferenceItalian = _fileReferenceItalian,
            FileReferenceRomanshId = _fileReferenceRomanshId,
            FileReferenceRomansh = _fileReferenceRomansh,
            OriginalDocumentId = _originalDocumentId,
            OriginalDocument = _originalDocument,
            OldId = _oldId
        };
    }
}
