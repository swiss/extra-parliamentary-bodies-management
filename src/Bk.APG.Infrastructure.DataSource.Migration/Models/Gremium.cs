namespace Bk.APG.Infrastructure.DataSource.Migration.Models;

public class Gremium
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public Guid GuidTranslation { get; set; }
    public int GraId { get; set; }
    public int DepId { get; set; }
    public int AmpId { get; set; }
    public int? StatusId { get; set; }
    public int? StuId { get; set; }
    public int? ZustVerwStelleId { get; set; }
    public int? AnzahlVakanzenGEW { get; set; }
    public string? BegrAnzMitglieder { get; set; }
    public string? BegrSprachen { get; set; }
    public string? MassnaGremSprachen { get; set; }
    public int? MassnaAllgSprachenId { get; set; }
    public string? BegrGeschlechter { get; set; }
    public string? MassnaGremGeschlechter { get; set; }
    public int? MassnaAllgGeschlechterId { get; set; }
    public string? Rechtsform { get; set; }
    public DateTime? BeginnDatum { get; set; }
    public DateTime? EndDatum { get; set; }
    public bool? KomVer { get; set; }
    public string? GesetzlicheGrundlagen { get; set; }
    public bool? Bundesgesetz { get; set; }
    public int? MaxAnzMitglieder { get; set; }
    public int? MinAnzMitglieder { get; set; }
    public string? BemerkungGrunddaten { get; set; }
    public string? BemerkungBegründungen { get; set; }
    public string? BemerkungEnsetzvBeschl { get; set; }
    public string? BemerkungZusatzInfo { get; set; }
    public string? BemerkungSekretariate { get; set; }
    public string? BemerkungMitglieder { get; set; }
    public string? BemerkungGrunddatenAdmin { get; set; }
    public string? BemerkungBegründungenAdmin { get; set; }
    public string? BemerkungEnsetzvBeschlAdmin { get; set; }
    public string? BemerkungZusatzInfoAdmin { get; set; }
    public string? BemerkungSekretariateAdmin { get; set; }
    public string? BemerkungMitgliederAdmin { get; set; }
    public bool? ZusätzKantonGewMitglieder { get; set; }
    public string? ZusätzKantonGewMitgliederUrl { get; set; }
    public string? HomepageLinkDe { get; set; }
    public string? HomepageLinkFr { get; set; }
    public string? HomepageLinkIt { get; set; }
    public string? HomepageLinkRm { get; set; }
    public bool? FreigabeSammelantrag { get; set; }
    public bool? Marktorientiert { get; set; }
    public bool? PublishIB { get; set; }
    public bool? GEW { get; set; }
    public DateTime InsertDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public string? LastupdateUser { get; set; }
    public DateTime? Histo { get; set; }
    public bool? Aufsichtsaufgabe { get; set; }

    // additional fields for migration
    public Guid StufeGuid { get; set; }
    public Guid GremiumartGuid { get; set; }
    public Guid DepartementGuid { get; set; }
    public Guid AmtsperiodeGuid { get; set; }
    public Guid AmtGuid { get; set; }
    public string? BezeichnungD { get; set; }
    public string? BezeichnungF { get; set; }
    public string? BezeichnungI { get; set; }
    public string? BezeichnungR { get; set; }
    public Guid? MassnaAllgGeschlechterGuid { get; set; }
    public Guid? MassnaAllgSprachenGuid { get; set; }
}
