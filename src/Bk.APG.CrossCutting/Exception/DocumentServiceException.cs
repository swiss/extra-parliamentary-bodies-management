namespace Bk.APG.CrossCutting.Exception;

[Serializable]
public class DocumentServiceException : System.Exception
{
    public DocumentServiceException()
    {
    }

    public DocumentServiceException(string message) : base(message)
    {
    }

    public DocumentServiceException(string message, System.Exception innerException) : base(message, innerException)
    {
    }
}
