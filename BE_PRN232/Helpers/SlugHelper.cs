using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
namespace BE_PRN232.Helpers;

public class SlugHelper
{
    public static string GenerateSlug(string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
            return string.Empty;
        string normalized = phrase.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalized)
        {
            var uc = CharUnicodeInfo.GetUnicodeCategory(c);
            if (uc != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        string str = sb.ToString().Normalize(NormalizationForm.FormC);
        str = str.ToLowerInvariant();
        str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
        str = Regex.Replace(str, @"\s+", "-").Trim('-');
        str = Regex.Replace(str, @"-+", "-");
        return str;
    }
}