using SiteLixeiras.Models;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace SiteLixeiras.Sevices
{
    public class EmailService
    {
        private readonly EmailSetting _emailSetting;

        // Altere para receber IOptions<EmailSetting>
        public EmailService(IOptions<EmailSetting> emailSetting)
        {
            _emailSetting = emailSetting.Value;
        }

        public async Task EnviarEmail(string destinatario, string assunto, string mensagem)
        {
            using (var client = new SmtpClient(_emailSetting.Host, _emailSetting.Port))
            {
                client.EnableSsl = _emailSetting.EnableSsl;
                client.Credentials = new System.Net.NetworkCredential(_emailSetting.UserName, _emailSetting.Password);
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSetting.Remetente),
                    Subject = assunto,
                    Body = mensagem,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(destinatario);
                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
