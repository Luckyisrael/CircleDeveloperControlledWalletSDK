using System;
using System.Net.Http;
using System.Net.Http.Headers;
using CircleDeveloperControlledWalletSDK.Services;
using CircleDeveloperControlledWalletSDK.Utilities;

namespace CircleDeveloperControlledWalletSDK
{
    public class CircleClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private bool _disposed;

        public WalletSetService WalletSets { get; }
        public WalletService Wallets { get; }
        public EntitySecretService EntitySecrets { get; }
        public TransactionService Transactions { get; }
        public TokenService Tokens { get; }

        private const string BaseUrl = "https://api.circle.com/v1/w3s/";

        public CircleClient(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentNullException(nameof(apiKey), "API key cannot be null or empty.");

            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var cryptoUtils = new CryptoUtils(_httpClient);
            WalletSets = new WalletSetService(_httpClient);
            Wallets = new WalletService(_httpClient, cryptoUtils);
            EntitySecrets = new EntitySecretService(_httpClient, cryptoUtils);
            Transactions = new TransactionService(_httpClient, cryptoUtils);
            Tokens = new TokenService(_httpClient); 
        }

        public void Dispose()
        {
            if (_disposed) return;
            _httpClient?.Dispose();
            _disposed = true;
        }
    }
}