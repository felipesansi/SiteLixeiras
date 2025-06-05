using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SiteLixeiras.Context;
using SiteLixeiras.Models;
using SiteLixeiras.Repositorios.Interfaces;

namespace SiteLixeiras.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminFotoController : Controller
    {
        private readonly IUploadFotosService _uploadFotosService;
        private readonly AppDbContext _context;

        public AdminFotoController(IUploadFotosService uploadFotosService, AppDbContext context)
        {
            _uploadFotosService = uploadFotosService;
            _context = context;
        }

        private void CarregarProdutos()
        {
            var produtos = _context.Produtos.ToList();
            ViewData["ListaProdutos"] = new SelectList(produtos, "Id_Produto", "Nome");
        }

        [HttpGet]
        public IActionResult EnviarFotos()
        {
            CarregarProdutos();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EnviarFotos(IFormFile file, int Id_Produto, bool DefinirComoCapa = false)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("file", "Selecione um arquivo para upload.");
                CarregarProdutos();
                return View();
            }

            var fotosExistentes = await _context.Fotos
                .Where(f => f.ProdutoId == Id_Produto)
                .ToListAsync();

            if (!fotosExistentes.Any() && !DefinirComoCapa)
            {
                ModelState.AddModelError("file", "Defina a primeira imagem como capa antes de enviar.");
                CarregarProdutos();
                return View();
            }

            var nomeUnico = $"{Guid.NewGuid()}_{file.FileName}";
            var caminhoDropbox = $"/LixeirasIcena/{nomeUnico}";

            try
            {
                // Upload e captura da URL original do Dropbox (com dl=0)
                var urlOriginal = await _uploadFotosService.UploadFileAsync(file, caminhoDropbox);

                // Ajusta para exibição direta
                var urlExibicao = urlOriginal.Replace("dl=0", "raw=1");

                var produto = await _context.Produtos.FindAsync(Id_Produto);
                if (produto == null)
                {
                    ModelState.AddModelError("file", "Produto não encontrado.");
                    CarregarProdutos();
                    return View();
                }

                var novaFoto = new Foto
                {
                    ProdutoId = Id_Produto,
                    Url = urlExibicao,
                    CaminhoDropbox = caminhoDropbox
                };

                _context.Fotos.Add(novaFoto);
                await _context.SaveChangesAsync();

                if (DefinirComoCapa)
                {
                    produto.ImagemThumbUrl = urlExibicao;
                    _context.Produtos.Update(produto);
                    await _context.SaveChangesAsync();
                }

                ViewData["MensagemSucesso"] = "Foto enviada com sucesso!";
                ViewData["LinkImagem"] = urlExibicao;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Erro ao enviar a foto: {ex.Message}");
                Console.WriteLine(ex);
            }

            CarregarProdutos();
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ListarFotos(int id)
        {
            var fotos = await _context.Fotos
                .Include(f => f.Produto)
                .Where(f => f.ProdutoId == id)
                .ToListAsync();

            return View(fotos);
        }

        [HttpPost]
        public async Task<IActionResult> ExcluirFoto(int id)
        {
            var foto = await _context.Fotos.FindAsync(id);
            if (foto == null)
                return NotFound();

            await _uploadFotosService.DeleteFileAsync(foto.CaminhoDropbox);

            _context.Fotos.Remove(foto);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ListarFotos), new { id = foto.ProdutoId });
        }
    }
}
