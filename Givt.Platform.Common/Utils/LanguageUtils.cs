using System.Net.Http.Headers;

namespace Givt.Platform.Common.Utils;

public class LanguageUtils
{
    /// <summary>
    /// Decodes HttpHeader AcceptLanguage, and combines these with otherLanguages. OtherLanguages take precedence
    /// </summary>
    /// <param name="acceptLanguages">languages from e.g. a HTTP Header values</param>
    /// <param name="language">languages used to override HTTP Header values</param>
    /// <returns></returns>
    public static IList<string> GetLanguages(string acceptLanguages, string language)
    {
        string header = acceptLanguages + String.Empty;
        if (!String.IsNullOrWhiteSpace(language))
            header = language + ";q=10, " + header;// q larger than 1.0

        // select language/region codes in (descending) order of quality, and clean up language/locale passed from client
        /* Routine to get a language from the API front, and normalise it to a IETF standard value
         * IETF BCP 47 language tag: en-GB (https://en.wikipedia.org/wiki/IETF_language_tag
         * Http Headers a.o.: en_GB https://en.wikipedia.org/wiki/Locale_(computer_software)
         */
        var result = header.Split(',')
            .Select(StringWithQualityHeaderValue.Parse)
            .Distinct()
            .OrderByDescending(s => s.Quality.GetValueOrDefault(1))
            .Select(vq => vq.Value.Trim().Replace('_', '-'))
            .ToList();

        return result;
    }
}