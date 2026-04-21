namespace Bk.APG.Business.Services;

public interface IOpenDataStackService
{
    Task<string> ExchangeToken(string accessToken);
}
