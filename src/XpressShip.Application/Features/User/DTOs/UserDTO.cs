using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpressShip.Domain.Entities.Users;

namespace XpressShip.Application.Features.User.DTOs
{
    public record UserDTO
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DeActivatedAt { get; set; }

        public UserDTO(ApplicationUser user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            UserName = user.UserName!;
            Email = user.Email!;
            Phone = user.PhoneNumber!;
            CreatedAt = user.CreatedAt;
            IsActive = user.IsActive;
            DeActivatedAt = user.DeActivatedAt;
        }
    }
}
