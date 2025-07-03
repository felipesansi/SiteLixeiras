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
            ViewBag.MetaTitle = "Lixeiras de Resina | Cestos Artesanais para Ambientes Sofisticados";
            ViewBag.MetaDescription = "Explore nossa coleção de lixeiras de resina feitas à mão com design elegante e exclusivo. Ideal para banheiros, lavabos e decoração refinada.";
            ViewBag.MetaKeywords = "lixeiras de resina, lixeira artesanal, cesto decorativo, decoração de luxo, lavabo, banheiro, resina poliéster, Lixeiras de resina";

            var produtos = await _context.Produtos
                .Where(p => p.Destaque)
                .Include(p => p.Fotos)
                .ToListAsync();

            await CarregarNotificacoes();
            return View(produtos);
        }

        public async Task<IActionResult> About()
        {
            ViewBag.MetaTitle = "Sobre a Lixeiras de resina | Lixeiras de Resina Artesanal";
            ViewBag.MetaDescription = "Conheça a história da lixeiras de resina e como criamos peças decorativas em resina com exclusividade e sofisticação.";
            ViewBag.MetaKeywords = "lixeira de resina, sobre lixeiras de resina, quem somos, lixeiras artesanais, história, missão, resina de luxo";

            await CarregarNotificacoes();
            return View();
        }

        public async Task<IActionResult> Produtos(decimal? precoMin, decimal? precoMax)
        {
            ViewBag.MetaTitle = "Catálogo de Lixeiras de Resina | Todos os Produtos";
            ViewBag.MetaDescription = "Veja todos os modelos de lixeiras de resina disponíveis. Escolha o design ideal para o seu espaço com a elegância da Lixeiras de resina.";
            ViewBag.MetaKeywords = "produtos de resina, catálogo lixeiras, lixeira para banheiro, lavabo, luxo, decoração, resina artesanal";

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

        public async Task<IActionResult> Privacy()
        {
            ViewBag.MetaTitle = "Política de Privacidade - Lixeiras de Resina";
            ViewBag.MetaDescription = "Entenda como protegemos suas informações e garantimos sua privacidade em nosso site.";
            ViewBag.MetaKeywords = "lixeiras de resina, cesto, política de privacidade, proteção de dados, informações pessoais, segurança, site Lixeiras de resina";

            await CarregarNotificacoes();
            return View();
        }

        public IActionResult Catalogo()
        {
            ViewBag.MetaTitle = "Catálogo de Cores | Lixeiras de Resina Personalizadas";
            ViewBag.MetaDescription = "Descubra todas as cores disponíveis para personalizar suas lixeiras de resina de forma única e elegante.";
            ViewBag.MetaKeywords = "catálogo de cores, personalização, lixeiras sob medida, lixeira decorativa, resina pigmentada";

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            ViewBag.MetaTitle = "Erro no Site - Lixeiras de Resina";
            ViewBag.MetaDescription = "Ocorreu um erro inesperado. Tente novamente mais tarde ou entre em contato com o suporte.";
            ViewBag.MetaKeywords = "erro, falha, problema, lixeiras de resina, suporte técnico";

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
    }
}
