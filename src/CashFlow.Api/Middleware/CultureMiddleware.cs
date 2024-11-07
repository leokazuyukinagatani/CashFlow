using System.Globalization;

namespace CashFlow.Api.Middleware;

public class CultureMiddleware
{
    private readonly RequestDelegate _next;
    public CultureMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task Invoke(HttpContext context) 
    {

        var culture = GetCultureFromHeaders(context.Request.Headers);

        var cultureInfo = CreateCultureInfo(culture);

        SetCurrentCulture(cultureInfo);

        await _next(context); ;
    }

    private static string GetCultureFromHeaders(IHeaderDictionary headers)
    {
        // Retorna o primeiro valor ou uma string vazia se não houver valor
        return headers.AcceptLanguage.FirstOrDefault() ?? string.Empty;
    }

    private static CultureInfo CreateCultureInfo(string culture)
    {
        if (IsInvalidCulture(culture) || IsLanguageUnsupported(culture))
        {
            return new CultureInfo("en");
        }

        return new CultureInfo(culture);
    }

    private static bool IsInvalidCulture(string culture)
    {
        return string.IsNullOrWhiteSpace(culture);
    }

    private static bool IsLanguageUnsupported(string requestedCulture)
    {
        var supportedLanguages = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList();
        return !supportedLanguages.Exists(language => language.Name.Equals(requestedCulture));
    }

    private static void SetCurrentCulture(CultureInfo cultureInfo)
    {
        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;
    }

}
