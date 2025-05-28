using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteLixeiras.Context;

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
            .Where(n => !n.Lida && !n.EnviadaPeloAdmin)
            .CountAsync();

        return View(count);
    }
}
