using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Util
{
    public static class FilterHelper
    {
        private static readonly Dictionary<string, string> NomesCustomizados = new(StringComparer.OrdinalIgnoreCase)
        {
            { "cpf", "CPF" },
            { "cartao sus", "cartão do SUS" }
        };

        public static string ExtrairNomeChave(string sqlMessage)
        {
            var match = Regex.Match(sqlMessage ?? "", @"(?:for key|CONSTRAINT)\s+['`]?([^'`]+)['`]?", RegexOptions.IgnoreCase);
            if (!match.Success)
                return "registro";

            var nomeChave = match.Groups[1].Value.Split('.').Last();
            var chave = Regex.Replace(nomeChave, @"(?i)(_unique|unique|fk_|uk_)", "")
                             .Replace("_", " ")
                             .Trim();

            if (string.IsNullOrEmpty(chave))
                return "registro";

            if (NomesCustomizados.TryGetValue(chave, out var nomePersonalizado))
                return nomePersonalizado;

            return char.ToUpper(chave[0]) + chave[1..];
        }
    }
}