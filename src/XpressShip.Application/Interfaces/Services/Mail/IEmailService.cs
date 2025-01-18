using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Application.DTOs.Mail;
using XpressShip.Domain.Enums;

namespace XpressShip.Application.Interfaces.Services.Mail
{
    public interface IEmailService
    {
        Task SendEmailAsync(RecipientDetailsDTO recipientDetails, string subject, string body);
        Task SendEmailWithAttachmentsAsync(RecipientDetailsDTO recipientDetails, string subject, string body, IFormFileCollection attachments);
        Task SendBulkEmailAsync(IEnumerable<RecipientDetailsDTO> recipientDetails, string subject, string body);
    }
}
