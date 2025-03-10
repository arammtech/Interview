using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Interview.Domain.Identity;

namespace Interview.Domain.Entities
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = null!;
        public int SmtpPort { get; set; }
        public string SenderEmail { get; set; } = null!;
        public string AppPassword { get; set; } = null!;
        public string? SenderName { get; set; }
        
    }
}
