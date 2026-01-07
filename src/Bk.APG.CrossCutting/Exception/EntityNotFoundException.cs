namespace Bk.APG.CrossCutting.Exception;

[Serializable]
public class EntityNotFoundException : System.Exception
{
    public EntityNotFoundException(string message) : base(message)
    {
    }

    public EntityNotFoundException(string message, System.Exception innerException) : base(message, innerException)
    {
    }

    public EntityNotFoundException()
    {
    }
}
