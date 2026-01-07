namespace Bk.APG.CrossCutting.Exception;

public class AppointmentDecisionUpdateException : System.Exception
{
    public AppointmentDecisionUpdateException(string message) : base(message)
    {
    }

    public AppointmentDecisionUpdateException(string message, System.Exception innerException) : base(message, innerException)
    {
    }

    public AppointmentDecisionUpdateException()
    {
    }
}
