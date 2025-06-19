using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CircleDeveloperControlledWalletSDK.Exceptions;
using CircleDeveloperControlledWalletSDK.Models;
using CircleDeveloperControlledWalletSDK.Utilities;
using Newtonsoft.Json;

namespace CircleDeveloperControlledWalletSDK.Services
{
    public class EntitySecretService
    {
        private readonly HttpClient _httpClient;
        private readonly CryptoUtils _cryptoUtils;

        public EntitySecretService(HttpClient httpClient, CryptoUtils cryptoUtils)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _cryptoUtils = cryptoUtils ?? throw new ArgumentNullException(nameof(cryptoUtils));
        }

        public string GenerateEntitySecret()
        {
            return _cryptoUtils.GenerateEntitySecret();
        }

        public async Task<string> GenerateEntitySecretCiphertextAsync(string entitySecret)
        {
            return await _cryptoUtils.GenerateEntitySecretCiphertextAsync(entitySecret);
        }

        public async Task<RegisterEntitySecretResponse> RegisterEntitySecretAsync(string entitySecret, string recoveryFilePath = null)
        {
            if (string.IsNullOrEmpty(entitySecret))
                throw new ArgumentNullException(nameof(entitySecret));
            if (entitySecret.Length != 64 || !CryptoUtils.IsHexString(entitySecret))
                throw new ArgumentException("Entity Secret must be a 32-byte hex string (64 characters).", nameof(entitySecret));
            if (!string.IsNullOrEmpty(recoveryFilePath) && !CryptoUtils.IsValidFilePath(recoveryFilePath))
                throw new ArgumentException("Invalid recovery file path.", nameof(recoveryFilePath));

            var idempotencyKey = Guid.NewGuid().ToString();

            var request = new RegisterEntitySecretRequest
            {
                EntitySecret = entitySecret,
                IdempotencyKey = idempotencyKey
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "developer/register")
            {
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(requestMessage);
            await HandleResponseAsync(response);

            var content = await response.Content.ReadAsStringAsync();
            var registerResponse = JsonConvert.DeserializeObject<RegisterEntitySecretResponseWrapper>(content).Data;

            if (!string.IsNullOrEmpty(recoveryFilePath))
            {
                try
                {
                    var recoveryData = new
                    {
                        EntitySecret = entitySecret,
                        IdempotencyKey = idempotencyKey,
                        RegistrationDate = DateTime.UtcNow.ToString("o"),
                        Note = "Store this file securely and contact Circle Support for recovery."
                    };
                    await File.WriteAllTextAsync(recoveryFilePath, JsonConvert.SerializeObject(recoveryData, Formatting.Indented));
                }
                catch (Exception ex)
                {
                    throw new IOException($"Failed to write recovery file to {recoveryFilePath}.", ex);
                }
            }

            return registerResponse;
        }

        private async Task HandleResponseAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<ErrorResponse>(content) ?? new ErrorResponse { Message = "Unknown error" };
                throw new CircleApiException(error.Message, response.StatusCode);
            }
        }
    }
}