using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SiteLixeiras.Context;
using SiteLixeiras.Models;

namespace SiteLixeiras.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminProdutosController : Controller
    {
        private readonly AppDbContext _context;

        public AdminProdutosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminProdutos
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Produtos.Include(p => p.Categoria);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Admin/AdminProdutos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var produto = await _context.Produtos
                .Include(p => p.Categoria)
                .Include(p => p.ItensDoKit)
                .ThenInclude(i => i.ProdutoFilho)
                .FirstOrDefaultAsync(m => m.Id_Produto == id);

            if (produto == null) return NotFound();

            return View(produto);
        }

        // GET: Admin/AdminProdutos/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "IdCategoria", "Descricao");
            ViewData["ProdutosDisponiveis"] = _context.Produtos.ToList();
            return View();
        }

        // POST: Admin/AdminProdutos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Produtos produtos, int[]? produtosFilhos, int[]? quantidades)
        {
            if (ModelState.IsValid)
            {
                _context.Add(produtos);
                await _context.SaveChangesAsync();

                if (produtos.EhKit && produtosFilhos != null && quantidades != null)
                {
                    for (int i = 0; i < produtosFilhos.Length; i++)
                    {
                        var kitItem = new ProdutoKitItem
                        {
                            ProdutoKitId = produtos.Id_Produto,
                            ProdutoFilhoId = produtosFilhos[i],
                            Quantidade = quantidades[i]
                        };
                        _context.Add(kitItem);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "IdCategoria", "Descricao", produtos.CategoriaId);
            ViewData["ProdutosDisponiveis"] = _context.Produtos.ToList();
            return View(produtos);
        }

        // GET: Admin/AdminProdutos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var produto = await _context.Produtos
                .Include(p => p.ItensDoKit)
                .FirstOrDefaultAsync(p => p.Id_Produto == id);

            if (produto == null) return NotFound();

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "IdCategoria", "Descricao", produto.CategoriaId);
            ViewData["ProdutosDisponiveis"] = _context.Produtos
                .Where(p => p.Id_Produto != id)
                .ToList();

            return View(produto);
        }

        // POST: Admin/AdminProdutos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Produtos produtos, int[]? produtosFilhos, int[]? quantidades)
        {
            if (id != produtos.Id_Produto) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var produtoDb = await _context.Produtos
                        .Include(p => p.ItensDoKit)
                        .FirstOrDefaultAsync(p => p.Id_Produto == id);

                    if (produtoDb == null) return NotFound();

                    if (string.IsNullOrEmpty(produtos.Imagem))
                        produtos.Imagem = produtoDb.Imagem;
                    if (string.IsNullOrEmpty(produtos.ImagemThumbUrl))
                        produtos.ImagemThumbUrl = produtoDb.ImagemThumbUrl;

                    _context.Entry(produtoDb).CurrentValues.SetValues(produtos);

                    if (produtos.EhKit)
                    {
                        _context.RemoveRange(produtoDb.ItensDoKit);

                        if (produtosFilhos != null && quantidades != null)
                        {
                            for (int i = 0; i < produtosFilhos.Length; i++)
                            {
                                var kitItem = new ProdutoKitItem
                                {
                                    ProdutoKitId = produtos.Id_Produto,
                                    ProdutoFilhoId = produtosFilhos[i],
                                    Quantidade = quantidades[i]
                                };
                                _context.Add(kitItem);
                            }
                        }
                    }
                    else
                    {
                        _context.RemoveRange(produtoDb.ItensDoKit);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutosExists(produtos.Id_Produto)) return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "IdCategoria", "Descricao", produtos.CategoriaId);
            ViewData["ProdutosDisponiveis"] = _context.Produtos.ToList();
            return View(produtos);
        }

        // GET: Admin/AdminProdutos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var produto = await _context.Produtos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id_Produto == id);

            if (produto == null) return NotFound();

            return View(produto);
        }

        // POST: Admin/AdminProdutos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produto = await _context.Produtos
                .Include(p => p.ItensDoKit)
                .FirstOrDefaultAsync(p => p.Id_Produto == id);

            if (produto != null)
            {
                _context.RemoveRange(produto.ItensDoKit);
                _context.Produtos.Remove(produto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProdutosExists(int id)
        {
            return _context.Produtos.Any(e => e.Id_Produto == id);
        }
    }
}
