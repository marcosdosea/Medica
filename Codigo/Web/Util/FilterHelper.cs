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
            { "cartao sus", "cartão do SUS" },
            { "cartaosus", "cartão do SUS" }
        };

        public static string ObterMensagemChaveDuplicada(string sqlMessage)
        {
            var match = Regex.Match(sqlMessage ?? "", @"(?:for key|CONSTRAINT)\s+['`]?([^'`]+)['`]?", RegexOptions.IgnoreCase);
            if (!match.Success)
                return "Já existe um registro com estes dados cadastrado.";

            var nomeChave = match.Groups[1].Value.Split('.').Last().ToLowerInvariant();

            if ((nomeChave.Contains("planejamento") && (nomeChave.Contains("horario") || nomeChave.Contains("hora")))
                || (nomeChave.Contains("horario") && nomeChave.Contains("paciente")))
            {
                return "Já existe um planejamento cadastrado para este paciente neste mesmo horário.";
            }

            if ((nomeChave.Contains("medicamento") && (nomeChave.Contains("nome") || nomeChave.Contains("cuidador")))
                || (nomeChave.Contains("nome") && nomeChave.Contains("cuidador")))
            {
                return "Já existe um medicamento cadastrado com este nome.";
            }

            if (nomeChave.Contains("cpf"))
            {
                return "Já existe um paciente cadastrado com este CPF.";
            }

            if (nomeChave.Contains("cartaosus") || nomeChave.Contains("cartao_sus") || nomeChave.Contains("sus"))
            {
                return "Já existe um paciente cadastrado com este Cartão do SUS.";
            }

            if (nomeChave.Contains("email"))
            {
                return "Já existe uma conta cadastrada com este e-mail.";
            }

            var campo = ExtrairNomeChave(sqlMessage ?? "");
            return $"Já existe um registro com este {campo} cadastrado.";
        }

        public static string ExtrairNomeChave(string sqlMessage)
        {
            var match = Regex.Match(sqlMessage ?? "", @"(?:for key|CONSTRAINT)\s+['`]?([^'`]+)['`]?", RegexOptions.IgnoreCase);
            if (!match.Success)
                return "registro";

            var nomeChave = match.Groups[1].Value.Split('.').Last();
            var chave = Regex.Replace(nomeChave, @"(?i)(^uq_|^uk_|^fk_|^idx_|_unique$|_idx$|_fk$|_uk$|_uq$)", "");
            chave = Regex.Replace(chave, @"(?i)(_unique|unique|fk_|uk_|uq_|idx_)", "")
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