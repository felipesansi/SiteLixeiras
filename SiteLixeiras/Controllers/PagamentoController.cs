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

            
            var itens_preferencias = carrinho.Select(i => new PreferenceItemRequest
            {
                Id = i.Produtos.Id_Produto.ToString(),
                Title = i.Produtos.Nome,
                CurrencyId = "BRL",
                Quantity = i.Quantidade,
                UnitPrice = Math.Round(i.Produtos.Preco, 2)
            }).ToList();

           
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
                NotificationUrl = Url.Action("Notificacao", "Pagamento", null, Request.Scheme),
            };

            
            var client = new PreferenceClient();
            var preferencia_resposta = await client.CreateAsync(preferenceRequest);

            return Redirect(preferencia_resposta.InitPoint);
        }
        public IActionResult PagamentoSucesso(int enderecoId)
        {
            TempData["Sucesso"] = "Pagamento realizado com sucesso!";
            _carrinhoCompra.LimparCarrinho();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Falha()
        {
            TempData["Erro"] = "Ocorreu um erro ao processar o pagamento.";
            return RedirectToAction("Falha", "Pagamento");
        }
        public IActionResult Aguardando()
        {
            TempData["Aguardando"] = "Seu pagamento está aguardando confirmação.";
            return RedirectToAction("Index", "CarrinhoCompra");
        }
    }
}
