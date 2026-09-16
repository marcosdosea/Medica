using System;

namespace Util
{
    public static class PacienteHelper
    {
        public static string FormatarPrimeiroEUltimoNome(string? nomeCompleto)
        {
            if (string.IsNullOrWhiteSpace(nomeCompleto))
                return "-";

            var partes = nomeCompleto.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return partes.Length <= 1 ? partes[0] : $"{partes[0]} {partes[^1]}";
        }
    }
}