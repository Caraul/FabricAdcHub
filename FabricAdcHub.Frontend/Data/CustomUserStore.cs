using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FabricAdcHub.Frontend.Models;
using Microsoft.AspNetCore.Identity;

namespace FabricAdcHub.Frontend.Data
{
    public class CustomUserStore : IUserPasswordStore<CustomUser>, IQueryableUserStore<CustomUser>
    {
        private static readonly ConcurrentDictionary<string, CustomUser> IdsToUsers = new ConcurrentDictionary<string, CustomUser>();

        public IQueryable<CustomUser> Users => IdsToUsers.Values.AsQueryable();

        public Task<IdentityResult> CreateAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!IdsToUsers.TryAdd(user.UserId, user) ? IdentityResult.Failed() : IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(CustomUser user, CancellationToken cancellationToken)
        {
            CustomUser customUser;
            return Task.FromResult(IdsToUsers.TryRemove(user.UserId, out customUser) ? IdentityResult.Success : IdentityResult.Failed());
        }

        public Task<CustomUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            CustomUser user;
            return Task.FromResult(IdsToUsers.TryGetValue(userId, out user) ? user : default(CustomUser));
        }

        public Task<CustomUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return Task.FromResult(
                IdsToUsers
                    .Where(pair => pair.Value.NormalizedUserName == normalizedUserName)
                    .Select(pair => pair.Value)
                    .SingleOrDefault());
        }

        public Task<string> GetNormalizedUserNameAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserId);
        }

        public Task<string> GetUserNameAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(CustomUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(CustomUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(CustomUser user, CancellationToken cancellationToken)
        {
            IdsToUsers.AddOrUpdate(user.UserId, user, (_, __) => user);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<string> GetPasswordHashAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetPasswordHashAsync(CustomUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}
