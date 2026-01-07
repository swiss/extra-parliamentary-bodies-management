namespace Bk.APG.Business.Models;

public class AppointmentDecisionType : MasterDataBase
{
    /// BR-Beschluss
    public static readonly Guid DecisionFederalCouncil = new("03043662-caa9-40ec-ab77-d8f2825eb775");

    /// Einsetzung
    public static readonly Guid Institution = new("7a68f837-ea2c-42b9-b9c7-6506fa3e9c67");
}
