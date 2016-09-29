using System;

namespace FabricAdcHub.Frontend.Models
{
    public class CustomRole
    {
        public CustomRole()
        {
            RoleId = "role-" + Guid.NewGuid();
        }

        public string RoleId { get; set; }

        public string RoleName { get; set; }

        public string NormalizedRoleName { get; set; }
    }
}
