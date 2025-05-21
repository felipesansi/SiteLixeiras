using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SiteLixeiras.Models;
using SiteLixeiras.ViewModel;
using System.Net.Mail;

namespace SiteLixeiras.Controllers
{
    using Microsoft.Extensions.Options;

    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly EmailSetting _emailSetting;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IOptions<EmailSetting> emailOptions)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSetting = emailOptions.Value;
        }



        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            var viewModel = new LoginViewModel { ReturnUrl = returnUrl };
            return View(viewModel);
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            IdentityUser? usuario = await _userManager.FindByNameAsync(model.UserName) ??
                                    (model.UserName.Contains("@") ? await _userManager.FindByEmailAsync(model.UserName) : null);

            if (usuario != null)
            {
                var resultado = await _signInManager.PasswordSignInAsync(usuario, model.Password, false, false);
                if (resultado.Succeeded)
                {
                    return string.IsNullOrEmpty(model.ReturnUrl) ? RedirectToAction("Index", "Home") : Redirect(model.ReturnUrl);
                }
            }

            ModelState.AddModelError("", "Usuário ou senha inválidos.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            var viewModel = new RegisterViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Preencha todos os campos corretamente.");
                return View(model);
            }

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "As senhas não conferem.");
                return View(model);
            }

            if (model.Password.Length < 6)
            {
                ModelState.AddModelError("", "A senha deve ter pelo menos 6 caracteres.");
                return View(model);
            }

            if (model.UserName.Length < 3 || model.UserName.Length > 20)
            {
                ModelState.AddModelError("", "O nome de usuário deve ter entre 3 e 20 caracteres.");
                return View(model);
            }

            if (model.Email.Length < 5 || model.Email.Length > 50)
            {
                ModelState.AddModelError("", "O e-mail deve ter entre 5 e 50 caracteres.");
                return View(model);
            }

            var usuario = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed = true
            };

            var resultado = await _userManager.CreateAsync(usuario, model.Password);

            if (resultado.Succeeded)
            {
                await _userManager.AddToRoleAsync(usuario, "User");

            
                await _signInManager.SignInAsync(usuario, isPersistent: false);

               
                return RedirectToAction("Create", "EnderecoEntregas");
            }

            foreach (var erro in resultado.Errors)
            {
                ModelState.AddModelError("", erro.Description);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        
        public IActionResult EsqueciSenha()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EsqueciSenha(ResetSenhaViewModel model)
        {
           
            IdentityUser? usuario = await _userManager.FindByNameAsync(model.UserName) ??
                                     (model.UserName.Contains("@") ? await _userManager.FindByEmailAsync(model.UserName) : null);
            if (usuario == null)
            {
                ModelState.AddModelError("", "E-mail não encontrado.");
                return View(model);
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
            var callbackUrl = Url.Action("ResetSenha", "Account", new { userId = usuario.Id, token }, protocol: HttpContext.Request.Scheme);
            var corpo = $"Olá, recebemos uma solicitação de Redefinição de senha \n\n" + $"Click no link para redefinir a senha  {callbackUrl}";

            try
            {
                using (var client = new SmtpClient(_emailSetting.Host))
                {
                    client.Port = _emailSetting.Port;
                    client.EnableSsl = _emailSetting.EnableSsl;
                    client.Credentials = new System.Net.NetworkCredential(_emailSetting.UserName, _emailSetting.Password);
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_emailSetting.Remetente),
                        Subject = "Redefinição de Senha",
                        Body = corpo,
                        IsBodyHtml = false
                    };
                    mailMessage.To.Add(usuario.Email);
                    await client.SendMailAsync(mailMessage);
                }
            }



            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erro ao enviar o e-mail. Tente novamente mais tarde.");
                return View(model);

            }
            return RedirectToAction("ConfirmacaoEnvioEmail");
        }
        public IActionResult ConfirmacaoEnvioEmail()
        {
            return View();
        }
        public IActionResult ResetSenha(string userId, string token)
        {
            var viewModel = new ResetSenhaViewModel
            {
                UserId = userId,
                Token = token
            };
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> ResetSenha(ResetSenhaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Preencha todos os campos corretamente.");
                return View(model);
            }
            var usuario = await _userManager.FindByIdAsync(model.UserId);
            if (usuario == null)
            {
                ModelState.AddModelError("", "Usuário não encontrado.");
                return View(model);
            }
            var resultado = await _userManager.ResetPasswordAsync(usuario, model.Token, model.NovaSenha);
            if (resultado.Succeeded)
            {
                return RedirectToAction("Login");
            }
            foreach (var erro in resultado.Errors)
            {
                ModelState.AddModelError("", erro.Description);
            }
            return View(model);
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}