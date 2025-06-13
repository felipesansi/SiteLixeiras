using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteLixeiras.Context;
using System.Text;

namespace SiteLixeiras.Controllers
{
    public class SitemapController : Controller
    {
        private readonly AppDbContext _context;

        public SitemapController(AppDbContext context)
        {
            _context = context;
        }

        [Route("sitemap.xml")]
        public async Task<IActionResult> Sitemap()
        {
            var produtos = await _context.Produtos.ToListAsync();
            var categorias = await _context.Categorias.ToListAsync();

            var urls = new List<string>
            {
                "https://www.lixeirasderesina.com.br/",
                "https://www.lixeirasderesina.com.br/Home/Produtos",
                "https://www.lixeirasderesina.com.br/Home/About",
                "https://www.lixeirasderesina.com.br/Home/Catalogo"
                
            };

            var xml = new StringBuilder();
            xml.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
            xml.AppendLine(@"<urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"">");

            foreach (var url in urls)
            {
                xml.AppendLine("  <url>");
                xml.AppendLine($"    <loc>{url}</loc>");
                xml.AppendLine($"    <lastmod>{DateTime.UtcNow:yyyy-MM-dd}</lastmod>");
                xml.AppendLine("    <changefreq>weekly</changefreq>");
                xml.AppendLine("    <priority>0.8</priority>");
                xml.AppendLine("  </url>");
            }

            foreach (var categoria in categorias)
            {
                xml.AppendLine("  <url>");
                xml.AppendLine($"    <loc>https://www.lixeirasderesina.com.br/Categorias/Detalhes/{categoria.IdCategoria}</loc>");
                xml.AppendLine("    <changefreq>weekly</changefreq>");
                xml.AppendLine("    <priority>0.7</priority>");
                xml.AppendLine("  </url>");
            }

            foreach (var produto in produtos)
            {
                xml.AppendLine("  <url>");
                xml.AppendLine($"    <loc>https://www.lixeirasderesina.com.br/Produtos/Detalhes/{produto.Id_Produto}</loc>");
                xml.AppendLine("    <changefreq>weekly</changefreq>");
                xml.AppendLine("    <priority>0.9</priority>");
                xml.AppendLine("  </url>");
            }

            xml.AppendLine("</urlset>");
            return Content(xml.ToString(), "application/xml");
        }
    }
}
