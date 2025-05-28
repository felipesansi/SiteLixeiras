using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteLixeiras.Context;
using SiteLixeiras.Models;

namespace SiteLixeiras.Controllers
{
    [Authorize(Roles = "User")]
    public class NotificacoesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public NotificacoesController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var notificacoes = await _context.Notificacoes
                .Where(n => n.UsuarioId == userId)
                .OrderByDescending(n => n.DataCriacao)
                .ToListAsync();


            var naoLidas = notificacoes
                .Where(n => !n.Lida && n.EnviadaPeloAdmin)
                .ToList();

            if (naoLidas.Any())
            {
                foreach (var n in naoLidas)
                    n.Lida = true;

                await _context.SaveChangesAsync();
            }

            return View(notificacoes);
        }

        public async Task<IActionResult> Detalhes(int id)
        {
            var userId = _userManager.GetUserId(User);

            var notificacao = await _context.Notificacoes
                .Include(n => n.Respostas)
                .FirstOrDefaultAsync(n => n.Id == id && n.UsuarioId == userId);

            if (notificacao == null)
                return NotFound();

            return View(notificacao);
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

            var userId = _userManager.GetUserId(User);

            var notificacaoOriginal = await _context.Notificacoes
                .FirstOrDefaultAsync(n => n.Id == id && n.UsuarioId == userId);

            if (notificacaoOriginal == null)
                return NotFound();


            var novaNotificacao = new Notificacao
            {
                UsuarioId = userId,
                Mensagem = resposta,
                EnviadaPeloAdmin = false,
                Lida = false,
                DataCriacao = DateTime.Now,
                NotificacaoPaiId = notificacaoOriginal.Id
            };

            _context.Notificacoes.Add(novaNotificacao);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Resposta enviada com sucesso!";
            return RedirectToAction("Detalhes", new { id = notificacaoOriginal.Id });
        }
        [HttpGet]
        public IActionResult EnviarNovaMensagem()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnviarNovaMensagem(string mensagem)
        {
            if (string.IsNullOrWhiteSpace(mensagem))
            {
                TempData["Erro"] = "A mensagem não pode estar vazia.";
                return View("NovaMensagem");
            }

            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
            {
                TempData["Erro"] = "Usuário não encontrado.";
                return View("Index");
            }

            var notificacao = new Notificacao
            {
                UsuarioId = usuario.Id,
                Mensagem = mensagem,
                EnviadaPeloAdmin = false,
                Lida = false,
                DataCriacao = DateTime.Now
            };

            _context.Notificacoes.Add(notificacao);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Mensagem enviada ao administrador com sucesso.";
            return RedirectToAction("Index");
        }
    }
}

    

