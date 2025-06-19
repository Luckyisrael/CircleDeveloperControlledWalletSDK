using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CircleDeveloperControlledWalletSDK.Models;
using CircleDeveloperControlledWalletSDK.Exceptions;

using Newtonsoft.Json;

namespace CircleDeveloperControlledWalletSDK.Utilities
{
    /// <summary>
    /// Provides cryptographic utility functions for entity secret generation, encryption, and validation.
    /// </summary>
    public class CryptoUtils
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="CryptoUtils"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client used for making API requests.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="httpClient"/> is null.</exception>
        public CryptoUtils(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }


        /// <summary>
        /// Generates a random 32-byte entity secret as a lowercase hexadecimal string.
        /// </summary>
        /// <returns>A 64-character lowercase hexadecimal string representing the entity secret.</returns>
        /// <exception cref="CryptographicException">Thrown when entity secret generation fails.</exception>
        public string GenerateEntitySecret()
        {
            try
            {
                byte[] randomBytes = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(randomBytes);
                }
                return Convert.ToHexString(randomBytes).ToLowerInvariant();
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Failed to generate Entity Secret.", ex);
            }
        }

        /// <summary>
        /// Generates a Base64-encoded Entity Secret ciphertext using Circle's public key.
        /// </summary>
        /// <param name="entitySecret">Unencrypted Entity Secret (64-character hex string).</param>
        /// <returns>A Base64-encoded ciphertext (684 characters).</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entitySecret"/> is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="entitySecret"/> is not a valid 32-byte hex string.</exception>
        /// <exception cref="CircleApiException">Thrown when the public key retrieval fails.</exception>
        /// <exception cref="CryptographicException">Thrown when RSA encryption fails.</exception>
        /// <remarks>
        /// Fetches Circle's RSA public key from GET /v1/w3s/config/entity/publicKey and encrypts the Entity Secret
        /// using RSA with OAEP-SHA256 padding. The resulting ciphertext is unique per request, as required by Circle.
        /// </remarks>
        public async Task<string> GenerateEntitySecretCiphertextAsync(string entitySecret)
        {
            if (string.IsNullOrEmpty(entitySecret))
                throw new ArgumentNullException(nameof(entitySecret));
            if (entitySecret.Length != 64 || !IsHexString(entitySecret))
                throw new ArgumentException("Entity Secret must be a 32-byte hex string (64 characters).", nameof(entitySecret));

            // Fetch Circle's public key
            var publicKeyResponse = await _httpClient.GetAsync("config/entity/publicKey");

            if (!publicKeyResponse.IsSuccessStatusCode)
            {
                var content = await publicKeyResponse.Content.ReadAsStringAsync();
                throw new CircleApiException(
                    $"Failed to fetch public key (Status {(int)publicKeyResponse.StatusCode}): {content}",
                        publicKeyResponse.StatusCode
                );
            }

            var publicKeyContent = await publicKeyResponse.Content.ReadAsStringAsync();
            var publicKeyWrapper = JsonConvert.DeserializeObject<PublicKeyResponseWrapper>(publicKeyContent);
            var publicKeyPem = publicKeyWrapper.Data.PublicKey;

            try
            {
                // Convert hex Entity Secret to bytes
                byte[] entitySecretBytes = Convert.FromHexString(entitySecret);
                for (int i = 0; i < entitySecret.Length; i += 2)
                {
                    entitySecretBytes[i / 2] = Convert.ToByte(entitySecret.Substring(i, 2), 16);
                }

                // Import RSA public key
                using var rsa = RSA.Create();
                rsa.ImportFromPem(publicKeyPem);

                // Encrypt with RSA-OAEP-SHA256
                byte[] encryptedBytes = rsa.Encrypt(entitySecretBytes, RSAEncryptionPadding.OaepSHA256);
                string ciphertext = Convert.ToBase64String(encryptedBytes);

                // Verify ciphertext length (should be 684 characters for 2048-bit RSA)
                if (ciphertext.Length != 684)
                    throw new CryptographicException($"Generated ciphertext length ({ciphertext.Length}) does not match expected length (684).");

                return ciphertext;
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Failed to encrypt Entity Secret.", ex);
            }
        }

        /// <summary>
        /// Determines whether a string contains only valid hexadecimal characters (0-9, a-f, A-F).
        /// </summary>
        /// <param name="value">The string to validate.</param>
        /// <returns>true if the string contains only valid hexadecimal characters; otherwise, false.</returns>
        public static bool IsHexString(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            foreach (char c in value)
            {
                if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F')))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Validates whether a given string represents a valid file path.
        /// </summary>
        /// <param name="path">The file path to validate.</param>
        /// <returns>true if the path is valid; otherwise, false.</returns>
        public static bool IsValidFilePath(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            try
            {
                var fullPath = Path.GetFullPath(path);
                var directory = Path.GetDirectoryName(fullPath);
                return string.IsNullOrEmpty(directory) || Directory.Exists(directory);
            }
            catch
            {
                return false;
            }
        }
    }
}