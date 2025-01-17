using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XpressShip.Application.DTOs.Mail
{
    public record MessageDTO
    {
        public MailboxAddress? To { get; set; }
        public List<MailboxAddress> Recipients { get; set; } = [];
        public string Subject { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
