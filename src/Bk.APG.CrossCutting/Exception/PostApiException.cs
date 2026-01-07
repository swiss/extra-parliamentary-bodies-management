namespace Bk.APG.CrossCutting.Exception;

public class PostApiException : System.Exception
{
    public PostApiException(string message) : base(message)
    {
    }

    public PostApiException(string message, System.Exception innerException) : base(message, innerException)
    {
    }

    public PostApiException()
    {
    }
}
