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
    public class AdminNotificacoesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminNotificacoesController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Todas()
        {
            var notificacoes = await _context.Notificacoes
                .Include(n => n.Usuario)
                .Where(n => n.NotificacaoPaiId == null)
                .OrderByDescending(n => n.DataCriacao)
                .ToListAsync();

            ViewBag.Usuarios = await _context.Users.OrderBy(u => u.UserName).ToListAsync();

            return View(notificacoes);
        }




        public async Task<IActionResult> Detalhes(int id)
        {
            // Garante que sempre pega a notificação raiz da conversa
            var notificacaoPai = await _context.Notificacoes
                .Include(n => n.Usuario)
                .Include(n => n.Respostas.OrderBy(r => r.DataCriacao))
                .FirstOrDefaultAsync(n => n.Id == id);

            if (notificacaoPai == null)
            {
                // Se for resposta, pega o pai
                var resposta = await _context.Notificacoes
                    .Include(n => n.Usuario)
                    .FirstOrDefaultAsync(n => n.Id == id);

                if (resposta == null)
                    return NotFound();

                notificacaoPai = await _context.Notificacoes
                    .Include(n => n.Usuario)
                    .Include(n => n.Respostas.OrderBy(r => r.DataCriacao))
                    .FirstOrDefaultAsync(n => n.Id == resposta.NotificacaoPaiId);

                if (notificacaoPai == null)
                    return NotFound();
            }

        
            var respostasNaoLidas = notificacaoPai.Respostas
                .Where(r => !r.EnviadaPeloAdmin && !r.Lida)
                .ToList();

            foreach (var resposta in respostasNaoLidas)
                resposta.Lida = true;

            await _context.SaveChangesAsync();

            return View(notificacaoPai);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Responder(int id, string resposta)
        {
            if (string.IsNullOrWhiteSpace(resposta))
            {
                TempData["Erro"] = "A resposta não pode ser vazia.";
                return RedirectToAction("Detalhes", new { id });
            }

            var notificacaoOriginal = await _context.Notificacoes
                .FirstOrDefaultAsync(n => n.Id == id);

            if (notificacaoOriginal == null)
                return NotFound();

          
            int idMensagemPai = notificacaoOriginal.NotificacaoPaiId ?? notificacaoOriginal.Id;

            var novaResposta = new Notificacao
            {
                UsuarioId = notificacaoOriginal.UsuarioId,
                Mensagem = resposta,
                EnviadaPeloAdmin = true,
                Lida = false,
                DataCriacao = DateTime.Now,
                NotificacaoPaiId = idMensagemPai
            };

            _context.Notificacoes.Add(novaResposta);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Resposta enviada ao usuário.";
            return RedirectToAction("Detalhes", new { id = idMensagemPai });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnviarNovaMensagem(string id, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(mensagem))
            {
                TempData["Erro"] = "ID do usuário e mensagem são obrigatórios.";
                return RedirectToAction(nameof(Todas));
            }

            try
            {
                var usuario = await _userManager.FindByIdAsync(id);
                if (usuario == null)
                {
                    TempData["Erro"] = "Usuário não encontrado.";
                    return RedirectToAction(nameof(Todas));
                }

                var notificacao = new Notificacao
                {
                    UsuarioId = usuario.Id,
                    Mensagem = mensagem,
                    EnviadaPeloAdmin = true,
                    Lida = false,
                    DataCriacao = DateTime.Now
                };

                _context.Notificacoes.Add(notificacao);
                await _context.SaveChangesAsync();

                TempData["Sucesso"] = "Mensagem enviada com sucesso.";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = $"Erro ao salvar a notificação: {ex.InnerException?.Message ?? ex.Message}";
            }

            return RedirectToAction(nameof(Todas));
        }


    }
}
