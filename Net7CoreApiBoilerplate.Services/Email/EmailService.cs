using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Net7CoreApiBoilerplate.Infrastructure.Services;
using Net7CoreApiBoilerplate.Infrastructure.Settings;

namespace Net7CoreApiBoilerplate.Services.Email
{
    public interface IEmailService : IService
    {
        Task SendAsync(string subject, string body, string from, string to);
        Task<bool> Send(MailMessage message, IEnumerable<Attachment> attachments = null, List<string> bcc = null);

        Task SendEmailConfirmationAsync(string email, string callbackUrl);

        Task SendPasswordResetAsync(string email, string callbackUrl);

        Task SendException(Exception ex, string subject = null);
    }

    public class EmailService : IEmailService
    {
        private readonly IEmailSettings _emailSettings;
        private readonly IAppSettings _settings;

        public EmailService(IEmailSettings emailSettings, IAppSettings settings)
        {
            _emailSettings = emailSettings;
            _settings = settings;
        }

        public async Task SendAsync(string subject, string body, string from, string to)
        {
            using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
            using (var mailMessage = new MailMessage())
            {
                // client.EnableSsl = true; // Do this if you don't want https://stackoverflow.com/questions/30342884/5-7-57-smtp-client-was-not-authenticated-to-send-anonymous-mail-during-mail-fr

                if (!_emailSettings.DefaultCredentials)
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
                }

                PrepareMailMessage(subject, body, from, to, mailMessage);

                await client.SendMailAsync(mailMessage);
            }
        }

        public async Task<bool> Send(MailMessage message, IEnumerable<Attachment> attachments = null, List<string> bcc = null)
        {
            try
            {
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
                {
                    // To avoid: https://stackoverflow.com/questions/30342884/5-7-57-smtp-client-was-not-authenticated-to-send-anonymous-mail-during-mail-fr
                    smtp.EnableSsl = true;

                    if (_settings.IsDebug)
                    {
                        message.To.Clear();
                        message.CC.Clear();
                        message.Bcc.Clear();
                        message.To.Add(_settings.AdminEmail);
                    }
                    else
                    {
                        if (bcc != null && bcc.Any() && bcc.All(s => s != "nobcc@yourdomain.com"))
                        {
                            message.Bcc.Add(string.Join(",", bcc));
                        }
                        else if (bcc == null)
                        {
                            message.Bcc.Add("info@yourdomain.com");
                        }

                        message.Bcc.Add("admin@yourdomain.com");
                    }

                    if (attachments != null)
                    {
                        foreach (var attachment in attachments)
                        {
                            message.Attachments.Add(attachment);
                        }
                    }

                    smtp.Send(message);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task SendEmailConfirmationAsync(string emailAddress, string callbackUrl)
        {
            using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
            using (var mailMessage = new MailMessage())
            {
                if (!_emailSettings.DefaultCredentials)
                {
                    client.EnableSsl = true; // Do this if you don't want https://stackoverflow.com/questions/30342884/5-7-57-smtp-client-was-not-authenticated-to-send-anonymous-mail-during-mail-fr
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
                }

                PrepareMailMessage("Confirm your email", $"Please confirm your email by clicking here: <a href='{callbackUrl}'>link</a>", _emailSettings.EmailSourceAddress, emailAddress, mailMessage);

                await client.SendMailAsync(mailMessage);
            }
        }

        public async Task SendPasswordResetAsync(string emailAddress, string callbackUrl)
        {
            using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
            using (var mailMessage = new MailMessage())
            {
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;

                if (!_emailSettings.DefaultCredentials)
                {
                    client.EnableSsl = true; // Do this if you don't want https://stackoverflow.com/questions/30342884/5-7-57-smtp-client-was-not-authenticated-to-send-anonymous-mail-during-mail-fr
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
                }

                PrepareMailMessage("Reset your password", $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>", _emailSettings.EmailSourceAddress, emailAddress, mailMessage);

                await client.SendMailAsync(mailMessage);
            }
        }

        public async Task SendException(Exception ex, string subject = null)
        {
            using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
            using (var mailMessage = new MailMessage())
            {
                if (!_emailSettings.DefaultCredentials)
                {
                    client.EnableSsl = true; // Do this if you don't want https://stackoverflow.com/questions/30342884/5-7-57-smtp-client-was-not-authenticated-to-send-anonymous-mail-during-mail-fr
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
                }

                // Construct SUBJECT
                var constructedSubject = "SERVER ERROR";
                if (!string.IsNullOrEmpty(subject))
                {
                    constructedSubject += subject;
                }

                // Construct BODY
                var body = ex.Message;
                body += ex.StackTrace;
                if (ex.InnerException != null)
                {
                    body += "<br /><br />" + ex.InnerException;
                    body += "<br /><br />" + ex.InnerException.StackTrace;
                }

                PrepareMailMessage(constructedSubject, body, _emailSettings.EmailSourceAddress, _settings.AdminEmail, mailMessage);

                await client.SendMailAsync(mailMessage);
            }
        }

        private void PrepareMailMessage(string subject, string body, string from, string to, MailMessage mailMessage)
        {
            if (_settings.IsDebug)
            {
                mailMessage.To.Clear();
                mailMessage.CC.Clear();
                mailMessage.Bcc.Clear();
                mailMessage.To.Add(_settings.AdminEmail);
            }

            mailMessage.From = new MailAddress(from, to);
            mailMessage.To.Add(to);
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;
            mailMessage.Subject = subject;
        }
    }
}
