using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteLixeiras.Context;
using SiteLixeiras.Models;

namespace SiteLixeiras.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminNotificacoesController : Controller
    {
        private readonly AppDbContext _context;

        public AdminNotificacoesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Todas()
        {
            var notificacoes = await _context.Notificacoes
                .Include(n => n.Usuario)
                .OrderByDescending(n => n.DataCriacao)
                .ToListAsync();

            return View(notificacoes);
        }

        public async Task<IActionResult> NovasRespostas()
        {
            var novas = await _context.Notificacoes
                .Include(n => n.Usuario)
                .Where(n => !n.EnviadaPeloAdmin && !n.Lida)
                .OrderByDescending(n => n.DataCriacao)
                .ToListAsync();

            return View(novas);
        }

      
        public async Task<IActionResult> Detalhes(int id)
        {
            var notificacao = await _context.Notificacoes
                .Include(n => n.Usuario)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (notificacao == null)
                return NotFound();

          
            if (!notificacao.EnviadaPeloAdmin && !notificacao.Lida)
            {
                notificacao.Lida = true;
                _context.Update(notificacao);
                await _context.SaveChangesAsync();
            }

            return View(notificacao);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Responder(int id, string resposta)
        {
            if (string.IsNullOrWhiteSpace(resposta))
            {
                TempData["Erro"] = "A resposta não pode ser vazia.";
                return RedirectToAction("Todas");
            }

            var notificacaoOriginal = await _context.Notificacoes
                .Include(n => n.Usuario)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (notificacaoOriginal == null)
                return NotFound();

            var novaNotificacao = new Notificacao
            {
                UsuarioId = notificacaoOriginal.UsuarioId,
                Mensagem = resposta,
                EnviadaPeloAdmin = true,
                Lida = false,
                DataCriacao = DateTime.Now
            };

            _context.Notificacoes.Add(novaNotificacao);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Resposta enviada com sucesso!";
            return RedirectToAction("Todas");
        }
    }
}
