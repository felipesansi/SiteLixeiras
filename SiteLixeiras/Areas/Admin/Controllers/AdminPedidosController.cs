using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

        private readonly UserManager<IdentityUser> _userManager;

        public AdminPedidosController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
                
                if (!_context.Notificacoes.Any(n => n.UsuarioId == pedido.UsuarioId && n.Mensagem.Contains($"Pedido #{pedido.PedidoId}")))
                {
                    // Cria uma notificação
                    _context.Notificacoes.Add(new Notificacao
                    {
                        UsuarioId = pedido.UsuarioId,
                        Mensagem = $"Seu pedido #{pedido.PedidoId} foi visualizado por um administrador.",
                        DataCriacao = DateTime.Now,
                        EnviadaPeloAdmin = true,
                        Lida = false
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
                Mensagem = $"Seu pedido #{pedido.PedidoId} Seu pedido foi entregue a transportadora ou mercado livre para ser entregue no seu endereço.",
                EnviadaPeloAdmin = true,
                Lida = false,
                DataCriacao = DateTime.Now 
            });

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> EnviarMensagem(string usuarioId, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(mensagem)) return BadRequest("A mensagem não pode estar vazia.");

            var usuario = await _userManager.FindByIdAsync(usuarioId);
            if (usuario == null) return NotFound("Usuário não encontrado.");

            var notificacao = new Notificacao
            {
                UsuarioId = usuarioId,
                Mensagem = mensagem,
                DataCriacao = DateTime.Now
            };

            _context.Notificacoes.Add(notificacao);
            await _context.SaveChangesAsync();

            TempData["MensagemEnviada"] = "Mensagem enviada com sucesso!";
            return RedirectToAction("Index");
        }
    }
}
