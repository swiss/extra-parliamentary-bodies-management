using System.Globalization;

namespace Bk.APG.Business.Services;

public interface ICultureService
{
    CultureInfo GetCurrentUiCulture();
}
