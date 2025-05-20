using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteLixeiras.Context;
using SiteLixeiras.Models;
using SiteLixeiras.Repositorios.Interfaces;
using System.Security.Claims;

namespace SiteLixeiras.Controllers
{
    public class PedidosController : Controller
    {
        private readonly IProdutosRepositorio _produtosRepositorio;
        private readonly IPedido _pedido;
        private readonly AppDbContext _context;
        private readonly CarrinhoCompra carrinhoCompra;

        public PedidosController(IProdutosRepositorio produtosRepositorio, IPedido pedido, AppDbContext context, CarrinhoCompra carrinhoCompra)
        {
            _produtosRepositorio = produtosRepositorio;
            _pedido = pedido;
            _context = context;
            this.carrinhoCompra = carrinhoCompra;
        }

        // Tela para exibir endereços cadastrados
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var enderecos = await _context.EnderecosEntregas
                .Where(e => e.UsuarioId == userId)
                .ToListAsync();

            return View(enderecos);
        }

        // Cadastro de novo endereço via POST
        [HttpPost]
        public IActionResult Checkout(EnderecoEntrega enderecoEntrega)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                enderecoEntrega.UsuarioId = userId;
                _context.EnderecosEntregas.Add(enderecoEntrega);
                _context.SaveChanges();
                return RedirectToAction("Checkout");
            }

            return View(enderecoEntrega);
        }

        // Finaliza o pedido e redireciona para o pagamento
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
                }).ToList()
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

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
                .Where(p => p.UsuarioId == userId && p.PedididoEntregue !=null) 
                .ToListAsync();
            return View(pedidos);
        }
        public async Task<IActionResult> DetalhesPedido(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.PedidoItens)
                .ThenInclude(pd => pd.Produto)
                .Include(p => p.EnderecoEntrega)
                .FirstOrDefaultAsync(p => p.PedidoId == id);
            if (pedido == null)
                return NotFound();
            return View(pedido);
        }
    }
}
