using System.Text;
using CircleDeveloperControlledWalletSDK.Exceptions;
using CircleDeveloperControlledWalletSDK.Models;
using CircleDeveloperControlledWalletSDK.Utilities;
using Newtonsoft.Json;

namespace CircleDeveloperControlledWalletSDK.Services
{
    /// <summary>
    /// Provides functionality for managing developer-controlled wallets through the Circle API.
    /// This service handles wallet creation, retrieval, and updates.
    /// </summary>
    public class WalletService
    {
        private readonly HttpClient _httpClient;
        private readonly CryptoUtils _cryptoUtils;

        /// <summary>
        /// Initializes a new instance of the <see cref="WalletService"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client used for making API requests.</param>
        /// <param name="cryptoUtils">The crypto utilities for handling encryption operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when httpClient or cryptoUtils is null.</exception>
        public WalletService(HttpClient httpClient, CryptoUtils cryptoUtils)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _cryptoUtils = cryptoUtils ?? throw new ArgumentNullException(nameof(cryptoUtils));
        }

        /// <summary>
        /// Creates one or more developer-controlled wallets within a wallet set.
        /// </summary>
        /// <param name="walletSetId">The ID of the wallet set.</param>
        /// <param name="blockchains">List of blockchains for the wallets.</param>
        /// <param name="entitySecret">Unencrypted Entity Secret for ciphertext generation.</param>
        /// <param name="idempotencyKey">Optional UUIDv4 idempotency key; defaults to a new GUID.</param>
        /// <param name="accountType">Account type (EOA or SCA); defaults to EOA.</param>
        /// <param name="count">Number of wallets to create per blockchain; defaults to 1.</param>
        /// <param name="metadata">Metadata for each wallet; count must match if provided.</param>
        /// <param name="xRequestId">Optional request identifier for Circle Support.</param>
        /// <returns>A <see cref="WalletsResponse"/> containing the created wallets.</returns>
        /// <exception cref="ArgumentException">Thrown when parameters are invalid.</exception>
        /// <exception cref="CircleApiException">Thrown when the API request fails.</exception>
        public async Task<WalletsResponse> CreateWalletAsync(
            string walletSetId,
            IEnumerable<string> blockchains,
            string entitySecret,
            string idempotencyKey = null,
            string accountType = "EOA",
            int count = 1,
            IEnumerable<WalletMetadata> metadata = null,
            string xRequestId = null)
        {
            if (string.IsNullOrEmpty(walletSetId))
                throw new ArgumentException("Wallet set ID cannot be null or empty.", nameof(walletSetId));
            if (blockchains == null || !blockchains.Any())
                throw new ArgumentException("At least one blockchain must be specified.", nameof(blockchains));
            if (string.IsNullOrEmpty(entitySecret) || entitySecret.Length != 64 || !CryptoUtils.IsHexString(entitySecret))
                throw new ArgumentException("Entity Secret must be a 32-byte hex string (64 characters).", nameof(entitySecret));
            if (count < 1)
                throw new ArgumentException("Count must be at least 1.", nameof(count));
            if (!new[] { "EOA", "SCA" }.Contains(accountType))
                throw new ArgumentException("Account type must be 'EOA' or 'SCA'.", nameof(accountType));
            if (metadata != null && metadata.Count() != count)
                throw new ArgumentException("Metadata count must match the specified count.", nameof(metadata));

            var validBlockchains = new HashSet<string>
            {
                "ETH", "ETH-SEPOLIA", "AVAX", "AVAX-FUJI", "MATIC", "MATIC-AMOY",
                "SOL", "SOL-DEVNET", "ARB", "ARB-SEPOLIA", "NEAR", "NEAR-TESTNET",
                "EVM", "EVM-TESTNET", "UNI", "UNI-SEPOLIA", "BASE", "BASE-SEPOLIA",
                "OP", "OP-SEPOLIA"
            };
            if (blockchains.Any(b => !validBlockchains.Contains(b)))
                throw new ArgumentException("Invalid blockchain specified.", nameof(blockchains));

            idempotencyKey ??= Guid.NewGuid().ToString();
            var entitySecretCiphertext = await _cryptoUtils.GenerateEntitySecretCiphertextAsync(entitySecret);

            var request = new CreateWalletRequest
            {
                WalletSetId = walletSetId,
                Blockchains = blockchains.ToList(),
                EntitySecretCiphertext = entitySecretCiphertext,
                IdempotencyKey = idempotencyKey,
                AccountType = accountType,
                Count = count,
                Metadata = metadata?.ToList()
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "developer/wallets")
            {
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
            };
            if (!string.IsNullOrEmpty(xRequestId))
                requestMessage.Headers.Add("X-Request-Id", xRequestId);

            var response = await _httpClient.SendAsync(requestMessage);
            await HandleResponseAsync(response);

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<WalletsResponseWrapper>(content).Data;
        }

        /// <summary>
        /// Retrieves a list of wallets based on specified filters.
        /// </summary>
        /// <param name="address">Optional blockchain address filter.</param>
        /// <param name="blockchain">Optional blockchain filter.</param>
        /// <param name="scaCore">Optional SCA version filter.</param>
        /// <param name="walletSetId">Optional wallet set ID filter.</param>
        /// <param name="refId">Optional reference ID filter.</param>
        /// <param name="from">Optional start date for creation filter.</param>
        /// <param name="to">Optional end date for creation filter.</param>
        /// <param name="pageBefore">Optional pagination cursor for previous page.</param>
        /// <param name="pageAfter">Optional pagination cursor for next page.</param>
        /// <param name="pageSize">Optional number of items per page (1-50).</param>
        /// <returns>A <see cref="WalletsResponse"/> containing the wallets.</returns>
        /// <exception cref="ArgumentException">Thrown when pagination parameters are invalid.</exception>
        /// <exception cref="CircleApiException">Thrown when the API request fails.</exception>
        public async Task<WalletsResponse> GetAllWalletsAsync(
            string address = null,
            string blockchain = null,
            string scaCore = null,
            string walletSetId = null,
            string refId = null,
            DateTime? from = null,
            DateTime? to = null,
            string pageBefore = null,
            string pageAfter = null,
            int? pageSize = null)
        {
            if (!string.IsNullOrEmpty(pageBefore) && !string.IsNullOrEmpty(pageAfter))
                throw new ArgumentException("Cannot specify both pageBefore and pageAfter.");
            if (pageSize.HasValue && (pageSize < 1 || pageSize > 50))
                throw new ArgumentException("Page size must be between 1 and 50.", nameof(pageSize));

            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(address)) queryParams.Add($"address={Uri.EscapeDataString(address)}");
            if (!string.IsNullOrEmpty(blockchain)) queryParams.Add($"blockchain={Uri.EscapeDataString(blockchain)}");
            if (!string.IsNullOrEmpty(scaCore)) queryParams.Add($"scaCore={Uri.EscapeDataString(scaCore)}");
            if (!string.IsNullOrEmpty(walletSetId)) queryParams.Add($"walletSetId={Uri.EscapeDataString(walletSetId)}");
            if (!string.IsNullOrEmpty(refId)) queryParams.Add($"refId={Uri.EscapeDataString(refId)}");
            if (from.HasValue) queryParams.Add($"from={Uri.EscapeDataString(from.Value.ToString("o"))}");
            if (to.HasValue) queryParams.Add($"to={Uri.EscapeDataString(to.Value.ToString("o"))}");
            if (!string.IsNullOrEmpty(pageBefore)) queryParams.Add($"pageBefore={Uri.EscapeDataString(pageBefore)}");
            if (!string.IsNullOrEmpty(pageAfter)) queryParams.Add($"pageAfter={Uri.EscapeDataString(pageAfter)}");
            if (pageSize.HasValue) queryParams.Add($"pageSize={pageSize.Value}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"wallets{queryString}");

            var response = await _httpClient.SendAsync(requestMessage);
            await HandleResponseAsync(response);

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<WalletsResponseWrapper>(content).Data;
        }

        /// <summary>
        /// Retrieves a specific wallet by its ID.
        /// </summary>
        /// <param name="id">The wallet ID.</param>
        /// <param name="xRequestId">Optional request identifier for Circle Support.</param>
        /// <returns>A <see cref="WalletResponse"/> containing the wallet details.</returns>
        /// <exception cref="ArgumentException">Thrown when ID is invalid.</exception>
        /// <exception cref="CircleApiException">Thrown when the API request fails.</exception>
        public async Task<WalletResponse> GetWalletAsync(string id, string xRequestId = null)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Wallet ID cannot be null or empty.", nameof(id));

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"wallets/{Uri.EscapeDataString(id)}");
            if (!string.IsNullOrEmpty(xRequestId))
                requestMessage.Headers.Add("X-Request-Id", xRequestId);

            var response = await _httpClient.SendAsync(requestMessage);
            await HandleResponseAsync(response);

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<WalletResponseWrapper>(content).Data;
        }

        /// <summary>
        /// Updates the metadata of a specific wallet.
        /// </summary>
        /// <param name="id">The wallet ID.</param>
        /// <param name="name">Optional new name for the wallet.</param>
        /// <param name="refId">Optional new reference ID for the wallet.</param>
        /// <param name="xRequestId">Optional request identifier for Circle Support.</param>
        /// <returns>A <see cref="WalletResponse"/> containing the updated wallet details.</returns>
        /// <exception cref="ArgumentException">Thrown when ID is invalid or no updates provided.</exception>
        /// <exception cref="CircleApiException">Thrown when the API request fails.</exception>
        public async Task<WalletResponse> UpdateWalletAsync(string id, string name = null, string refId = null, string xRequestId = null)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Wallet ID cannot be null or empty.", nameof(id));
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(refId))
                throw new ArgumentException("At least one of name or refId must be provided.", nameof(name));

            var request = new UpdateWalletRequest
            {
                Name = name,
                RefId = refId
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"wallets/{Uri.EscapeDataString(id)}")
            {
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
            };
            if (!string.IsNullOrEmpty(xRequestId))
                requestMessage.Headers.Add("X-Request-Id", xRequestId);

            var response = await _httpClient.SendAsync(requestMessage);
            await HandleResponseAsync(response);

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<WalletResponseWrapper>(content).Data;
        }

         /// <summary>
        /// Derives an EOA or SCA wallet on a specified blockchain for an existing wallet.
        /// </summary>
        /// <param name="id">The wallet ID.</param>
        /// <param name="blockchain">The target blockchain.</param>
        /// <param name="metadata">Optional metadata (name, refId) for the derived wallet.</param>
        /// <param name="xRequestId">Optional request identifier for Circle Support.</param>
        /// <returns>A <see cref="WalletResponse"/> containing the derived wallet details.</returns>
        /// <exception cref="ArgumentException">Thrown when parameters are invalid.</exception>
        /// <exception cref="CircleApiException">Thrown when the API request fails.</exception>
        public async Task<WalletResponse> DeriveWalletAsync(string id, string blockchain, WalletMetadata metadata = null, string xRequestId = null)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Wallet ID cannot be null or empty.", nameof(id));
            if (string.IsNullOrEmpty(blockchain))
                throw new ArgumentException("Blockchain cannot be null or empty.", nameof(blockchain));

            var validBlockchains = new HashSet<string>
            {
                "ETH", "ETH-SEPOLIA", "AVAX", "AVAX-FUJI", "MATIC", "MATIC-AMOY",
                "ARB", "ARB-SEPOLIA", "EVM", "EVM-TESTNET", "UNI", "UNI-SEPOLIA",
                "BASE", "BASE-SEPOLIA", "OP", "OP-SEPOLIA"
            };
            if (!validBlockchains.Contains(blockchain))
                throw new ArgumentException("Invalid blockchain specified.", nameof(blockchain));

            var request = new DeriveWalletRequest
            {
                Metadata = metadata
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"developer/wallets/{Uri.EscapeDataString(id)}/blockchains/{Uri.EscapeDataString(blockchain)}")
            {
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
            };
            if (!string.IsNullOrEmpty(xRequestId))
                requestMessage.Headers.Add("X-Request-Id", xRequestId);

            var response = await _httpClient.SendAsync(requestMessage);
            await HandleResponseAsync(response);

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<WalletResponseWrapper>(content).Data;
        }

        
        /// <summary>
        /// Retrieves a list of wallets with their token balances based on specified filters.
        /// </summary>
        /// <param name="blockchain">Required blockchain filter.</param>
        /// <param name="address">Optional blockchain address filter.</param>
        /// <param name="scaCore">Optional SCA version filter.</param>
        /// <param name="walletSetId">Optional wallet set ID filter.</param>
        /// <param name="refId">Optional reference ID filter.</param>
        /// <param name="amountGte">Optional minimum balance filter.</param>
        /// <param name="tokenAddress">Optional token address filter.</param>
        /// <param name="from">Optional start date for creation filter.</param>
        /// <param name="to">Optional end date for creation filter.</param>
        /// <param name="pageBefore">Optional pagination cursor for previous page.</param>
        /// <param name="pageAfter">Optional pagination cursor for next page.</param>
        /// <param name="pageSize">Optional number of items per page (1-50).</param>
        /// <param name="xRequestId">Optional request identifier for Circle Support.</param>
        /// <returns>A <see cref="WalletsWithBalancesResponse"/> containing wallets and their balances.</returns>
        /// <exception cref="ArgumentException">Thrown when parameters are invalid.</exception>
        /// <exception cref="CircleApiException">Thrown when the API request fails.</exception>
        public async Task<WalletsWithBalancesResponse> GetWalletBalancesAsync(
            string blockchain,
            string address = null,
            string scaCore = null,
            string walletSetId = null,
            string refId = null,
            string amountGte = null,
            string tokenAddress = null,
            DateTime? from = null,
            DateTime? to = null,
            string pageBefore = null,
            string pageAfter = null,
            int? pageSize = null,
            string xRequestId = null)
        {
            if (string.IsNullOrEmpty(blockchain))
                throw new ArgumentException("Blockchain cannot be null or empty.", nameof(blockchain));
            if (!string.IsNullOrEmpty(pageBefore) && !string.IsNullOrEmpty(pageAfter))
                throw new ArgumentException("Cannot specify both pageBefore and pageAfter.");
            if (pageSize.HasValue && (pageSize < 1 || pageSize > 50))
                throw new ArgumentException("Page size must be between 1 and 50.", nameof(pageSize));

            var queryParams = new List<string> { $"blockchain={Uri.EscapeDataString(blockchain)}" };
            if (!string.IsNullOrEmpty(address)) queryParams.Add($"address={Uri.EscapeDataString(address)}");
            if (!string.IsNullOrEmpty(scaCore)) queryParams.Add($"scaCore={Uri.EscapeDataString(scaCore)}");
            if (!string.IsNullOrEmpty(walletSetId)) queryParams.Add($"walletSetId={Uri.EscapeDataString(walletSetId)}");
            if (!string.IsNullOrEmpty(refId)) queryParams.Add($"refId={Uri.EscapeDataString(refId)}");
            if (!string.IsNullOrEmpty(amountGte)) queryParams.Add($"amount__gte={Uri.EscapeDataString(amountGte)}");
            if (!string.IsNullOrEmpty(tokenAddress)) queryParams.Add($"tokenAddress={Uri.EscapeDataString(tokenAddress)}");
            if (from.HasValue) queryParams.Add($"from={Uri.EscapeDataString(from.Value.ToString("o"))}");
            if (to.HasValue) queryParams.Add($"to={Uri.EscapeDataString(to.Value.ToString("o"))}");
            if (!string.IsNullOrEmpty(pageBefore)) queryParams.Add($"pageBefore={Uri.EscapeDataString(pageBefore)}");
            if (!string.IsNullOrEmpty(pageAfter)) queryParams.Add($"pageAfter={Uri.EscapeDataString(pageAfter)}");
            if (pageSize.HasValue) queryParams.Add($"pageSize={pageSize.Value}");

            var queryString = "?" + string.Join("&", queryParams);
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"developer/wallets/balances{queryString}");
            if (!string.IsNullOrEmpty(xRequestId))
                requestMessage.Headers.Add("X-Request-Id", xRequestId);

            var response = await _httpClient.SendAsync(requestMessage);
            await HandleResponseAsync(response);

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<WalletsWithBalancesResponseWrapper>(content).Data;
        }

        /// <summary>
        /// Retrieves NFTs stored in a specific wallet.
        /// </summary>
        /// <param name="id">The wallet ID.</param>
        /// <param name="includeAll">Optional flag to include monitored and non-monitored tokens.</param>
        /// <param name="name">Optional token name filter.</param>
        /// <param name="tokenAddress">Optional token address filter.</param>
        /// <param name="standard">Optional token standard filter.</param>
        /// <param name="pageBefore">Optional pagination cursor for previous page.</param>
        /// <param name="pageAfter">Optional pagination cursor for next page.</param>
        /// <param name="pageSize">Optional number of items per page (1-50).</param>
        /// <param name="xRequestId">Optional request identifier for Circle Support.</param>
        /// <returns>A <see cref="NftsResponse"/> containing the NFTs.</returns>
        /// <exception cref="ArgumentException">Thrown when parameters are invalid.</exception>
        /// <exception cref="CircleApiException">Thrown when the API request fails.</exception>
        public async Task<NftsResponse> GetWalletNftsAsync(
            string id,
            bool? includeAll = null,
            string name = null,
            string tokenAddress = null,
            string standard = null,
            string pageBefore = null,
            string pageAfter = null,
            int? pageSize = null,
            string xRequestId = null)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Wallet ID cannot be null or empty.", nameof(id));
            if (!string.IsNullOrEmpty(pageBefore) && !string.IsNullOrEmpty(pageAfter))
                throw new ArgumentException("Cannot specify both pageBefore and pageAfter.");
            if (pageSize.HasValue && (pageSize < 1 || pageSize > 50))
                throw new ArgumentException("Page size must be between 1 and 50.", nameof(pageSize));

            var queryParams = new List<string>();
            if (includeAll.HasValue) queryParams.Add($"includeAll={includeAll.Value.ToString().ToLower()}");
            if (!string.IsNullOrEmpty(name)) queryParams.Add($"name={Uri.EscapeDataString(name)}");
            if (!string.IsNullOrEmpty(tokenAddress)) queryParams.Add($"tokenAddress={Uri.EscapeDataString(tokenAddress)}");
            if (!string.IsNullOrEmpty(standard)) queryParams.Add($"standard={Uri.EscapeDataString(standard)}");
            if (!string.IsNullOrEmpty(pageBefore)) queryParams.Add($"pageBefore={Uri.EscapeDataString(pageBefore)}");
            if (!string.IsNullOrEmpty(pageAfter)) queryParams.Add($"pageAfter={Uri.EscapeDataString(pageAfter)}");
            if (pageSize.HasValue) queryParams.Add($"pageSize={pageSize.Value}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"wallets/{Uri.EscapeDataString(id)}/nfts{queryString}");
            if (!string.IsNullOrEmpty(xRequestId))
                requestMessage.Headers.Add("X-Request-Id", xRequestId);

            var response = await _httpClient.SendAsync(requestMessage);
            await HandleResponseAsync(response);

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<NftsResponseWrapper>(content).Data;
        }

        /// <summary>
        /// Signs a message from a specified developer-controlled wallet (EIP-191 for Ethereum, Ed25519 for Solana).
        /// </summary>
        /// <param name="walletId">The wallet ID.</param>
        /// <param name="message">The message to sign (hex string if encodedByHex is true).</param>
        /// <param name="entitySecret">Unencrypted Entity Secret for ciphertext generation.</param>
        /// <param name="encodedByHex">Whether the message is hex-encoded (starts with 0x).</param>
        /// <param name="memo">Optional human-readable explanation for the sign action.</param>
        /// <param name="xRequestId">Optional request identifier for Circle Support.</param>
        /// <returns>A <see cref="SignMessageResponse"/> containing the signature.</returns>
        /// <exception cref="ArgumentException">Thrown when parameters are invalid.</exception>
        /// <exception cref="CircleApiException">Thrown when the API request fails.</exception>
        public async Task<SignMessageResponse> SignMessageAsync(
            string walletId,
            string message,
            string entitySecret,
            bool encodedByHex = false,
            string memo = null,
            string xRequestId = null)
        {
            if (string.IsNullOrEmpty(walletId))
                throw new ArgumentException("Wallet ID cannot be null or empty.", nameof(walletId));
            if (string.IsNullOrEmpty(message))
                throw new ArgumentException("Message cannot be null or empty.", nameof(message));
            if (string.IsNullOrEmpty(entitySecret) || entitySecret.Length != 64 || !CryptoUtils.IsHexString(entitySecret))
                throw new ArgumentException("Entity Secret must be a 32-byte hex string (64 characters).", nameof(entitySecret));
            if (encodedByHex && (!message.StartsWith("0x") || message.Length % 2 != 0 || !CryptoUtils.IsHexString(message.Substring(2))))
                throw new ArgumentException("Message must be a valid hex string starting with '0x' when encodedByHex is true.", nameof(message));

            var entitySecretCiphertext = await _cryptoUtils.GenerateEntitySecretCiphertextAsync(entitySecret);

            var request = new SignMessageRequest
            {
                WalletId = walletId,
                Message = message,
                EntitySecretCiphertext = entitySecretCiphertext,
                EncodedByHex = encodedByHex,
                Memo = memo
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "developer/sign/message")
            {
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
            };
            if (!string.IsNullOrEmpty(xRequestId))
                requestMessage.Headers.Add("X-Request-Id", xRequestId);

            var response = await _httpClient.SendAsync(requestMessage);
            await HandleResponseAsync(response);

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SignMessageResponseWrapper>(content).Data;
        }

        /// <summary>
        /// Signs EIP-712 typed structured data from a specified developer-controlled wallet (EVM chains only).
        /// </summary>
        /// <param name="walletId">The wallet ID.</param>
        /// <param name="data">The EIP-712 typed structured data to sign.</param>
        /// <param name="entitySecret">Unencrypted Entity Secret for ciphertext generation.</param>
        /// <param name="memo">Optional human-readable explanation for the sign action.</param>
        /// <param name="xRequestId">Optional request identifier for Circle Support.</param>
        /// <returns>A <see cref="SignTypedDataResponse"/> containing the signature.</returns>
        /// <exception cref="ArgumentException">Thrown when parameters are invalid.</exception>
        /// <exception cref="CircleApiException">Thrown when the API request fails.</exception>
        public async Task<SignTypedDataResponse> SignTypedDataAsync(
            string walletId,
            string data,
            string entitySecret,
            string memo = null,
            string xRequestId = null)
        {
            if (string.IsNullOrEmpty(walletId))
                throw new ArgumentException("Wallet ID cannot be null or empty.", nameof(walletId));
            if (string.IsNullOrEmpty(data))
                throw new ArgumentException("Data cannot be null or empty.", nameof(data));
            if (string.IsNullOrEmpty(entitySecret) || entitySecret.Length != 64 || !CryptoUtils.IsHexString(entitySecret))
                throw new ArgumentException("Entity Secret must be a 32-byte hex string (64 characters).", nameof(entitySecret));

            var entitySecretCiphertext = await _cryptoUtils.GenerateEntitySecretCiphertextAsync(entitySecret);

            var request = new SignTypedDataRequest
            {
                WalletId = walletId,
                Data = data,
                EntitySecretCiphertext = entitySecretCiphertext,
                Memo = memo
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "developer/sign/typedData")
            {
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
            };
            if (!string.IsNullOrEmpty(xRequestId))
                requestMessage.Headers.Add("X-Request-Id", xRequestId);

            var response = await _httpClient.SendAsync(requestMessage);
            await HandleResponseAsync(response);

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SignTypedDataResponseWrapper>(content).Data;
        }

        /// <summary>
        /// Signs a transaction from a specified developer-controlled wallet (supported chains only).
        /// </summary>
        /// <param name="walletId">The wallet ID.</param>
        /// <param name="entitySecret">Unencrypted Entity Secret for ciphertext generation.</param>
        /// <param name="rawTransaction">Raw transaction string (base64 for NEAR/Solana, hex for EVM).</param>
        /// <param name="transaction">JSON transaction object (EVM chains only).</param>
        /// <param name="memo">Optional human-readable explanation for the sign action.</param>
        /// <param name="xRequestId">Optional request identifier for Circle Support.</param>
        /// <returns>A <see cref="SignTransactionResponse"/> containing the signature and signed transaction.</returns>
        /// <exception cref="ArgumentException">Thrown when parameters are invalid.</exception>
        /// <exception cref="CircleApiException">Thrown when the API request fails.</exception>
        public async Task<SignTransactionResponse> SignTransactionAsync(
            string walletId,
            string entitySecret,
            string rawTransaction = null,
            string transaction = null,
            string memo = null,
            string xRequestId = null)
        {
            if (string.IsNullOrEmpty(walletId))
                throw new ArgumentException("Wallet ID cannot be null or empty.", nameof(walletId));
            if (string.IsNullOrEmpty(entitySecret) || entitySecret.Length != 64 || !CryptoUtils.IsHexString(entitySecret))
                throw new ArgumentException("Entity Secret must be a 32-byte hex string (64 characters).", nameof(entitySecret));
            if (string.IsNullOrEmpty(rawTransaction) && string.IsNullOrEmpty(transaction))
                throw new ArgumentException("Either rawTransaction or transaction must be provided.", nameof(rawTransaction));
            if (!string.IsNullOrEmpty(rawTransaction) && !string.IsNullOrEmpty(transaction))
                throw new ArgumentException("Cannot specify both rawTransaction and transaction.", nameof(rawTransaction));

            var entitySecretCiphertext = await _cryptoUtils.GenerateEntitySecretCiphertextAsync(entitySecret);

            var request = new SignTransactionRequest
            {
                WalletId = walletId,
                EntitySecretCiphertext = entitySecretCiphertext,
                RawTransaction = rawTransaction,
                Transaction = transaction,
                Memo = memo
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "developer/sign/transaction")
            {
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
            };
            if (!string.IsNullOrEmpty(xRequestId))
                requestMessage.Headers.Add("X-Request-Id", xRequestId);

            var response = await _httpClient.SendAsync(requestMessage);
            await HandleResponseAsync(response);

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SignTransactionResponseWrapper>(content).Data;
        }

        /// <summary>
        /// Signs a delegate action from a specified developer-controlled wallet (NEAR chains only).
        /// </summary>
        /// <param name="walletId">The wallet ID.</param>
        /// <param name="unsignedDelegateAction">Base64-encoded unsigned delegate action string.</param>
        /// <param name="entitySecret">Unencrypted Entity Secret for ciphertext generation.</param>
        /// <param name="xRequestId">Optional request identifier for Circle Support.</param>
        /// <returns>A <see cref="SignDelegateActionResponse"/> containing the signature and signed delegate action.</returns>
        /// <exception cref="ArgumentException">Thrown when parameters are invalid.</exception>
        /// <exception cref="CircleApiException">Thrown when the API request fails.</exception>
        public async Task<SignDelegateActionResponse> SignDelegateActionAsync(
            string walletId,
            string unsignedDelegateAction,
            string entitySecret,
            string xRequestId = null)
        {
            if (string.IsNullOrEmpty(walletId))
                throw new ArgumentException("Wallet ID cannot be null or empty.", nameof(walletId));
            if (string.IsNullOrEmpty(unsignedDelegateAction))
                throw new ArgumentException("Unsigned delegate action cannot be null or empty.", nameof(unsignedDelegateAction));
            if (string.IsNullOrEmpty(entitySecret) || entitySecret.Length != 64 || !CryptoUtils.IsHexString(entitySecret))
                throw new ArgumentException("Entity Secret must be a 32-byte hex string (64 characters).", nameof(entitySecret));

            var entitySecretCiphertext = await _cryptoUtils.GenerateEntitySecretCiphertextAsync(entitySecret);

            var request = new SignDelegateActionRequest
            {
                WalletId = walletId,
                UnsignedDelegateAction = unsignedDelegateAction,
                EntitySecretCiphertext = entitySecretCiphertext
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "developer/sign/delegateAction")
            {
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
            };
            if (!string.IsNullOrEmpty(xRequestId))
                requestMessage.Headers.Add("X-Request-Id", xRequestId);

            var response = await _httpClient.SendAsync(requestMessage);
            await HandleResponseAsync(response);

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SignDelegateActionResponseWrapper>(content).Data;
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