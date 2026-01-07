using System.Globalization;

namespace Bk.APG.Business.Services;

public class CultureService : ICultureService
{
    public CultureInfo GetCurrentUiCulture()
    {
        return CultureInfo.CurrentUICulture;
    }
}
