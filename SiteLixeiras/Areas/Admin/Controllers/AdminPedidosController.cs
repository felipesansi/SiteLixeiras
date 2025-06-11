using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SiteLixeiras.Context;
using SiteLixeiras.Models;
using SiteLixeiras.Helpers;

namespace SiteLixeiras.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminPedidosController : Controller
    {
        private readonly AppDbContext _context;

        private readonly UserManager<IdentityUser> _userManager;

        private CriptografiaHelper _criptografia;

        public AdminPedidosController(AppDbContext context, UserManager<IdentityUser> userManager, CriptografiaHelper criptografia)
        {
            _context = context;
            _userManager = userManager;
            _criptografia = criptografia;
        }

        public async Task<IActionResult> Index()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Usuario)
                .Include(p => p.EnderecoEntrega)
                .Include(p => p.PedidoItens)
                    .ThenInclude(i => i.Produto)
                .ToListAsync();

            // Descriptografa os endereços de entrega
            foreach (var pedido in pedidos)
            {
                if (pedido.EnderecoEntrega != null)
                {
                    DescriptografarEndereco(pedido.EnderecoEntrega);
                }
            }
            foreach (var pedido in pedidos)
            {

                if (!_context.Notificacoes.Any(n => n.UsuarioId == pedido.UsuarioId && n.Mensagem.Contains($"Pedido #{pedido.PedidoId}")))
                {
                    // Cria uma notificação
                    _context.Notificacoes.Add(new Notificacao
                    {
                        UsuarioId = pedido.UsuarioId,
                        Mensagem = $"Seu pedido #{pedido.PedidoId} foi visualizado por um administrador.",
                        DataCriacao = DateTime.Now,
                        EnviadaPeloAdmin = true,
                        Lida = false
                    });
                }
            }

            await _context.SaveChangesAsync();

            return View(pedidos);
        }

        [HttpPost]
        public async Task<IActionResult> MarcarComoEntregue(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null) return NotFound();

            // Marca o pedido como entregue
            pedido.PedididoEntregue = DateTime.Now;

            // Cria uma notificação para o usuário informando que o pedido foi entregue
            _context.Notificacoes.Add(new Notificacao
            {
                UsuarioId = pedido.UsuarioId,
                Mensagem = $"Seu pedido #{pedido.PedidoId} Seu pedido foi entregue a transportadora ou mercado livre para ser entregue no seu endereço.",
                EnviadaPeloAdmin = true,
                Lida = false,
                DataCriacao = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> EnviarMensagem(string usuarioId, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(mensagem)) return BadRequest("A mensagem não pode estar vazia.");

            var usuario = await _userManager.FindByIdAsync(usuarioId);
            if (usuario == null) return NotFound("Usuário não encontrado.");

            var notificacao = new Notificacao
            {
                UsuarioId = usuarioId,
                Mensagem = mensagem,
                DataCriacao = DateTime.Now
            };

            _context.Notificacoes.Add(notificacao);
            await _context.SaveChangesAsync();

            TempData["MensagemEnviada"] = "Mensagem enviada com sucesso!";
            return RedirectToAction("Index");
        }
        public void DescriptografarEndereco(EnderecoEntrega endereco)
        {
            endereco.Nome = _criptografia.Descriptografar(endereco.Nome);
            endereco.SobreNome = _criptografia.Descriptografar(endereco.SobreNome);

            endereco.Rua = _criptografia.Descriptografar(endereco.Rua);
            endereco.Bairro = _criptografia.Descriptografar(endereco.Bairro);
            endereco.Estado = _criptografia.Descriptografar(endereco.Estado);
            endereco.Cidade = _criptografia.Descriptografar(endereco.Cidade);
            endereco.Telefone = _criptografia.Descriptografar(endereco.Telefone);
            endereco.Cep = _criptografia.Descriptografar(endereco.Cep);
            endereco.CPF = _criptografia.Descriptografar(endereco.CPF);
            endereco.Numero = _criptografia.Descriptografar(endereco.Numero);
            endereco.Complemento = _criptografia.Descriptografar(endereco.Complemento);

        }
    }
}