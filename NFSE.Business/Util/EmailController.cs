// using Funcoes.Email;
using NFSE.Business.Tabelas.DP;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace NFSE.Business.Util
{
    public class EmailController
    {
        public void Enviar(string mensagem, bool isDev)
        {
            //string Subject = "DP - " + (isDev ? "DESENVOLVIMENTO" : "PRODUÇÃO") + " - Serviço de Cadastro de Imagem de Nota Fiscal";

            //List<EmailDestination> emailDestinationList = new List<EmailDestination>();

            //var emailList = new UsuarioController().Selecionar(1).Email.Split(';');

            //foreach (var item in emailList)
            //{
            //    emailDestinationList.Add(new EmailDestination { Name = item, Email = item });
            //}

            //var email = new EmailConfiguration
            //{
            //    Configuration = new SmtpConfiguration("smtp.googlemail.com", 0),

            //    Sender = new EmailSender("Link-Pátios", "system@mob-link.net.br", "Studio55#"),

            //    Subject = Subject,

            //    DestinationList = emailDestinationList,

            //    Body = new StringBuilder(mensagem)
            //};

            //try
            //{
            //    Task.Run(() => Email.Send(email));
            //}
            //catch { }
        }

        //public static void Send(EmailConfiguration emailConfiguration)
        //{
        //    try
        //    {
        //        using (SmtpClient SmtpServer = new SmtpClient())
        //        {
        //            // CONFIGURAÇÕES DO GOOGLE
        //            SmtpServer.UseDefaultCredentials = false;

        //            SmtpServer.Credentials = new NetworkCredential(emailConfiguration.Sender.Email, emailConfiguration.Sender.Password);

        //            SmtpServer.Host = emailConfiguration.Configuration.Server;


        //            switch (emailConfiguration.Configuration.Server)
        //            {
        //                case "smtp.googlemail.com":
        //                case "smtp.mail.yahoo.com":

        //                    SmtpServer.Port = 587;

        //                    SmtpServer.EnableSsl = true;

        //                    break;

        //                default:

        //                    SmtpServer.Port = emailConfiguration.Configuration.Port;

        //                    SmtpServer.EnableSsl = emailConfiguration.Configuration.EnableSsl;

        //                    break;
        //            }


        //            if (emailConfiguration.Configuration.Timeout.Equals(0))
        //            {
        //                SmtpServer.Timeout = 180000; // 3 minutos
        //            }
        //            else
        //            {
        //                SmtpServer.Timeout = emailConfiguration.Configuration.Timeout;
        //            }


        //            using (MailMessage mailMessage = new MailMessage())
        //            {
        //                // FROM
        //                mailMessage.From = new MailAddress(emailConfiguration.Sender.Email, emailConfiguration.Sender.Name);

        //                //mailMessage.ReplyToList = ;

        //                for (int i = 0; i < emailConfiguration.DestinationList.Count; i++)
        //                {
        //                    mailMessage.To.Add(new MailAddress(emailConfiguration.DestinationList[i].Email, emailConfiguration.DestinationList[i].Name));
        //                }

        //                // SUBJECT
        //                mailMessage.SubjectEncoding = Encoding.GetEncoding("ISO-8859-1"); // Western European (ISO) AKA Windows-1252

        //                mailMessage.Subject = emailConfiguration.Subject;


        //                // BODY
        //                mailMessage.BodyEncoding = Encoding.GetEncoding("ISO-8859-1");

        //                mailMessage.IsBodyHtml = true;

        //                if (emailConfiguration.Body != null)
        //                {
        //                    mailMessage.Body = "<span style=\"font-family:Consolas;font-size: 10pt;\">" + emailConfiguration.Body.ToString().Replace(Environment.NewLine, "<br />") + "</span>";
        //                }


        //                // ATTACHMENT
        //                if (emailConfiguration.AttachmentFiles != null)
        //                {
        //                    for (int i = 0; i < emailConfiguration.AttachmentFiles.Count; i++)
        //                    {
        //                        mailMessage.Attachments.Add(new Attachment(emailConfiguration.AttachmentFiles[i].FullName, "text/plain"));
        //                    }
        //                }


        //                // SERVER CERTIFICATE VALIDATION
        //                if (!emailConfiguration.Configuration.ServerCertificateValidation)
        //                {
        //                    ServicePointManager.ServerCertificateValidationCallback = delegate (object s, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) { return true; };
        //                }

        //                SmtpServer.Send(mailMessage);
        //            }
        //        }
        //    }
        //    catch (SmtpException)
        //    {
        //        throw;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
    }
}
