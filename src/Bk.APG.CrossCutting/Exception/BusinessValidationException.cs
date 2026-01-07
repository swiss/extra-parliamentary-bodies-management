namespace Bk.APG.CrossCutting.Exception;

[Serializable]
public class BusinessValidationException : System.Exception
{
    public BusinessValidationException(string message) : base(message)
    {
    }

    public BusinessValidationException(string message, System.Exception innerException) : base(message, innerException)
    {
    }

    public BusinessValidationException()
    {
    }
}
