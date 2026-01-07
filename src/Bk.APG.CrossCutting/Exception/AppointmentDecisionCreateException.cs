namespace Bk.APG.CrossCutting.Exception;

public class AppointmentDecisionCreateException : System.Exception
{
    public AppointmentDecisionCreateException(string message) : base(message)
    {
    }

    public AppointmentDecisionCreateException(string message, System.Exception innerException) : base(message, innerException)
    {
    }

    public AppointmentDecisionCreateException()
    {
    }
}
