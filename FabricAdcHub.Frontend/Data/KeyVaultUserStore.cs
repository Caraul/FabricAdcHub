using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FabricAdcHub.Frontend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.KeyVault;
using Newtonsoft.Json;

namespace FabricAdcHub.Frontend.Data
{
    public class KeyVaultUserStore : IUserPasswordStore<CustomUser>, IQueryableUserStore<CustomUser>
    {
        private readonly Uri _keyVaultUri;
        private readonly KeyVaultClient _client;

        public KeyVaultUserStore(Uri keyVaultUri)
        {
            _keyVaultUri = keyVaultUri;
            _client = new KeyVaultClient(AuthenticationCallback);
        }

        public IQueryable<CustomUser> Users
        {
            get
            {
                var secrets = _client.GetSecretsAsync(_keyVaultUri.ToString()).Result;
                var users = secrets.Value
                    .Where(secret => secret.Identifier.Name.StartsWith("user"))
                    .Select(secret => _client.GetSecretAsync(secret.Identifier.Identifier).Result.Value)
                    .Select(ConvertFromSecret)
                    .ToList();

                while (!string.IsNullOrEmpty(secrets.NextLink))
                {
                    secrets = _client.GetSecretsNextAsync(secrets.NextLink).Result;
                    users.AddRange(secrets.Value
                        .Where(secret => secret.Identifier.Name.StartsWith("user"))
                        .Select(secret => _client.GetSecretAsync(secret.Identifier.Identifier).Result.Value)
                        .Select(ConvertFromSecret));
                }

                return users.AsQueryable();
            }
        }

        public async Task<IdentityResult> CreateAsync(CustomUser user, CancellationToken cancellationToken)
        {
            await _client.SetSecretAsync(_keyVaultUri.ToString(), user.UserId, ConvertToSecret(user), cancellationToken: cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(CustomUser user, CancellationToken cancellationToken)
        {
            await _client.DeleteSecretAsync(_keyVaultUri.ToString(), user.UserId, cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<CustomUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var secret = await _client.GetSecretAsync(_keyVaultUri.ToString(), userId, cancellationToken: cancellationToken);
            var user = ConvertFromSecret(secret.Value);
            return user;
        }

        public Task<CustomUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return Task.FromResult(Users.SingleOrDefault(user => user.NormalizedUserName == normalizedUserName));
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

        public async Task<IdentityResult> UpdateAsync(CustomUser user, CancellationToken cancellationToken)
        {
            await _client.SetSecretAsync(_keyVaultUri.ToString(), user.UserId, ConvertToSecret(user), cancellationToken: cancellationToken);
            return IdentityResult.Success;
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

        private static Task<string> AuthenticationCallback(string authority, string resource, string scope)
        {
            return Task.FromResult(string.Empty);
        }

        private static string ConvertToSecret(CustomUser user)
        {
            return JsonConvert.SerializeObject(user);
        }

        private static CustomUser ConvertFromSecret(string json)
        {
            return JsonConvert.DeserializeObject<CustomUser>(json);
        }
    }
}
