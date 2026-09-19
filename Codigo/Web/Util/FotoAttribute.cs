using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Util
{
    public class FotoAttribute : ValidationAttribute
    {
        public int LarguraMaxima { get; set; }
        public int AlturaMaxima { get; set; }
        public long TamanhoMaximoBytes { get; set; } = 65535; // Limite padrão do tipo BLOB do MySQL (64 KB)

        public override bool IsValid(object? value)
        {
            if (value == null) return true;

            byte[]? bytesArquivo = null;

            long limiteKb = (long)Math.Ceiling((double)TamanhoMaximoBytes / 1024);

            if (value is IFormFile file)
            {
                if (file.Length == 0) return true;

                if (TamanhoMaximoBytes > 0 && file.Length > TamanhoMaximoBytes)
                {
                    ErrorMessage = $"A foto ultrapassa o limite permitido de {limiteKb}KB.";
                    return false;
                }

                using var ms = new MemoryStream();
                file.CopyTo(ms);
                bytesArquivo = ms.ToArray();
            }
            else if (value is byte[] bytes)
            {
                if (bytes.Length == 0) return true;

                if (TamanhoMaximoBytes > 0 && !Methods.ValidarTamanhoFoto(bytes, TamanhoMaximoBytes))
                {
                    ErrorMessage = $"A foto ultrapassa o limite permitido de {limiteKb}KB.";
                    return false;
                }

                bytesArquivo = bytes;
            }
            else
            {
                return true;
            }

            if (bytesArquivo != null && bytesArquivo.Length > 0)
            {
                if (!Methods.ValidarExtensaoFoto(bytesArquivo))
                {
                    ErrorMessage = "Formato de imagem inválido. Use apenas imagens JPG, JPEG ou PNG.";
                    return false;
                }

                if (LarguraMaxima > 0 && AlturaMaxima > 0)
                {
                    if (!Methods.ValidarDimensoesFoto(bytesArquivo, LarguraMaxima, AlturaMaxima))
                    {
                        ErrorMessage = $"As dimensões da imagem devem ser de no máximo {LarguraMaxima}x{AlturaMaxima} pixels.";
                        return false;
                    }
                }
            }

            return true;
        }
    }
}