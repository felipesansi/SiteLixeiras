using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteLixeiras.Context;
using SiteLixeiras.Models;

namespace SiteLixeiras.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminPedidosController : Controller
    {
        private readonly AppDbContext _context;

        public AdminPedidosController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Usuario)
                .Include(p => p.EnderecoEntrega)
                .Include(p => p.PedidoItens)
                    .ThenInclude(i => i.Produto)
                .ToListAsync();

            foreach (var pedido in pedidos)
            {
                // Verifica se já existe uma notificação para esse pedido
                if (!_context.Notificacoes.Any(n => n.UsuarioId == pedido.UsuarioId && n.Mensagem.Contains($"Pedido #{pedido.PedidoId}")))
                {
                    // Cria uma notificação
                    _context.Notificacoes.Add(new Notificacao
                    {
                        UsuarioId = pedido.UsuarioId,
                        Mensagem = $"Seu pedido #{pedido.PedidoId} foi visualizado por um administrador.",
                        DataCriacao = DateTime.Now  // Definindo a data de criação da notificação
                    });
                }
            }

            await _context.SaveChangesAsync();

            return View(pedidos);
        }

        [HttpPost]
        public async Task<IActionResult> MarcarComoEntregue(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return NotFound();

            // Marca o pedido como entregue
            pedido.PedididoEntregue = DateTime.Now;

            // Cria uma notificação para o usuário informando que o pedido foi entregue
            _context.Notificacoes.Add(new Notificacao
            {
                UsuarioId = pedido.UsuarioId,
                Mensagem = $"Seu pedido #{pedido.PedidoId} foi marcado como entregue.",
                DataCriacao = DateTime.Now  // Data da notificação
            });

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
