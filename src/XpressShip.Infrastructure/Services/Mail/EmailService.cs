using MailKit.Security;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using XpressShip.Application.Options;
using XpressShip.Domain.Enums;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using XpressShip.Application.DTOs.Mail;
using XpressShip.Application.Abstractions.Services.Mail;

namespace XpressShip.Infrastructure.Services.Mail
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailConfig;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailConfig = emailSettings.Value;
        }

        public async Task SendBulkEmailAsync(IEnumerable<RecipientDetailsDTO> recipientsDetails, string subject, string body)
        {
            var message = new MessageDTO
            {
                Recipients = recipientsDetails.Select(r => new MailboxAddress(r.Name, r.Email)).ToList(),
                Subject = subject,
                Content = body
            };

            var emailMessage = CreateEmailMessage(message);
            await SendAsync(emailMessage);
        }

        public async Task SendEmailAsync(RecipientDetailsDTO recipientDetails, string subject, string body)
        {
            var message = new MessageDTO
            {
                To = new MailboxAddress(recipientDetails.Name, recipientDetails.Email),
                Subject = subject,
                Content = body
            };

            var emailMessage = CreateEmailMessage(message);
            await SendAsync(emailMessage);
        }

        public async Task SendEmailWithAttachmentsAsync(RecipientDetailsDTO recipientDetails, string subject, string body, IFormFileCollection attachments)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailConfig.UserName, _emailConfig.From));
            message.To.Add(new MailboxAddress(recipientDetails.Name, recipientDetails.Email));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };

            foreach (var attachment in attachments)
            {
                using var stream = new MemoryStream();
                await attachment.CopyToAsync(stream);
                bodyBuilder.Attachments.Add(attachment.FileName, stream.ToArray(), ContentType.Parse(attachment.ContentType));
            }

            message.Body = bodyBuilder.ToMessageBody();
            await SendAsync(message);
        }

        private MimeMessage CreateEmailMessage(MessageDTO message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.UserName, _emailConfig.From));

            if (message.To != null)
            {
                emailMessage.To.Add(message.To);
            }
            else if (message.Recipients?.Count > 0)
            {
                emailMessage.To.AddRange(message.Recipients);
            }

            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message.Content };

            return emailMessage;
        }

        private async Task SendAsync(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();

            client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.StartTls);
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            client.Authenticate(_emailConfig.UserName, _emailConfig.Password);

            await client.SendAsync(mailMessage);

        }
    }

}
