using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using eCommerce.Domain.Models.Entities;
using eCommerce.Domain.Models.Interfaces.Repository.Email;

namespace eCommerce.Infrastructure.Repository
{
    public class EmailRepository : IEmailRepository
    {
        public void SendEmail(Usuario entity)
        {
            try
            {
                MailMessage mailMessage = new MailMessage("alexandretaumaturgo4@gmail.com", entity.Email);

                mailMessage.Subject = "Titulo email";
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = $"<p> Boaaaaa!!!!! </p> " +
                                   $"<br/>" +
                                   $"<hr/> " +
                                   $" <h1>Usuário {entity.Nome} criado com sucesso!</h1>";
                
                mailMessage.SubjectEncoding = Encoding.GetEncoding("UTF-8");
                mailMessage.BodyEncoding = Encoding.GetEncoding("UTF-8");

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);

                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("alexandretaumaturgo4@gmail.com", "");

                smtpClient.EnableSsl = true;

                smtpClient.Send(mailMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}