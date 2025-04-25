using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SiteLixeiras.Context;
using SiteLixeiras.Models;

namespace SiteLixeiras.Controllers
{

    [Authorize]
    public class EnderecoEntregasController : Controller
    {
        private readonly AppDbContext _context;

        public EnderecoEntregasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: EnderecoEntregas
        public async Task<IActionResult> Index()
        {   var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var appDbContext = _context.EnderecosEntregas.Where(e => e.UsuarioId == userId);
            return View(await appDbContext.ToListAsync());
        }

        // GET: EnderecoEntregas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enderecoEntrega = await _context.EnderecosEntregas
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.EnderecoEntregaId == id);
            if (enderecoEntrega == null)
            {
                return NotFound();
            }

            return View(enderecoEntrega);
        }

        // GET: EnderecoEntregas/Create
        public IActionResult Create()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var endereco = new EnderecoEntrega
            {
                UsuarioId = userId
            };

            return View(endereco);
        }


        // POST: EnderecoEntregas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EnderecoEntregaId,UsuarioId,Nome,SobreNome,Rua,Bairro,Estado,Cidade,Telefone,Cep,CPF,Numero,Complemento")] EnderecoEntrega enderecoEntrega)
        {
            var enderecoExistente = await _context.EnderecosEntregas
                .FirstOrDefaultAsync(e => e.UsuarioId == enderecoEntrega.UsuarioId && e.Cep == enderecoEntrega.Cep && e.Numero == enderecoEntrega.Numero);

            if (enderecoExistente != null)
            {
                ModelState.AddModelError("Cep", "Esse endereço já está cadastrado.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(enderecoEntrega);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(enderecoEntrega);
        }

        // GET: EnderecoEntregas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enderecoEntrega = await _context.EnderecosEntregas.FindAsync(id);
            if (enderecoEntrega == null)
            {
                return NotFound();
            }
            ViewData["UsuarioId"] = new SelectList(_context.Users, "Id", "Id", enderecoEntrega.UsuarioId);
            return View(enderecoEntrega);
        }

        // POST: EnderecoEntregas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EnderecoEntregaId,UsuarioId,Nome,SobreNome,Rua,Bairro,Estado,Cidade,Telefone,Cep,CPF,Numero,Complemento")] EnderecoEntrega enderecoEntrega)
        {
            if (id != enderecoEntrega.EnderecoEntregaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enderecoEntrega);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnderecoEntregaExists(enderecoEntrega.EnderecoEntregaId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(enderecoEntrega);
        }

        // GET: EnderecoEntregas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enderecoEntrega = await _context.EnderecosEntregas
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.EnderecoEntregaId == id);
            if (enderecoEntrega == null)
            {
                return NotFound();
            }

            return View(enderecoEntrega);
        }

        // POST: EnderecoEntregas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enderecoEntrega = await _context.EnderecosEntregas.FindAsync(id);
            if (enderecoEntrega != null)
            {
                _context.EnderecosEntregas.Remove(enderecoEntrega);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnderecoEntregaExists(int id)
        {
            return _context.EnderecosEntregas.Any(e => e.EnderecoEntregaId == id);
        }
        // GET: /EnderecoEntregas/ConsultarCep?cep=12345678
        [HttpGet]
        public async Task<IActionResult> ConsultarCep(string cep)
        {

            if (string.IsNullOrWhiteSpace(cep) || cep.Length != 8)
                return Json(new { sucesso = false, mensagem = "CEP inválido." });

            try
            {
                if (cep.Contains("-"))
                    cep = cep.Replace("-", "");

                using var http = new HttpClient();
                var response = await http.GetStringAsync($"https://viacep.com.br/ws/{cep}/json/");
                var dados = JsonSerializer.Deserialize<Dictionary<string, string>>(response);

                if (dados.ContainsKey("erro"))
                    return Json(new { sucesso = false, mensagem = "CEP não encontrado." });

                return Json(new
                {
                    sucesso = true,
                    rua = dados["logradouro"],
                    bairro = dados["bairro"],
                    cidade = dados["localidade"],
                    estado = dados["uf"]
                });
            }
            catch
            {
                return Json(new { sucesso = false, mensagem = "Erro ao consultar o CEP." });
            }
        }

    }
}
