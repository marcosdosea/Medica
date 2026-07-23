namespace Util
{
    public static class FormatterCpf
    {
        public static string FormatarCpf(string cpf)
        {
            return $"{cpf[..3]}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf[9..]}";
        }
    }
}
