using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteLixeiras.Context;
using SiteLixeiras.Models;
using SiteLixeiras.Repositorios.Interfaces;

namespace SiteLixeiras.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly IProdutosRepositorio produtosRepositorio;

        public HomeController(ILogger<HomeController> logger, AppDbContext context, IProdutosRepositorio produtosRepositorio)
        {
            _logger = logger;
            _context = context;
            this.produtosRepositorio = produtosRepositorio;
        }

        public async Task<IActionResult> Index()
        {
            var produtos = await _context.Produtos
                .Where(p => p.Destaque)
                .Include(p => p.Fotos)
                .ToListAsync();

            await CarregarNotificacoes();

            return View(produtos);
        }

        public async Task<IActionResult> About()
        {
            await CarregarNotificacoes();
            return View();
        }

        public async Task<IActionResult> Produtos(decimal? precoMin, decimal? precoMax)
        {
            var produtosQuery = _context.Produtos
                .Include(p => p.Fotos)
                .Include(p => p.Categoria) 
                .AsQueryable();

            if (precoMin.HasValue && precoMax.HasValue)
            {
                if (precoMin <= precoMax)
                {
                    produtosQuery = produtosQuery
                        .Where(p => p.Preco >= precoMin && p.Preco <= precoMax);
                    ViewData["precoMin"] = precoMin;
                    ViewData["precoMax"] = precoMax;
                }
                else
                {
                    ViewBag.Mensagem = "Intervalo de preço inválido.";
                }
            }

            var produtos = await produtosQuery.ToListAsync();

            await CarregarNotificacoes();

            return View(produtos);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
        private async Task CarregarNotificacoes()
        {
            if (User.IsInRole("User"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Obtém o ID do usuário autenticado

                var notificacoes = await _context.Notificacoes
                    .Where(n => n.UsuarioId == userId && !n.Lida)
                    .OrderByDescending(n => n.DataCriacao)
                    .ToListAsync();

                ViewBag.Notificacoes = notificacoes;
            }
        }
        public async Task<IActionResult> Privacy()
        {
            await CarregarNotificacoes();
            return View();
        }
    }
}
