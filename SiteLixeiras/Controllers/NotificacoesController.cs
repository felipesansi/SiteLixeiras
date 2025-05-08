using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiteLixeiras.Context;
using System.Security.Claims;
using System.Linq;

[Authorize]
public class NotificacoesController : Controller
{
    private readonly AppDbContext _context;

    public NotificacoesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult MarcarTodasComoLidas()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var notificacoes = _context.Notificacoes
            .Where(n => n.UsuarioId == userId && !n.Lida)
            .ToList();

        if (notificacoes.Any())
        {
            foreach (var notificacao in notificacoes)
            {
                notificacao.Lida = true;
            }

            _context.SaveChanges();
        }

        return RedirectToAction("Index", "Home");
    }
}
