using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteLixeiras.Context;
using SiteLixeiras.Helpers;
using SiteLixeiras.Models;

namespace SiteLixeiras.Controllers
{
    [Authorize]
    public class EnderecoEntregasController : Controller
    {
        private readonly AppDbContext _context;
        private readonly CriptografiaHelper _criptografia;

        public EnderecoEntregasController(AppDbContext context, CriptografiaHelper criptografia)
        {
            _context = context;
            _criptografia = criptografia;
        }

        // GET: EnderecoEntregas
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var enderecos = await _context.EnderecosEntregas
                .Where(e => e.UsuarioId == userId)
                .ToListAsync();

            foreach (var e in enderecos)
            {
                DescriptografarEndereco(e);
            }

            return View(enderecos);
        }

        // GET: EnderecoEntregas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var endereco = await _context.EnderecosEntregas
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EnderecoEntregaId == id);

            if (endereco == null) return NotFound();

            DescriptografarEndereco(endereco);

            return View(endereco);

        }

        // GET: EnderecoEntregas/Create
        public IActionResult Create()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var model = new EnderecoEntrega { UsuarioId = userId };
            return View(model);
        }

        // POST: EnderecoEntregas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EnderecoEntrega endereco)
        {
            var enderecoExistente = await _context.EnderecosEntregas.FirstOrDefaultAsync(e =>
                e.UsuarioId == endereco.UsuarioId &&
                e.Cep == endereco.Cep &&
                e.Numero == endereco.Numero);

            if (enderecoExistente != null)
                ModelState.AddModelError("Cep", "Esse endereço já está cadastrado.");

            if (ModelState.IsValid)
            {
                CriptografarEndereco(endereco);

                _context.EnderecosEntregas.Add(endereco);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(endereco);
        }

        // GET: EnderecoEntregas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var endereco = await _context.EnderecosEntregas.FindAsync(id);
            if (endereco == null) return NotFound();

            DescriptografarEndereco(endereco);

            return View(endereco);

        }

        // POST: EnderecoEntregas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EnderecoEntrega endereco)
        {
            if (id != endereco.EnderecoEntregaId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    CriptografarEndereco(endereco);

                    _context.Update(endereco);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnderecoEntregaExists(endereco.EnderecoEntregaId))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(endereco);
        }

        // GET: EnderecoEntregas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var endereco = await _context.EnderecosEntregas
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EnderecoEntregaId == id);

            if (endereco == null) return NotFound();

            if (_context.Pedidos.Any(p => p.EnderecoEntregaId == id))
            {
                TempData["Erro"] = "Este endereço está associado a pedidos e não pode ser excluído.";
                return View(endereco);
            }

            return View(endereco);
        }

        // POST: EnderecoEntregas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var endereco = await _context.EnderecosEntregas.FindAsync(id);

            if (_context.Pedidos.Any(p => p.EnderecoEntregaId == id))
            {
                TempData["Erro"] = "Este endereço está associado a pedidos e não pode ser excluído.";
                return View(endereco);
            }

            if (endereco != null)
                _context.EnderecosEntregas.Remove(endereco);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnderecoEntregaExists(int id)
        {
            return _context.EnderecosEntregas.Any(e => e.EnderecoEntregaId == id);
        }

        [HttpGet]
        public async Task<IActionResult> ConsultarCep(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep) || cep.Length < 8)
                return Json(new { sucesso = false, mensagem = "CEP inválido." });

            try
            {
                cep = cep.Replace("-", "").Trim();
                using var http = new HttpClient();
                var json = await http.GetStringAsync($"https://viacep.com.br/ws/{cep}/json/");
                var dados = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                if (dados.ContainsKey("erro"))
                    return Json(new { sucesso = false, mensagem = "CEP não encontrado." });

                return Json(new
                {
                    sucesso = true,
                    rua = dados.GetValueOrDefault("logradouro"),
                    bairro = dados.GetValueOrDefault("bairro"),
                    cidade = dados.GetValueOrDefault("localidade"),
                    estado = dados.GetValueOrDefault("uf")
                });
            }
            catch
            {
                return Json(new { sucesso = false, mensagem = "Erro ao consultar o CEP." });
            }
        }

        private void CriptografarEndereco(EnderecoEntrega e)
        {
            e.Nome = _criptografia.Criptografar(e.Nome);
            e.SobreNome = _criptografia.Criptografar(e.SobreNome);
            e.Rua = _criptografia.Criptografar(e.Rua);
            e.Bairro = _criptografia.Criptografar(e.Bairro);
            e.Estado = _criptografia.Criptografar(e.Estado);
            e.Cidade = _criptografia.Criptografar(e.Cidade);
            e.Telefone = _criptografia.Criptografar(e.Telefone);
            e.Cep = _criptografia.Criptografar(e.Cep);
            e.CPF = _criptografia.Criptografar(e.CPF);
            e.Numero = _criptografia.Criptografar(e.Numero);
            e.Complemento = _criptografia.Criptografar(e.Complemento);
        }
        private void DescriptografarEndereco(EnderecoEntrega e)
        {
            e.Nome = _criptografia.Descriptografar(e.Nome);
            e.SobreNome = _criptografia.Descriptografar(e.SobreNome);
            e.Rua = _criptografia.Descriptografar(e.Rua);
            e.Bairro = _criptografia.Descriptografar(e.Bairro);
            e.Estado = _criptografia.Descriptografar(e.Estado);
            e.Cidade = _criptografia.Descriptografar(e.Cidade);
            e.Telefone = _criptografia.Descriptografar(e.Telefone);
            e.Cep = _criptografia.Descriptografar(e.Cep);
            e.CPF = _criptografia.Descriptografar(e.CPF);
            e.Numero = _criptografia.Descriptografar(e.Numero);
            e.Complemento = _criptografia.Descriptografar(e.Complemento);
        }

    }
}
