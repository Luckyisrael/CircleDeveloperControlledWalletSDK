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
    /// <summary>
    /// Service for managing entity secrets, including generation, encryption, and registration with Circle's API.
    /// </summary>
    public class EntitySecretService
    {
        private readonly HttpClient _httpClient;
        private readonly CryptoUtils _cryptoUtils;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntitySecretService"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client used for making API requests.</param>
        /// <param name="cryptoUtils">The cryptographic utilities for entity secret operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="httpClient"/> or <paramref name="cryptoUtils"/> is null.</exception>
        public EntitySecretService(HttpClient httpClient, CryptoUtils cryptoUtils)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _cryptoUtils = cryptoUtils ?? throw new ArgumentNullException(nameof(cryptoUtils));
        }

        /// <summary>
        /// Generates a new entity secret using cryptographic utilities.
        /// </summary>
        /// <returns>A 32-byte hex string representing the generated entity secret.</returns>
        public string GenerateEntitySecret()
        {
            return _cryptoUtils.GenerateEntitySecret();
        }

        /// <summary>
        /// Generates an encrypted ciphertext from the provided entity secret.
        /// </summary>
        /// <param name="entitySecret">The entity secret to encrypt.</param>
        /// <returns>A Task that represents the asynchronous operation. The task result contains the encrypted ciphertext as a string.</returns>
        public async Task<string> GenerateEntitySecretCiphertextAsync(string entitySecret)
        {
            return await _cryptoUtils.GenerateEntitySecretCiphertextAsync(entitySecret);
        }

        /// <summary>
        /// Registers an entity secret with Circle's API and optionally saves recovery information to a file.
        /// </summary>
        /// <param name="entitySecret">The entity secret to register, must be a 32-byte hex string.</param>
        /// <param name="recoveryFilePath">Optional. The file path where recovery information will be saved.</param>
        /// <returns>A Task that represents the asynchronous operation. The task result contains the registration response from Circle's API.</returns>
        /// <exception cref="ArgumentNullException">Thrown when entitySecret is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown when entitySecret is not a valid 32-byte hex string or when recoveryFilePath is invalid.</exception>
        /// <exception cref="IOException">Thrown when there is an error writing to the recovery file.</exception>
        /// <exception cref="CircleApiException">Thrown when the API request fails.</exception>
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

        private static async Task HandleResponseAsync(HttpResponseMessage response)
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