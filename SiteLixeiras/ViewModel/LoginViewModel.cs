using System.ComponentModel.DataAnnotations;

namespace SiteLixeiras.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Informe o e-mail ou nome de usuário")]
        [Display(Name = "Usuário ou E-mail")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Informe a senha")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
