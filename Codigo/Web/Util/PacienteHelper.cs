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

        public static string? FormatarApenasNumeros(this string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return texto;

            return new string(texto.Where(char.IsDigit).ToArray());
        }
    }
}