using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MercadoPago.Config;
using MercadoPago.Client.Preference;
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
                Title = i.Produtos.Nome,
                Quantity = i.Quantidade,
                UnitPrice = Math.Round(i.Produtos.Preco, 2),
                CurrencyId = "BRL",
                Description = i.Produtos.Descricao,
                PictureUrl = i.Produtos.ImagemThumbUrl
            }).ToList();

            var metodos_pagamento= new PreferencePaymentMethodsRequest
            {
                ExcludedPaymentTypes = new List<PreferencePaymentTypeRequest>(),
                Installments = 12 // até 12 parcelas
            };

          
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
                PaymentMethods = metodos_pagamento
            };

            var client = new PreferenceClient();
            var preference = await client.CreateAsync(preferenceRequest);

            return Redirect(preference.InitPoint);
        }

        public IActionResult PagamentoSucesso(int enderecoId)
        {
            TempData["Sucesso"] = "Pagamento aprovado com sucesso!";
            _carrinhoCompra.LimparCarrinho();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Falha()
        {
            TempData["Erro"] = "O pagamento falhou. Tente novamente.";
            return RedirectToAction("Index", "CarrinhoCompra");
        }

        public IActionResult Aguardando()
        {
            TempData["Aguardando"] = "O pagamento está aguardando confirmação.";
            return RedirectToAction("Index", "Home");
        }
    }
}
