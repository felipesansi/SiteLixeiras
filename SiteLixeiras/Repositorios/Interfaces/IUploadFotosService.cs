using SiteLixeiras.Models;

namespace SiteLixeiras.Repositorios.Interfaces
{
    public interface IUploadFotosService
    {

        Task<string> UploadFileAsync(IFormFile file, string caminho_destino);


        Task SalvarTokensAsync(string accessToken, string refreshToken, string tokenType, DateTime dataExpiracao);


        Task<DadosDropBox> RecuperarTokenActivoAsync();

        Task<bool> DeleteFileAsync(string caminhoArquivo);

    }
}
