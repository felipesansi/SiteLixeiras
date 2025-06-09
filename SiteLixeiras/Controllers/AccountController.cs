using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SiteLixeiras.Context;
using SiteLixeiras.Models;
using SiteLixeiras.Services;
using SiteLixeiras.Sevices;
using SiteLixeiras.ViewModel;
using System.Net.Mail;

namespace SiteLixeiras.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly EmailSetting _emailSetting;
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;
        private readonly RazorViewToStringRenderer _razorRenderer;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IOptions<EmailSetting> emailSetting,
            AppDbContext context,
            EmailService emailService,
            RazorViewToStringRenderer razorRenderer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSetting = emailSetting.Value;
            _context = context;
            _emailService = emailService;
            _razorRenderer = razorRenderer;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null) =>
            View(new LoginViewModel { ReturnUrl = returnUrl });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByNameAsync(model.UserName)
                    ?? (model.UserName.Contains("@") ? await _userManager.FindByEmailAsync(model.UserName) : null);

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                if (result.Succeeded)
                    return string.IsNullOrEmpty(model.ReturnUrl) ? RedirectToAction("Index", "Home") : Redirect(model.ReturnUrl);
            }

            TempData["Erro"] = "Usuário ou senha inválidos.";
            return View(model);
        }

        [HttpGet]
        public IActionResult Register() => View(new RegisterViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!model.AceiteTermos)
            {
                ModelState.AddModelError("", "Você deve aceitar os Termos de Uso.");
                return View(model);
            }
            if (!ModelState.IsValid) return View(model);

            if (model.Password != model.ConfirmPassword)
                ModelState.AddModelError("", "As senhas não conferem.");

            if (model.Password.Length < 6)
                ModelState.AddModelError("", "A senha deve ter pelo menos 6 caracteres.");

            if (model.UserName.Length < 3 || model.UserName.Length > 20)
                ModelState.AddModelError("", "Nome de usuário deve ter entre 3 e 20 caracteres.");

            if (model.Email.Length < 5 || model.Email.Length > 50)
                ModelState.AddModelError("", "E-mail deve ter entre 5 e 50 caracteres.");

            if ((await _userManager.FindByEmailAsync(model.Email)) != null)
                ModelState.AddModelError("", "E-mail já cadastrado.");

            if ((await _userManager.FindByNameAsync(model.UserName)) != null)
                ModelState.AddModelError("", "Nome de usuário já cadastrado.");

            if (model.UserName.Contains(" "))
                ModelState.AddModelError("", "O nome de usuário não pode conter espaços.");

            if (model.UserName.Count(char.IsDigit) > 2)
                ModelState.AddModelError("", "O nome de usuário não pode conter mais de dois números.");

            if (!ModelState.IsValid) return View(model);

            var user = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed = true
            };
             
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, false);

                var mensagemHtml = await _razorRenderer.RenderViewToStringAsync("Emails/EmailCadastro", model);
                await _emailService.EnviarEmail(model.Email, "Bem-vindo ao SiteLixeiras!", mensagemHtml);

                return RedirectToAction("Create", "EnderecoEntregas");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult EsqueciSenha() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EsqueciSenha(ResetSenhaViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName)
                    ?? (model.UserName.Contains("@") ? await _userManager.FindByEmailAsync(model.UserName) : null);

            if (user == null)
            {
                TempData["Erro"] = "Usuário não encontrado.";
                return View(model);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetSenha", "Account", new { userId = user.Id, token }, Request.Scheme);
            var corpo = $"Clique no link para redefinir sua senha: {callbackUrl}";

            try
            {
                using var client = new SmtpClient(_emailSetting.Host)
                {
                    Port = _emailSetting.Port,
                    EnableSsl = _emailSetting.EnableSsl,
                    Credentials = new System.Net.NetworkCredential(_emailSetting.UserName, _emailSetting.Password)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSetting.Remetente),
                    Subject = "Redefinição de Senha",
                    Body = corpo,
                    IsBodyHtml = false
                };

                mailMessage.To.Add(user.Email);
                await client.SendMailAsync(mailMessage);
            }
            catch
            {
                ModelState.AddModelError("", "Erro ao enviar o e-mail.");
                return View(model);
            }

            return RedirectToAction("ConfirmacaoEnvioEmail");
        }

        public IActionResult ConfirmacaoEnvioEmail() => View();

        public IActionResult ResetSenha(string userId, string token) =>
            View(new ResetSenhaViewModel { UserId = userId, Token = token });

        [HttpPost]
        public async Task<IActionResult> ResetSenha(ResetSenhaViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ModelState.AddModelError("", "Usuário não encontrado.");
                return View(model);
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NovaSenha);
            if (result.Succeeded)
                return RedirectToAction("Login");

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Configuracao()
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null) return RedirectToAction("Login");

            var dadosUsuario = await _context.Users.FirstOrDefaultAsync(x => x.Id == usuario.Id);
            var enderecos = await _context.EnderecosEntregas.Where(e => e.UsuarioId == usuario.Id).ToListAsync();

            var viewModel = new UsuarioViewmodel
            {
                Id = dadosUsuario.Id,
                UserName = dadosUsuario.UserName,
                Email = dadosUsuario.Email,
                Telefone = enderecos.FirstOrDefault()?.Telefone,
                Entrega = enderecos
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Configuracao(UsuarioViewmodel model)
        {
            if (!ModelState.IsValid) return View(model);

            var usuario = await _userManager.FindByIdAsync(model.Id);
            if (usuario == null)
            {
                ModelState.AddModelError("", "Usuário não encontrado.");
                return View(model);
            }

            usuario.UserName = model.UserName;
            usuario.Email = model.Email;

            var result = await _userManager.UpdateAsync(usuario);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }

            var endereco = await _context.EnderecosEntregas.FirstOrDefaultAsync(e => e.UsuarioId == usuario.Id);
            if (endereco != null)
            {
                endereco.Telefone = model.Telefone;
                await _context.SaveChangesAsync();
            }

            if (usuario.UserName != User.Identity?.Name)
            {
                TempData["MensagemSucesso"] = "Nome de usuário atualizado com sucesso! Faça login novamente.";
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login");
            }

            TempData["MensagemSucesso"] = "Dados atualizados com sucesso!";
            return View(model);
        }

        [HttpGet]
        public IActionResult ExcluirConta() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirConta(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha))
            {
                ModelState.AddModelError("", "Preencha o campo de confirmação.");
                return View();
            }

            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
            {
                TempData["MensagemErro"] = "Usuário não encontrado.";
                return RedirectToAction("Login");
            }

            var senhaValida = await _userManager.CheckPasswordAsync(usuario, senha);
            if (!senhaValida)
            {
                ModelState.AddModelError("", "Senha incorreta.");
                return View();
            }

            var result = await _userManager.DeleteAsync(usuario);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View();
            }

            await _signInManager.SignOutAsync();
            return RedirectToAction("ContaExcluida");
        }

        public IActionResult ContaExcluida() => View();
        public IActionResult AccessDenied() => View();
        public IActionResult TermosUso() => View();
    }
}
