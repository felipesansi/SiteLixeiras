using Dropbox.Api;
using Dropbox.Api.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SiteLixeiras.Context;
using SiteLixeiras.Models;
using SiteLixeiras.Repositorios.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SiteLixeiras.Repositorios
{
   
        public class UploadFotosService : IUploadFotosService
        {
            private string? _accessToken;
            private readonly HttpClient _httpClient;
            private readonly AppDbContext _appDbContext;
            private readonly DropboxSettings _dropboxSettings;

            public UploadFotosService(AppDbContext appDbContext, IOptions<DropboxSettings> dropboxSettings)
            {
                _httpClient = new HttpClient();
                _appDbContext = appDbContext;
                _dropboxSettings = dropboxSettings.Value;
            }

            public async Task<string> UploadFileAsync(IFormFile file, string caminho_destino)
            {
                _accessToken = await ObeterTokenAcessoAsync();

                using (var dbx = new DropboxClient(_accessToken))
                {
                    using (var stream = file.OpenReadStream())
                    {
                        var Resultado_Upload = await dbx.Files.UploadAsync(
                            caminho_destino,
                            WriteMode.Overwrite.Instance,
                            body: stream);

                        var link_gerado = await dbx.Sharing.CreateSharedLinkWithSettingsAsync(Resultado_Upload.PathDisplay);
                        return link_gerado.Url;
                    }
                }
            }

            public async Task<bool> DeleteFileAsync(string caminhoArquivo)
            {
                _accessToken = await ObeterTokenAcessoAsync();

                using (var dbx = new DropboxClient(_accessToken))
                {
                    try
                    {
                        await dbx.Files.DeleteV2Async(caminhoArquivo);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao excluir arquivo: {ex.Message}");
                        return false;
                    }
                }
            }

        private async Task<string> ObeterTokenAcessoAsync()
        {
            var corpo_requisicao = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", _dropboxSettings.RefreshToken }
            };

            var requisicao = new HttpRequestMessage(HttpMethod.Post, "https://api.dropbox.com/oauth2/token")
            {
                Content = new FormUrlEncodedContent(corpo_requisicao)
            };
            var authHeaderValue = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{_dropboxSettings.AppKey}:{_dropboxSettings.AppSecret}"));
            requisicao.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

            var resposta = await _httpClient.SendAsync(requisicao);
            resposta.EnsureSuccessStatusCode();

            var respostaContent = await resposta.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<TokenResposta>(respostaContent);

            if (tokenResponse == null)
            {
                throw new InvalidOperationException("Falha ao desserializar a resposta do token.");
            }

            await SalvarTokensAsync(tokenResponse.AccessToken, _dropboxSettings.RefreshToken, tokenResponse.TokenType, DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn));

            return tokenResponse.AccessToken;
        }

        public async Task SalvarTokensAsync(string accessToken, string refreshToken, string tokenType, DateTime dataExpiracao)
            {
                var tokens = await _appDbContext.DadosDropBox.FirstOrDefaultAsync();

                if (tokens == null)
                {
                    tokens = new DadosDropBox
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        TokenType = tokenType,
                        DataExpiracao = dataExpiracao
                    };
                    _appDbContext.DadosDropBox.Add(tokens);
                }
                else
                {
                    tokens.AccessToken = accessToken;
                    tokens.RefreshToken = refreshToken;
                    tokens.TokenType = tokenType;
                    tokens.DataExpiracao = dataExpiracao;
                    _appDbContext.DadosDropBox.Update(tokens);
                }

                await _appDbContext.SaveChangesAsync();
            }

            public async Task<DadosDropBox> RecuperarTokenActivoAsync()
            {
                var token = await _appDbContext.DadosDropBox.FirstOrDefaultAsync();

                if (token == null)
                {
                    throw new InvalidOperationException("Falha ao recuperar o token.");
                }
                return token;
            }
        }
    }

    

