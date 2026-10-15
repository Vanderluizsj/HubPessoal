using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Vanderluiz.Hub.Application.Common;

public static partial class SlugHelper
{
    /// <summary>Gera um slug para URL. Ex.: "Gestão de Equipamentos" → "gestao-de-equipamentos".</summary>
    public static string Gerar(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return string.Empty;

        var normalizado = texto.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalizado.Length);
        foreach (var c in normalizado)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        var semAcento = sb.ToString().Normalize(NormalizationForm.FormC);
        var slug = CaracteresInvalidos().Replace(semAcento, "-");
        return slug.Trim('-');
    }

    [GeneratedRegex("[^a-z0-9]+")]
    private static partial Regex CaracteresInvalidos();
}

