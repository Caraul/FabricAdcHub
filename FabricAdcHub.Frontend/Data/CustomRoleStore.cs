using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FabricAdcHub.Frontend.Models;
using Microsoft.AspNetCore.Identity;

namespace FabricAdcHub.Frontend.Data
{
    public class CustomRoleStore : IRoleStore<CustomRole>
    {
        private static readonly ConcurrentDictionary<string, CustomRole> IdsToRoles = new ConcurrentDictionary<string, CustomRole>();

        public Task<IdentityResult> CreateAsync(CustomRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(!IdsToRoles.TryAdd(role.RoleId, role) ? IdentityResult.Failed() : IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(CustomRole role, CancellationToken cancellationToken)
        {
            CustomRole customRole;
            return Task.FromResult(IdsToRoles.TryRemove(role.RoleId, out customRole) ? IdentityResult.Success : IdentityResult.Failed());
        }

        public Task<CustomRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            CustomRole role;
            return Task.FromResult(IdsToRoles.TryGetValue(roleId, out role) ? role : default(CustomRole));
        }

        public Task<CustomRole> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return Task.FromResult(
                IdsToRoles
                    .Where(pair => pair.Value.NormalizedRoleName == normalizedUserName)
                    .Select(pair => pair.Value)
                    .SingleOrDefault());
        }

        public Task<string> GetNormalizedRoleNameAsync(CustomRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedRoleName);
        }

        public Task<string> GetRoleIdAsync(CustomRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.RoleId);
        }

        public Task<string> GetRoleNameAsync(CustomRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.RoleName);
        }

        public Task SetNormalizedRoleNameAsync(CustomRole role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedRoleName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(CustomRole role, string roleName, CancellationToken cancellationToken)
        {
            role.RoleName = roleName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(CustomRole role, CancellationToken cancellationToken)
        {
            IdsToRoles.AddOrUpdate(role.RoleId, role, (_, __) => role);
            return Task.FromResult(IdentityResult.Success);
        }

        public void Dispose()
        {
        }
    }
}
