using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain.Abstractions;

namespace XpressShip.Domain.Entities.Identity
{
    public class ApplicationRole : IdentityRole, IBase
    {
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        protected ApplicationRole() { }
        private ApplicationRole(string name, string? description)
        {
            Name = name;
            Description = description;
        }

        public static ApplicationRole Create(string name, string? description)
        {
            return new ApplicationRole(name, description);
        }
    }
}
