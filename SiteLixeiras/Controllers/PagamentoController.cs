using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MercadoPago.Config;
using MercadoPago.Client.Preference;
using MercadoPago.Client.Payment;
using SiteLixeiras.Models;
using SiteLixeiras.Context;
using Microsoft.Extensions.Logging;
using MercadoPago.Resource.Payment;
using Microsoft.EntityFrameworkCore;

namespace SiteLixeiras.Controllers
{
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
                CurrencyId = "BRL"
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
                Metadata = new Dictionary<string, object>
                {
                    { "user_id", User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "" },
                    { "endereco_id", enderecoId }
                }
            };

            var client = new PreferenceClient();
            var preference = await client.CreateAsync(preferenceRequest);

            return Redirect(preference.InitPoint);
        }

        [HttpPost]
        [Route("Pagamento/Notificacao")]
        public async Task<IActionResult> Notificacao()
        {
            _logger.LogInformation("Webhook chamado em {Hora}", DateTime.Now);

            string body;
            using (var reader = new StreamReader(Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }

            _logger.LogInformation("Corpo recebido: {Body}", body);

            var query = HttpContext.Request.Query;
            string paymentId = query["id"];
            string topic = query["topic"];

            if (string.IsNullOrEmpty(paymentId) || topic != "payment")
            {
                _logger.LogWarning("Notificação com dados inválidos: id={PaymentId}, topic={Topic}", paymentId, topic);
                return BadRequest("Dados inválidos na notificação.");
            }

            try
            {
                MercadoPagoConfig.AccessToken = _mercadoPagoSettings.AccessToken;
                var client = new PaymentClient();
                var payment = await client.GetAsync(long.Parse(paymentId));

                _logger.LogInformation("Pagamento recebido: ID={Id}, Status={Status}", payment.Id, payment.Status);

                await ProcessarPagamento(payment);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar notificação do Mercado Pago.");
                return StatusCode(500, "Erro interno");
            }
        }

        private async Task ProcessarPagamento(Payment payment)
        {
            if (payment.Status != "approved")
            {
                _logger.LogInformation("Pagamento {Id} não aprovado. Status: {Status}", payment.Id, payment.Status);
                return;
            }

            var userId = payment.Metadata.TryGetValue("user_id", out var userIdObj) ? userIdObj.ToString() : null;
            var enderecoIdStr = payment.Metadata.TryGetValue("endereco_id", out var enderecoIdObj) ? enderecoIdObj.ToString() : null;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(enderecoIdStr) || !int.TryParse(enderecoIdStr, out int enderecoId))
            {
                _logger.LogWarning("Metadata incompleta no pagamento {Id}.", payment.Id);
                return;
            }

            var pedidoExistente = await _context.Pedidos.FirstOrDefaultAsync(p => p.MercadoPagoPaymentId == payment.Id.ToString());
            if (pedidoExistente != null)
            {
                _logger.LogInformation("Pedido já registrado para pagamento {Id}.", payment.Id);
                return;
            }

            var usuario = await _context.Users.FindAsync(userId);
            var endereco = await _context.EnderecosEntregas.FindAsync(enderecoId);

            if (usuario == null || endereco == null)
            {
                _logger.LogWarning("Usuário ou endereço não encontrado para pagamento {Id}.", payment.Id);
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
                DataPagamento = DateTime.Now
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Pedido criado com sucesso para pagamento {Id}.", payment.Id);
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
