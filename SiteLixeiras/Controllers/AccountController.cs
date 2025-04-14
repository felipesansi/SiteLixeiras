using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SiteLixeiras.ViewModel;

namespace SiteLixeiras.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Exibe a tela de login
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
                return RedirectToAction("Login");
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

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}