﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RO.DevTest.Application.Contracts.Application.Service;

namespace RO.DevTest.Application.Services.SMTPEmail
{
    public class Send : ISend
    {
        private readonly IConfiguration _configuration;

        public Send(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Envia um e-mail com o link para recuperação de senha.
        /// </summary>
        /// <param name="recipientEmail">E-mail do destinatário.</param>
        /// <param name="recoveryLink">Link para a recuperação de senha.</param>
        /// <returns>True se enviado com sucesso, false caso contrário.</returns>
        public bool SendRecoveryEmail(string recipientEmail, string recoveryLink)
        {
            try
            {
                string host = _configuration.GetValue<string>("SMTP:Host")!;
                string displayName = _configuration.GetValue<string>("SMTP:Nome")!;
                string senderEmail = _configuration.GetValue<string>("SMTP:UserName")!;
                string senderPassword = _configuration.GetValue<string>("SMTP:Senha")!;
                int port = _configuration.GetValue<int>("SMTP:Porta");

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(senderEmail, displayName)
                };

                mail.To.Add(recipientEmail);
                mail.Subject = "Recuperação de Senha";
                mail.Body = $@"
                    <html>
                        <body>
                            <p>Olá,</p>
                            <p>Para redefinir sua senha, clique no link abaixo:</p>
                            <p><a href='{recoveryLink}'>Recuperar Senha</a></p>
                            <p>Se você não solicitou a recuperação, ignore este e-mail.</p>
                        </body>
                    </html>";
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                using (SmtpClient smtp = new SmtpClient(host, port))
                {
                    smtp.Credentials = new NetworkCredential(senderEmail, senderPassword);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }

                return true;
            }
            catch (Exception)
            {
                // Log detalhado para debug (em produção, opte por log sem detalhes sensíveis)
                return false;
            }
        }
    }
}

