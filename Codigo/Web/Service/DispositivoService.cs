using System.Text.Json;
using Core.Exceptions;
using Core.Service;
using Microsoft.Extensions.Configuration;

namespace Service
{
    public class DispositivoService : IDispositivoService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;

        public DispositivoService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
        }

        public async Task<string?> ObterToken(uint idPaciente)
        {
            try
            {
                var client = httpClientFactory.CreateClient();
                var baseUrl = configuration["MedicaApi:BaseUrl"];
                var response = await client.GetAsync($"{baseUrl}/api/auth/token-pareamento/{idPaciente}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(content);
                    if (doc.RootElement.TryGetProperty("data", out var dataProp))
                    {
                        return dataProp.GetString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new MedicaApiException("Não foi possível conectar à MedicaApi.", ex);
            }

            return null;
        }
    }
}
