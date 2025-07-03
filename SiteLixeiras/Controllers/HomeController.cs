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
            ViewBag.MetaDescription = "Explore nossa cole��o de lixeiras de resina feitas � m�o com design elegante e exclusivo. Ideal para banheiros, lavabos e decora��o refinada.";
            ViewBag.MetaKeywords = "lixeiras de resina, lixeira artesanal, cesto decorativo, decora��o de luxo, lavabo, banheiro, resina poli�ster, Lixeiras de resina";

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
            ViewBag.MetaDescription = "Conhe�a a hist�ria da lixeiras de resina e como criamos pe�as decorativas em resina com exclusividade e sofistica��o.";
            ViewBag.MetaKeywords = "lixeira de resina, sobre lixeiras de resina, quem somos, lixeiras artesanais, hist�ria, miss�o, resina de luxo";

            await CarregarNotificacoes();
            return View();
        }

        public async Task<IActionResult> Produtos(decimal? precoMin, decimal? precoMax)
        {
            ViewBag.MetaTitle = "Cat�logo de Lixeiras de Resina | Todos os Produtos";
            ViewBag.MetaDescription = "Veja todos os modelos de lixeiras de resina dispon�veis. Escolha o design ideal para o seu espa�o com a eleg�ncia da Lixeiras de resina.";
            ViewBag.MetaKeywords = "produtos de resina, cat�logo lixeiras, lixeira para banheiro, lavabo, luxo, decora��o, resina artesanal";

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
                    ViewBag.Mensagem = "Intervalo de pre�o inv�lido.";
                }
            }

            var produtos = await produtosQuery.ToListAsync();

            await CarregarNotificacoes();
            return View(produtos);
        }

        public async Task<IActionResult> Privacy()
        {
            ViewBag.MetaTitle = "Pol�tica de Privacidade - Lixeiras de Resina";
            ViewBag.MetaDescription = "Entenda como protegemos suas informa��es e garantimos sua privacidade em nosso site.";
            ViewBag.MetaKeywords = "lixeiras de resina, cesto, pol�tica de privacidade, prote��o de dados, informa��es pessoais, seguran�a, site Lixeiras de resina";

            await CarregarNotificacoes();
            return View();
        }

        public IActionResult Catalogo()
        {
            ViewBag.MetaTitle = "Cat�logo de Cores | Lixeiras de Resina Personalizadas";
            ViewBag.MetaDescription = "Descubra todas as cores dispon�veis para personalizar suas lixeiras de resina de forma �nica e elegante.";
            ViewBag.MetaKeywords = "cat�logo de cores, personaliza��o, lixeiras sob medida, lixeira decorativa, resina pigmentada";

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            ViewBag.MetaTitle = "Erro no Site - Lixeiras de Resina";
            ViewBag.MetaDescription = "Ocorreu um erro inesperado. Tente novamente mais tarde ou entre em contato com o suporte.";
            ViewBag.MetaKeywords = "erro, falha, problema, lixeiras de resina, suporte t�cnico";

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
