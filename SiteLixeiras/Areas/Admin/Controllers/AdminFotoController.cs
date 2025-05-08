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


        public void CarregarProdutos()
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

            var fotoExistente = await _context.Fotos.Where(f => f.ProdutoId == Id_Produto).ToListAsync();

            // Se não houver fotos existentes, exigir que a primeira seja definida como capa
            if (!fotoExistente.Any() && !DefinirComoCapa)
            {
                ModelState.AddModelError("file", "Defina a primeira imagem como capa antes de enviar.");
                CarregarProdutos();
                return View();
            }

            var nomeUnico = $"{Guid.NewGuid()}_{file.FileName}";
            var caminhoDestino = $"/LixeirasIcena/{nomeUnico}";

            try
            {
                var urlImagem = await _uploadFotosService.UploadFileAsync(file, caminhoDestino);
                var urlAjustada = urlImagem.Replace("dl=0", "raw=1");

                var produto = await _context.Produtos.FindAsync(Id_Produto);

                if (produto == null)
                {
                    ModelState.AddModelError("file", "Produto não encontrado.");
                    CarregarProdutos();
                    return View();
                }

                var novaFoto = new Foto { Url = urlAjustada, ProdutoId = Id_Produto };
                _context.Fotos.Add(novaFoto);
                await _context.SaveChangesAsync();

                // Se o usuário definiu a imagem como capa, atualiza o produto
                if (DefinirComoCapa)
                {
                    produto.ImagemThumbUrl= urlAjustada;
                    _context.Produtos.Update(produto);
                    await _context.SaveChangesAsync();
                }

                ViewData["MensagemSucesso"] = "Foto enviada com sucesso!";
                ViewData["LinkImagem"] = urlAjustada;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Erro ao enviar a foto: {ex.Message}");
                Console.WriteLine(ex.ToString()); // Ajuda na depuração
            }

            CarregarProdutos();
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
