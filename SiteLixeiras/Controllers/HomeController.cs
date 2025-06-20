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
            ViewBag.MetaTitle = "Lixeiras de Resina - Produtos em Destaque";
            ViewBag.MetaDescription = "Confira as melhores lixeiras de resina, produtos em destaque e soluções sustentáveis para o seu ambiente.";
            ViewBag.MetaKeywords = "Lixeiras de Resina, Cesto de Resina, lixeiras, Cesto, resina, sustentabilidade, produtos, destaque";
            ViewBag.MetaImage = Url.Content("~/imagens/imagem-compartilhamento.png");
            ViewBag.MetaUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";

            var produtos = await _context.Produtos
                .Where(p => p.Destaque)
                .Include(p => p.Fotos)
                .ToListAsync();

            await CarregarNotificacoes();

            return View(produtos);
        }

        public async Task<IActionResult> About()
        {
            ViewBag.MetaTitle = "Sobre Nós - Lixeiras de Resina";
            ViewBag.MetaDescription = "Conheça a história, missão e valores da Lixeiras de Resina.";
            ViewBag.MetaKeywords = "sobre, empresa, missão, valores, lixeiras de resina";

            await CarregarNotificacoes();
            return View();
        }

        public async Task<IActionResult> Produtos(decimal? precoMin, decimal? precoMax)
        {
            ViewBag.MetaTitle = "Todos os Produtos - Lixeiras de Resina";
            ViewBag.MetaDescription = "Veja todos os produtos de lixeiras de resina disponíveis para compra.";
            ViewBag.MetaKeywords = "Produtos de resina,produtos, lixeiras, resina, catálogo, comprar";

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
            ViewBag.MetaTitle = "Erro - Lixeiras de Resina";
            ViewBag.MetaDescription = "Ocorreu um erro ao processar sua solicitação.";
            ViewBag.MetaKeywords = "erro, lixeiras de resina";

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task CarregarNotificacoes()
        {
            if (User.IsInRole("User"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var notificacoes = await _context.Notificacoes
                    .Where(n => n.UsuarioId == userId && !n.Lida)
                    .OrderByDescending(n => n.DataCriacao)
                    .ToListAsync();

                ViewBag.Notificacoes = notificacoes;
            }
        }

        public async Task<IActionResult> Privacy()
        {
            ViewBag.MetaTitle = "Política de Privacidade - Lixeiras de Resina";
            ViewBag.MetaDescription = "Saiba como tratamos seus dados e garantimos sua privacidade.";
            ViewBag.MetaKeywords = "privacidade, dados, política, lixeiras de resina";

            await CarregarNotificacoes();
            return View();
        }

        public IActionResult Catalogo()
        {
            ViewBag.MetaTitle = "Catálogo de Cores - Lixeiras de Resina";
            ViewBag.MetaDescription = "Confira o catálogo de cores disponíveis para as lixeiras de resina.";
            ViewBag.MetaKeywords = "catálogo, cores, lixeiras, resina";

            return View();
        }
    }
}
