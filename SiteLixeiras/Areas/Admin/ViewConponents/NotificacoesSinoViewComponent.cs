using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteLixeiras.Context;

namespace SiteLixeiras.ViewComponents
{
    public class NotificacoesSinoViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public NotificacoesSinoViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var count = await _context.Notificacoes
                .Where(n => !n.Lida && n.Resposta == null)
                .CountAsync();

            return View(count);
        }
    }
}
