using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteLixeiras.Context;
using SiteLixeiras.Models;
using SiteLixeiras.Repositorios.Interfaces;
using SiteLixeiras.Repositorios;

namespace SiteLixeiras.Controllers
{
    [Authorize]
    public class ProdutosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IProdutosRepositorio _produtosRepositorio;
        private readonly CarrinhoCompra _carrinhoCompra;

        public ProdutosController(AppDbContext context, IProdutosRepositorio produtosRepositorio, CarrinhoCompra carrinhoCompra)
        {
            _context = context;
            _produtosRepositorio = produtosRepositorio;
            _carrinhoCompra = carrinhoCompra;
        }

        public async Task<IActionResult> Detalhes(int id)
        {
            try
            {
                var produto = await _context.Produtos
                    .Include(f => f.Fotos)
                    .Include(c => c.Categoria)
                    .FirstOrDefaultAsync(p => p.Id_Produto == id);

                if (produto == null)
                {
                    return NotFound();
                }

                return View(produto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocorreu um erro ao carregar os dados do produto. " + ex.Message);
            }
        }


    }
}
