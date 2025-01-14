using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Routing.Template;
using System.Net.Mail;
using System.Net.Sockets;

namespace emaildisparator.Service.Email
{
    public class EnviarEmail : IEmailSender
    {

        public async Task EnviarEmailslAsync(string email, string nome)
        {
            var fromAddress = "higortechnology@gmail.com";
            var smtpServer = "smtp.gmail.com";
            var smtpPort = 587;
            var appPassword = "sqwm ytfo esmf oikl";

            var templatePath = "Service/Email/EmailTemplate.html";
            var emailTemplate = await File.ReadAllTextAsync(templatePath);

            var custumizedTamplate = emailTemplate.Replace("{UsauarioNome}", nome);

            var message = new MailMessage
            {
                From = new MailAddress(fromAddress),
                Subject = "Bem-vindo a nossa plataforma",
                Body = custumizedTamplate,
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(email));

            using var cliente = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new System.Net.NetworkCredential(fromAddress, appPassword),
                EnableSsl = true 
            };

            try
            {
                await cliente.SendMailAsync(message);
            }
            catch (SocketException ex)
            {
                // Handle socket exception
                throw new SmtpException("Failed to connect to the SMTP server. Please verify your SMTP settings.", ex);
            }
            catch (SmtpException ex)
            {
                // Handle SMTP-specific exceptions
                throw new SmtpException("Failure sending mail.", ex);
            }
            catch (Exception ex)
            {
                // General exception handling
                throw new Exception("An error occurred while sending the email.", ex);
            }
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            throw new NotImplementedException();
        }
    }
}
