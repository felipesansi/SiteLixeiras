using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteLixeiras.Context;
using SiteLixeiras.Models;
using SiteLixeiras.Repositorios.Interfaces;
using SiteLixeiras.Services;
using SiteLixeiras.Sevices;
using System.Security.Claims;
using SiteLixeiras.Helpers;

namespace SiteLixeiras.Controllers
{
    public class PedidosController : Controller
    {
        private readonly IProdutosRepositorio _produtosRepositorio;
        private readonly IPedido _pedido;
        private readonly AppDbContext _context;
        private readonly CarrinhoCompra carrinhoCompra;
        private readonly RazorViewToStringRenderer _razorViewToStringRenderer;
        private readonly EmailService _emailService;
        private CriptografiaHelper _criptografia;

        public PedidosController(IProdutosRepositorio produtosRepositorio, IPedido pedido, AppDbContext context, CarrinhoCompra carrinhoCompra, RazorViewToStringRenderer razorViewToStringRenderer, EmailService emailService, CriptografiaHelper criptografia)
        {
            _produtosRepositorio = produtosRepositorio;
            _pedido = pedido;
            _context = context;
            this.carrinhoCompra = carrinhoCompra;
            _razorViewToStringRenderer = razorViewToStringRenderer;
            _emailService = emailService;
            _criptografia = criptografia;
        }

        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var enderecos = await _context.EnderecosEntregas
                .Where(e => e.UsuarioId == userId)
                .ToListAsync();

            foreach (var endereco in enderecos)
            {
                DescriptografarEndereco(endereco);
            }

            return View(enderecos);
        }

        [HttpPost]
        public IActionResult Checkout(EnderecoEntrega enderecoEntrega)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                enderecoEntrega.UsuarioId = userId;

                CriptografarEndereco(enderecoEntrega);
                _context.EnderecosEntregas.Add(enderecoEntrega);
                _context.SaveChanges();

                return RedirectToAction("Checkout");
            }

            return View(enderecoEntrega);
        }

        [HttpPost]
        public async Task<IActionResult> FinalizarPedido(int enderecoId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var endereco = await _context.EnderecosEntregas
                .FirstOrDefaultAsync(e => e.EnderecoEntregaId == enderecoId && e.UsuarioId == userId);

            if (endereco == null)
                return NotFound("Endereço não encontrado.");

            var itensCarrinho = carrinhoCompra.GetCarrinhoCompraItems();
            if (itensCarrinho == null || !itensCarrinho.Any())
                return RedirectToAction("Carrinho", "CarrinhoCompra");

            var usuario = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (usuario == null)
                return Unauthorized();

            var pedido = new Pedido
            {
                UsuarioId = userId,
                PedidoTotal = itensCarrinho.Sum(i => i.Produtos.Preco * i.Quantidade),
                TotalItensPedidos = itensCarrinho.Sum(i => i.Quantidade),
                PedidoEnviado = DateTime.Now,
                EnderecoEntregaId = endereco.EnderecoEntregaId,
                PedidoItens = itensCarrinho.Select(item => new PedidoDetalhe
                {
                    ProdutoId = item.Produtos.Id_Produto,
                    Quantidade = item.Quantidade,
                    Preco = item.Produtos.Preco
                }).ToList(),
                Usuario = usuario
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            var emailhtml = await _razorViewToStringRenderer.RenderViewToStringAsync("Emails/EmailPedido", pedido);
            await _emailService.EnviarEmail(pedido.Usuario.Email, "Confirmação de Pedido", emailhtml);

            return RedirectToAction("CriarPagamento", "Pagamento", new { enderecoId = endereco.EnderecoEntregaId });
        }

        public IActionResult Confirmacao()
        {
            ViewBag.Mensagem = "Pedido realizado com sucesso!";
            return View();
        }

        public async Task<IActionResult> HistoricoPedidos()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var pedidos = await _context.Pedidos
                .Include(p => p.EnderecoEntrega)
                .Where(p => p.UsuarioId == userId && p.PedididoEntregue != null)
                .ToListAsync();

            foreach (var pedido in pedidos)
            {
                DescriptografarEndereco(pedido.EnderecoEntrega);
            }

            return View(pedidos);
        }

        public async Task<IActionResult> DetalhesPedido(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.PedidoItens).ThenInclude(pd => pd.Produto)
                .Include(p => p.EnderecoEntrega)
                .FirstOrDefaultAsync(p => p.PedidoId == id);

            if (pedido == null)
                return NotFound();

            return View(pedido);
        }

        public void DescriptografarEndereco(EnderecoEntrega endereco)
        {
            if (endereco != null)
            {
                endereco.Nome = _criptografia.Descriptografar(endereco.Nome);
                endereco.SobreNome = _criptografia.Descriptografar(endereco.SobreNome);
                endereco.Rua = _criptografia.Descriptografar(endereco.Rua);
                endereco.Bairro = _criptografia.Descriptografar(endereco.Bairro);
                endereco.Estado = _criptografia.Descriptografar(endereco.Estado);
                endereco.Cidade = _criptografia.Descriptografar(endereco.Cidade);
                endereco.Telefone = _criptografia.Descriptografar(endereco.Telefone);
                endereco.Cep = _criptografia.Descriptografar(endereco.Cep);
                endereco.CPF = _criptografia.Descriptografar(endereco.CPF);
                endereco.Numero = _criptografia.Descriptografar(endereco.Numero);
                endereco.Complemento = _criptografia.Descriptografar(endereco.Complemento);
            }
        }

        public void CriptografarEndereco(EnderecoEntrega endereco)
        {
            if (endereco != null)
            {
                endereco.Nome = _criptografia.Criptografar(endereco.Nome);
                endereco.SobreNome = _criptografia.Criptografar(endereco.SobreNome);
                endereco.Rua = _criptografia.Criptografar(endereco.Rua);
                endereco.Bairro = _criptografia.Criptografar(endereco.Bairro);
                endereco.Estado = _criptografia.Criptografar(endereco.Estado);
                endereco.Cidade = _criptografia.Criptografar(endereco.Cidade);
                endereco.Telefone = _criptografia.Criptografar(endereco.Telefone);
                endereco.Cep = _criptografia.Criptografar(endereco.Cep);
                endereco.CPF = _criptografia.Criptografar(endereco.CPF);
                endereco.Numero = _criptografia.Criptografar(endereco.Numero);
                endereco.Complemento = _criptografia.Criptografar(endereco.Complemento);
            }
        }
    }
}
