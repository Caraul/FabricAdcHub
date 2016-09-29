using System;

namespace FabricAdcHub.Frontend.Models
{
    public class CustomUser
    {
        public CustomUser()
        {
            UserId = "user-" + Guid.NewGuid();
        }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string NormalizedUserName { get; set; }

        public string PasswordHash { get; set; }

        public string Email { get; set; }
    }
}
