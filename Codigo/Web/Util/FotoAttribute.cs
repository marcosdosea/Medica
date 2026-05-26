using System.ComponentModel.DataAnnotations;

namespace Util
{
    public class FotoAttribute : ValidationAttribute
    {
        public int LarguraMaxima { get; set; }
        public int AlturaMaxima { get; set; }
        public long TamanhoMaximoBytes { get; set; }

        public override bool IsValid(object? value)
        {
            if (value == null) return true;

            if (value is byte[] bytesArquivo)
            {
                if (!Methods.ValidarTamanhoFoto(bytesArquivo, TamanhoMaximoBytes))
                {
                    ErrorMessage = $"A foto ultrapassa o limite permitido de {TamanhoMaximoBytes / 1024}KB.";
                    return false;
                }

                if (!Methods.ValidarExtensaoFoto(bytesArquivo))
                {
                    ErrorMessage = "Formato de imagem inválido. Use apenas imagens JPG, JPEG ou PNG.";
                    return false;
                }

                if (!Methods.ValidarDimensoesFoto(bytesArquivo, LarguraMaxima, AlturaMaxima))
                {
                    ErrorMessage = $"As dimensões da imagem devem ser de no máximo {LarguraMaxima}x{AlturaMaxima} pixels.";
                    return false;
                }
            }
            return true;
        }
    }
}