using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MercadoPago;
using MercadoPago.Config;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using SiteLixeiras.Models;

namespace SiteLixeiras.Controllers
{
    public class PagamentoController : Controller
    {
        private readonly CarrinhoCompra _carrinhoCompra;
        private readonly MercadoPagoSettings _mercadoPagoSettings;

        public PagamentoController(CarrinhoCompra carrinhoCompra,
                                   IOptions<MercadoPagoSettings> mercadoPagoSettings)
        {
            _carrinhoCompra = carrinhoCompra;
            _mercadoPagoSettings = mercadoPagoSettings.Value;
        }

        public async Task<IActionResult> CriarPagamento(int enderecoId)
        {
            
            MercadoPagoConfig.AccessToken = _mercadoPagoSettings.AccessToken;

            
            var carrinho = _carrinhoCompra.GetCarrinhoCompraItems();

            
            if (carrinho == null || !carrinho.Any())
            {
                TempData["Erro"] = "Seu carrinho está vazio.";
                return RedirectToAction("Index", "CarrinhoCompra");
            }

            // Cria lista de itens para a preferência
            var itens_preferencias = carrinho.Select(i => new PreferenceItemRequest
            {
                Id = i.Produtos.Id_Produto.ToString(),
                Title = i.Produtos.Nome,
                CurrencyId = "BRL",
                Quantity = i.Quantidade,
                UnitPrice = Math.Round(i.Produtos.Preco, 2) // Garante valor válido
            }).ToList();

            // Criação da preferência
            var preferenceRequest = new PreferenceRequest
            {
                Items = itens_preferencias,
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = Url.Action("PagamentoSucesso", "Pagamento", new { enderecoId }, Request.Scheme),
                    Failure = Url.Action("Falha", "Pagamento", null, Request.Scheme),
                    Pending = Url.Action("Aguardando", "Pagamento", null, Request.Scheme)
                },
                AutoReturn = "approved",
            };

            // Cria a preferência e redireciona para o checkout do Mercado Pago
            var client = new PreferenceClient();
            var preferencia_resposta = await client.CreateAsync(preferenceRequest);

            return Redirect(preferencia_resposta.InitPoint);
        }
    }
}
