using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MercadoPago.Config;
using MercadoPago.Client.Preference;
using MercadoPago.Client.Payment;
using MercadoPago.Resource.Payment;
using SiteLixeiras.Models;
using SiteLixeiras.Context;
using Microsoft.AspNetCore.Authorization;

namespace SiteLixeiras.Controllers
{
    [Route("Pagamento")]
    public class PagamentoController : Controller
    {
        private readonly CarrinhoCompra _carrinhoCompra;
        private readonly MercadoPagoSettings _mercadoPagoSettings;
        private readonly AppDbContext _context;
        private readonly ILogger<PagamentoController> _logger;

        public PagamentoController(
            CarrinhoCompra carrinhoCompra,
            IOptions<MercadoPagoSettings> mercadoPagoSettings,
            AppDbContext context,
            ILogger<PagamentoController> logger)
        {
            _carrinhoCompra = carrinhoCompra;
            _mercadoPagoSettings = mercadoPagoSettings.Value;
            _context = context;
            _logger = logger;
        }

        [HttpGet("CriarPagamentoCartao")]
        public async Task<IActionResult> CriarPagamentoCartao(int enderecoId)
        {
            return await CriarPagamentoComOpcao(enderecoId, "cartao");
        }

        [HttpGet("CriarPagamentoBoleto")]
        public async Task<IActionResult> CriarPagamentoBoleto(int enderecoId)
        {
            return await CriarPagamentoComOpcao(enderecoId, "boleto");
        }

        [HttpGet("CriarPagamentoPix")]
        public async Task<IActionResult> CriarPagamentoPix(int enderecoId)
        {
            return await CriarPagamentoComOpcao(enderecoId, "pix");
        }

        private async Task<IActionResult> CriarPagamentoComOpcao(int enderecoId, string tipoPagamento)
        {
            MercadoPagoConfig.AccessToken = _mercadoPagoSettings.AccessToken;

            var carrinho = _carrinhoCompra.GetCarrinhoCompraItems();
            if (carrinho == null || !carrinho.Any())
            {
                TempData["Erro"] = "Seu carrinho está vazio.";
                return RedirectToAction("Index", "CarrinhoCompra");
            }

            var itens = carrinho.Select(i =>
            {
                decimal precoBase = i.Produtos.Preco;
                decimal precoComAcrescimo = precoBase;

                if (tipoPagamento == "cartao" || tipoPagamento == "boleto")
                {
                    precoComAcrescimo = Math.Round(precoBase * 1.05m, 2); // 5% acréscimo
                }

                return new PreferenceItemRequest
                {
                    Title = i.Produtos.Nome,
                    Quantity = i.Quantidade,
                    UnitPrice = precoComAcrescimo,
                    CurrencyId = "BRL"
                };
            }).ToList();

            var metodos = new PreferencePaymentMethodsRequest();

            if (tipoPagamento == "cartao")
            {
                metodos.ExcludedPaymentTypes = new List<PreferencePaymentTypeRequest>
                {
                    new() { Id = "ticket" },
                    new() { Id = "pix" }
                };
                metodos.Installments = 10; // até 10x no cartão
            }
            else if (tipoPagamento == "boleto")
            {
                metodos.ExcludedPaymentTypes = new List<PreferencePaymentTypeRequest>
                {
                    new() { Id = "credit_card" },
                    new() { Id = "pix" }
                };
                
            }
            else if (tipoPagamento == "pix")
            {
                metodos.ExcludedPaymentTypes = new List<PreferencePaymentTypeRequest>
                {
                    new() { Id = "credit_card" },
                    new() { Id = "ticket" }
                };
            }

            var preferenceRequest = new PreferenceRequest
            {
                Items = itens,
                PaymentMethods = metodos,
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = Url.Action("PagamentoSucesso", "Pagamento", new { enderecoId }, Request.Scheme),
                    Failure = Url.Action("Falha", "Pagamento", null, Request.Scheme),
                    Pending = Url.Action("Aguardando", "Pagamento", null, Request.Scheme)
                },
                AutoReturn = "approved",
                Metadata = new Dictionary<string, object>
                {
                    { "user_id", User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "" },
                    { "endereco_id", enderecoId }
                },
                NotificationUrl = $"{Request.Scheme}://{Request.Host}/Pagamento/Notificacao"
            };

            var client = new PreferenceClient();
            var preference = await client.CreateAsync(preferenceRequest);

            return Redirect(preference.InitPoint);
        }

        [HttpPost("Notificacao")]
        [AllowAnonymous]
        public async Task<IActionResult> Notificacao()
        {
            _logger.LogInformation("Webhook recebido em {Hora}", DateTime.Now);

            var query = HttpContext.Request.Query;
            string paymentId = query["id"];
            string topic = query["topic"];

            if (string.IsNullOrEmpty(paymentId) || topic != "payment")
            {
                _logger.LogWarning("Notificação inválida: id={PaymentId}, topic={Topic}", paymentId, topic);
                return BadRequest("Notificação inválida");
            }

            try
            {
                MercadoPagoConfig.AccessToken = _mercadoPagoSettings.AccessToken;
                var client = new PaymentClient();
                var payment = await client.GetAsync(long.Parse(paymentId));

                _logger.LogInformation("Status do pagamento {Id}: {Status}", payment.Id, payment.Status);
                _logger.LogInformation("Método de pagamento: {Type} - {Id}", payment.PaymentTypeId, payment.PaymentMethodId);

                await ProcessarPagamento(payment);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar notificação");
                return StatusCode(500);
            }
        }

        private async Task ProcessarPagamento(Payment payment)
        {
            if (payment.Status != "approved")
            {
                _logger.LogInformation("Pagamento {Id} não aprovado", payment.Id);
                return;
            }

            var userId = payment.Metadata.TryGetValue("user_id", out var userObj) ? userObj?.ToString() : null;
            var enderecoIdStr = payment.Metadata.TryGetValue("endereco_id", out var endObj) ? endObj?.ToString() : null;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(enderecoIdStr) || !int.TryParse(enderecoIdStr, out int enderecoId))
            {
                _logger.LogWarning("Metadata incompleta no pagamento {Id}", payment.Id);
                return;
            }

            if (await _context.Pedidos.AnyAsync(p => p.MercadoPagoPaymentId == payment.Id.ToString()))
            {
                _logger.LogInformation("Pedido já registrado para pagamento {Id}", payment.Id);
                return;
            }

            var usuario = await _context.Users.FindAsync(userId);
            var endereco = await _context.EnderecosEntregas.FindAsync(enderecoId);

            if (usuario == null || endereco == null)
            {
                _logger.LogWarning("Usuário ou endereço não encontrado para pagamento {Id}", payment.Id);
                return;
            }

            var pedido = new Pedido
            {
                UsuarioId = userId,
                PedidoTotal = payment.TransactionAmount ?? 0,
                TotalItensPedidos = 0,
                PedidoEnviado = DateTime.Now,
                EnderecoEntregaId = endereco.EnderecoEntregaId,
                MercadoPagoPaymentId = payment.Id.ToString(),
                Pago = true,
                DataPagamento = DateTime.Now,
                metodoPagamento = payment.PaymentTypeId 
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Pedido registrado com sucesso para pagamento {Id}", payment.Id);
            _carrinhoCompra.LimparCarrinho();
            TempData["sucesso"] = "Seu pagamento está pago";
        }

        [HttpGet("PagamentoSucesso")]
        public IActionResult PagamentoSucesso(int enderecoId)
        {
            TempData["Sucesso"] = "Pagamento aprovado com sucesso!";
            _carrinhoCompra.LimparCarrinho();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("Falha")]
        public IActionResult Falha()
        {
            TempData["Erro"] = "O pagamento falhou. Tente novamente.";
            return RedirectToAction("Index", "CarrinhoCompra");
        }

        [HttpGet("Aguardando")]
        public IActionResult Aguardando()
        {
            TempData["Aguardando"] = "O pagamento está aguardando confirmação.";
            return RedirectToAction("Index", "Home");
        }
    }
}
