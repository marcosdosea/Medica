using Microsoft.AspNetCore.Http;
using System.IO;

namespace Util
{
    public static class FotoHelper
    {
        /// <summary>
        /// Converte um IFormFile em um vetor de bytes (byte[])
        /// </summary>
        /// <param name="file">Arquivo de imagem enviado pelo formulário</param>
        /// <returns>Array de bytes da imagem ou null caso não exista</returns>
        public static byte[]? ConverterFoto(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return null;

            using var ms = new MemoryStream();
            file.CopyTo(ms);
            return ms.ToArray();
        }
    }
}