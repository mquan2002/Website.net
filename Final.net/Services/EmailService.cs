using System.Net.Mail;
using MailKit.Net.Smtp;
using MimeKit;
using Final.net.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Final.net.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail(string recipientEmail, string subject, string messageBody)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("Pizza Store", _configuration["EmailSettings:SenderEmail"]));
                email.To.Add(new MailboxAddress("", recipientEmail));
                email.Subject = subject;

                email.Body = new TextPart("html")
                {
                    Text = messageBody
                };

                using (var smtp = new SmtpClient())
                {
                    smtp.Connect(_configuration["EmailSettings:SmtpServer"], int.Parse(_configuration["EmailSettings:Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                    smtp.Authenticate(_configuration["EmailSettings:SenderEmail"], _configuration["EmailSettings:SenderPassword"]);
                    smtp.Send(email);
                    smtp.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }
    }

}
