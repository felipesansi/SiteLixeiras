using System.ComponentModel.DataAnnotations;

namespace SiteLixeiras.ViewModel
{
    public class ResetSenhaViewModel
    {
        [EmailAddress(ErrorMessage = "Informe o e-mail ou nome de usuário")]
        [Display(Name = "Usuário ou E-mail")]
        public string Email { get; set; } = string.Empty; // Valor padrão para evitar nulos

        public string UserName { get; set; }

        [Required(ErrorMessage = "A nova senha é obrigatória.")]
        [StringLength(100, ErrorMessage = "A senha deve ter pelo menos {2} caracteres.", MinimumLength = 6)]
        public string NovaSenha { get; set; }

        [Required(ErrorMessage = "A confirmação de senha é obrigatória.")]
        [Compare("NovaSenha", ErrorMessage = "As senhas não coincidem.")]
        public string ConfirmarSenha { get; set; }

        public string UserId { get; set; }
        public string Token { get; set; }
    }
}
