using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FabricAdcHub.Frontend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.KeyVault;
using Newtonsoft.Json;

namespace FabricAdcHub.Frontend.Data
{
    public class KeyVaultRoleStore : IRoleStore<CustomRole>
    {
        private readonly Uri _keyVaultUri;
        private readonly KeyVaultClient _client;

        public KeyVaultRoleStore(Uri keyVaultUri)
        {
            _keyVaultUri = keyVaultUri;
            _client = new KeyVaultClient(AuthenticationCallback);
        }

        public async Task<IdentityResult> CreateAsync(CustomRole role, CancellationToken cancellationToken)
        {
            await _client.SetSecretAsync(_keyVaultUri.ToString(), role.RoleId, ConvertToSecret(role), cancellationToken: cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(CustomRole role, CancellationToken cancellationToken)
        {
            await _client.DeleteSecretAsync(_keyVaultUri.ToString(), role.RoleId, cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<CustomRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            var secret = await _client.GetSecretAsync(_keyVaultUri.ToString(), roleId, cancellationToken: cancellationToken);
            var role = ConvertFromSecret(secret.Value);
            return role;
        }

        public Task<CustomRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return Task.FromResult(GetAllRoles().SingleOrDefault(role => role.NormalizedRoleName == normalizedRoleName));
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

        public async Task<IdentityResult> UpdateAsync(CustomRole role, CancellationToken cancellationToken)
        {
            await _client.SetSecretAsync(_keyVaultUri.ToString(), role.RoleId, ConvertToSecret(role), cancellationToken: cancellationToken);
            return IdentityResult.Success;
        }

        public void Dispose()
        {
        }

        private static Task<string> AuthenticationCallback(string authority, string resource, string scope)
        {
            return Task.FromResult(string.Empty);
        }

        public List<CustomRole> GetAllRoles()
        {
            var secrets = _client.GetSecretsAsync(_keyVaultUri.ToString()).Result;
            var roles = secrets.Value
                .Where(secret => secret.Identifier.Name.StartsWith("role"))
                .Select(secret => _client.GetSecretAsync(secret.Identifier.Identifier).Result.Value)
                .Select(ConvertFromSecret)
                .ToList();

            while (!string.IsNullOrEmpty(secrets.NextLink))
            {
                secrets = _client.GetSecretsNextAsync(secrets.NextLink).Result;
                roles.AddRange(secrets.Value
                    .Where(secret => secret.Identifier.Name.StartsWith("role"))
                    .Select(secret => _client.GetSecretAsync(secret.Identifier.Identifier).Result.Value)
                    .Select(ConvertFromSecret));
            }

            return roles;
        }

        private static string ConvertToSecret(CustomRole role)
        {
            return JsonConvert.SerializeObject(role);
        }

        private static CustomRole ConvertFromSecret(string json)
        {
            return JsonConvert.DeserializeObject<CustomRole>(json);
        }
    }
}
