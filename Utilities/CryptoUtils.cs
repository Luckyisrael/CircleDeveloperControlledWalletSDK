using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CircleDeveloperControlledWalletSDK.Models;
using Newtonsoft.Json;

namespace CircleDeveloperControlledWalletSDK.Utilities
{
    public class CryptoUtils
    {
        private readonly HttpClient _httpClient;

        public CryptoUtils(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public string GenerateEntitySecret()
        {
            byte[] randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return BitConverter.ToString(randomBytes).Replace("-", "").ToLower();
        }

        public async Task<string> GenerateEntitySecretCiphertextAsync(string entitySecret)
        {
            if (string.IsNullOrEmpty(entitySecret))
                throw new ArgumentNullException(nameof(entitySecret));
            if (entitySecret.Length != 64 || !IsHexString(entitySecret))
                throw new ArgumentException("Entity Secret must be a 32-byte hex string (64 characters).", nameof(entitySecret));

            var response = await _httpClient.GetAsync("config/entity/publicKey");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var publicKeyResponse = JsonConvert.DeserializeObject<PublicKeyResponseWrapper>(content).Data;
            var publicKeyPem = publicKeyResponse.PublicKey;

            if (string.IsNullOrEmpty(publicKeyPem))
                throw new CryptographicException("Failed to retrieve Circle's public key.");

            byte[] entitySecretBytes;
            try
            {
                entitySecretBytes = Convert.FromHexString(entitySecret);
            }
            catch (FormatException)
            {
                throw new CryptographicException("Entity Secret must be a valid hex string.");
            }

            using var rsa = RSA.Create();
            try
            {
                rsa.ImportFromPem(publicKeyPem);
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Failed to import Circle's public key.", ex);
            }

            byte[] encryptedBytes;
            try
            {
                encryptedBytes = rsa.Encrypt(entitySecretBytes, RSAEncryptionPadding.OaepSHA256);
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Failed to encrypt Entity Secret.", ex);
            }

            return Convert.ToBase64String(encryptedBytes);
        }

        public static bool IsHexString(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            return value.All(c => "0123456789abcdefABCDEF".Contains(c));
        }

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