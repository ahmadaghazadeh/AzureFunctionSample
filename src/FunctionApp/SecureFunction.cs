using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace FunctionApp
{
    public class SecureFunction
    {
        private readonly ILogger<SecureFunction> _logger;

        public SecureFunction(ILogger<SecureFunction> logger)
        {
            _logger = logger;
        }

        [Function("SecureFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            var authHeader = req.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return new UnauthorizedResult();
            }

            var token = authHeader.Substring("Bearer ".Length);

            var signingKey = await GetSigningKeyAsync("https://<YourKeyVaultName>.vault.azure.net/", "SigningKey");

            // Validate token
            var isValid = ValidateToken(token, "https://identityserver-url", "functionApi", signingKey);
            if (!isValid)
            {
                return new UnauthorizedResult();
            }

            return new OkObjectResult("Access granted.");
        }

        private static bool ValidateToken(string token, string issuer, string audience, string signingKey)
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
            };

            try
            {
                var handler = new JwtSecurityTokenHandler();
                handler.ValidateToken(token, validationParameters, out _);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static async Task<string> GetSigningKeyAsync(string keyVaultUrl, string secretName)
        {
            var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
            KeyVaultSecret secret = await client.GetSecretAsync(secretName);
            return secret.Value;
        }
    }
}