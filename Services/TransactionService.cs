using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Provides functionality for managing blockchain transactions through the Circle API.
    /// This includes listing transactions, getting transaction details, creating transfers,
    /// validating addresses, and estimating contract execution fees.
    /// </summary>
    public class TransactionService
    {
        private readonly HttpClient _httpClient;
        private readonly CryptoUtils _cryptoUtils;

        /// <summary>
        /// Initializes a new instance of the TransactionService class.
        /// </summary>
        /// <param name="httpClient">The HTTP client used to make API requests.</param>
        /// <param name="cryptoUtils">Utility class for cryptographic operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when httpClient or cryptoUtils is null.</exception>
        public TransactionService(HttpClient httpClient, CryptoUtils cryptoUtils)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _cryptoUtils = cryptoUtils ?? throw new ArgumentNullException(nameof(cryptoUtils));
        }

        /// <summary>
        /// Lists all transactions with optional filters.
        /// </summary>
        /// <param name="blockchain">Filter by blockchain.</param>
        /// <param name="custodyType">Filter by custody type.</param>
        /// <param name="destinationAddress">Filter by destination address.</param>
        /// <param name="includeAll">Include monitored and non-monitored tokens.</param>
        /// <param name="operation">Filter by transaction operation.</param>
        /// <param name="state">Filter by transaction state.</param>
        /// <param name="txHash">Filter by transaction hash.</param>
        /// <param name="txType">Filter by transaction type.</param>
        /// <param name="walletIds">Comma-separated list of wallet IDs.</param>
        /// <param name="from">Filter transactions created since this date (inclusive).</param>
        /// <param name="to">Filter transactions created before this date (inclusive).</param>
        /// <param name="pageBefore">Pagination cursor for previous page.</param>
        /// <param name="pageAfter">Pagination cursor for next page.</param>
        /// <param name="pageSize">Number of items per page (1-50).</param>
        /// <param name="xRequestId">Optional request identifier for Circle Support.</param>
        /// <returns>A <see cref="TransactionsResponse"/> containing the list of transactions.</returns>
        /// <exception cref="ArgumentException">Thrown when parameters are invalid.</exception>
        /// <exception cref="CircleApiException">Thrown when the API request fails.</exception>
        public async Task<TransactionsResponse> ListTransactionsAsync(
            string blockchain = null,
            string custodyType = null,
            string destinationAddress = null,
            bool? includeAll = null,
            string operation = null,
            string state = null,
            string txHash = null,
            string txType = null,
            string walletIds = null,
            DateTime? from = null,
            DateTime? to = null,
            string pageBefore = null,
            string pageAfter = null,
            int? pageSize = null,
            string xRequestId = null)
        {
            if (!string.IsNullOrEmpty(pageBefore) && !string.IsNullOrEmpty(pageAfter))
                throw new ArgumentException("Cannot specify both pageBefore and pageAfter.");
            if (pageSize.HasValue && (pageSize < 1 || pageSize > 50))
                throw new ArgumentException("Page size must be between 1 and 50.", nameof(pageSize));

            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(blockchain)) queryParams.Add($"blockchain={Uri.EscapeDataString(blockchain)}");
            if (!string.IsNullOrEmpty(custodyType)) queryParams.Add($"custodyType={Uri.EscapeDataString(custodyType)}");
            if (!string.IsNullOrEmpty(destinationAddress)) queryParams.Add($"destinationAddress={Uri.EscapeDataString(destinationAddress)}");
            if (includeAll.HasValue) queryParams.Add($"includeAll={includeAll.Value.ToString().ToLower()}");
            if (!string.IsNullOrEmpty(operation)) queryParams.Add($"operation={Uri.EscapeDataString(operation)}");
            if (!string.IsNullOrEmpty(state)) queryParams.Add($"state={Uri.EscapeDataString(state)}");
            if (!string.IsNullOrEmpty(txHash)) queryParams.Add($"txHash={Uri.EscapeDataString(txHash)}");
            if (!string.IsNullOrEmpty(txType)) queryParams.Add($"txType={Uri.EscapeDataString(txType)}");
            if (!string.IsNullOrEmpty(walletIds)) queryParams.Add($"walletIds={Uri.EscapeDataString(walletIds)}");
            if (from.HasValue) queryParams.Add($"from={Uri.EscapeDataString(from.Value.ToString("o"))}");
            if (to.HasValue) queryParams.Add($"to={Uri.EscapeDataString(to.Value.ToString("o"))}");
            if (!string.IsNullOrEmpty(pageBefore)) queryParams.Add($"pageBefore={Uri.EscapeDataString(pageBefore)}");
            if (!string.IsNullOrEmpty(pageAfter)) queryParams.Add($"pageAfter={Uri.EscapeDataString(pageAfter)}");
            if (pageSize.HasValue) queryParams.Add($"pageSize={pageSize.Value}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"transactions{queryString}");
            if (!string.IsNullOrEmpty(xRequestId))
                requestMessage.Headers.Add("X-Request-Id", xRequestId);

            var response = await _httpClient.SendAsync(requestMessage);
            await HandleResponseAsync(response);

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TransactionsResponseWrapper>(content).Data;
        }

        /// <summary>
        /// Retrieves a single transaction by its ID.
        /// </summary>
        /// <param name="id">The transaction ID.</param>
        /// <param name="txType">Filter by transaction type.</param>
        /// <param name="xRequestId">Optional request identifier for Circle Support.</param>
        /// <returns>A <see cref="TransactionResponse"/> containing the transaction details.</returns>
        /// <exception cref="ArgumentException">Thrown when parameters are invalid.</exception>
        /// <exception cref="CircleApiException">Thrown when the API request fails.</exception>
        public async Task<TransactionResponse> GetTransactionAsync(
            string id,
            string txType = null,
            string xRequestId = null)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Transaction ID cannot be null or empty.", nameof(id));

            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(txType)) queryParams.Add($"txType={Uri.EscapeDataString(txType)}");
            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"transactions/{Uri.EscapeDataString(id)}{queryString}");
            if (!string.IsNullOrEmpty(xRequestId))
                requestMessage.Headers.Add("X-Request-Id", xRequestId);

            var response = await _httpClient.SendAsync(requestMessage);
            await HandleResponseAsync(response);

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TransactionResponseWrapper>(content).Data;
        }

        /// <summary>
        /// Initiates an on-chain digital asset transfer from a developer-controlled wallet.
        /// </summary>
        /// <param name="walletId">The wallet ID (required if sourceAddress and blockchain are not provided).</param>
        /// <param name="entitySecret">Unencrypted Entity Secret for ciphertext generation.</param>
        /// <param name="destinationAddress">The destination blockchain address.</param>
        /// <param name="idempotencyKey">UUIDv4 idempotency key.</param>
        /// <param name="amounts">Transfer amounts (at least one required).</param>
        /// <param name="feeLevel">Fee level (LOW, MEDIUM, HIGH).</param>
        /// <param name="gasLimit">Gas limit (required if feeLevel is not provided).</param>
        /// <param name="gasPrice">Gas price in gwei (non-EIP-1559 chains).</param>
        /// <param name="maxFee">Max fee in gwei (EIP-1559 chains).</param>
        /// <param name="priorityFee">Priority fee in gwei (EIP-1559 chains).</param>
        /// <param name="nftTokenIds">NFT token IDs for ERC-1155 batch transfers.</param>
        /// <param name="refId">Optional reference ID for the transaction.</param>
        /// <param name="tokenId">Token ID (mutually exclusive with tokenAddress and blockchain).</param>
        /// <param name="tokenAddress">Token address (empty for native tokens).</param>
        /// <param name="blockchain">Blockchain (required if tokenId is not provided).</param>
        /// <param name="xRequestId">Optional request identifier for Circle Support.</param>
        /// <returns>A <see cref="CreateTransferResponse"/> containing the transaction ID and state.</returns>
        /// <exception cref="ArgumentException">Thrown when parameters are invalid.</exception>
        /// <exception cref="CircleApiException">Thrown when the API request fails.</exception>
        public async Task<CreateTransferResponse> CreateTransferTransactionAsync(
            string walletId,
            string entitySecret,
            string destinationAddress,
            string idempotencyKey,
            string[] amounts,
            string feeLevel = null,
            string gasLimit = null,
            string gasPrice = null,
            string maxFee = null,
            string priorityFee = null,
            string[] nftTokenIds = null,
            string refId = null,
            string tokenId = null,
            string tokenAddress = null,
            string blockchain = null,
            string xRequestId = null)
        {
            if (string.IsNullOrEmpty(walletId) && (string.IsNullOrEmpty(blockchain) || string.IsNullOrEmpty(destinationAddress)))
                throw new ArgumentException("Wallet ID is required when sourceAddress and blockchain are not provided.");
            if (string.IsNullOrEmpty(destinationAddress))
                throw new ArgumentException("Destination address cannot be null or empty.", nameof(destinationAddress));
            if (string.IsNullOrEmpty(idempotencyKey) || !Guid.TryParse(idempotencyKey, out _))
                throw new ArgumentException("Idempotency key must be a valid UUIDv4.", nameof(idempotencyKey));
            if (amounts == null || !amounts.Any())
                throw new ArgumentException("At least one amount is required.", nameof(amounts));
            if (string.IsNullOrEmpty(entitySecret) || entitySecret.Length != 64 || !CryptoUtils.IsHexString(entitySecret))
                throw new ArgumentException("Entity Secret must be a 32-byte hex string (64 characters).", nameof(entitySecret));
            if (!string.IsNullOrEmpty(tokenId) && (!string.IsNullOrEmpty(tokenAddress) || !string.IsNullOrEmpty(blockchain)))
                throw new ArgumentException("Token ID is mutually exclusive with tokenAddress and blockchain.");
            if (string.IsNullOrEmpty(tokenId) && string.IsNullOrEmpty(blockchain))
                throw new ArgumentException("Blockchain is required if tokenId is not provided.");
            if (nftTokenIds != null && nftTokenIds.Length != amounts.Length)
                throw new ArgumentException("NFT token IDs length must match amounts length for ERC-1155 transfers.");
            if (!string.IsNullOrEmpty(feeLevel) && (!new[] { "LOW", "MEDIUM", "HIGH" }.Contains(feeLevel)))
                throw new ArgumentException("Fee level must be LOW, MEDIUM, or HIGH.", nameof(feeLevel));
            if (string.IsNullOrEmpty(feeLevel) && string.IsNullOrEmpty(gasLimit))
                throw new ArgumentException("Gas limit is required if feeLevel is not provided.", nameof(gasLimit));
            if (!string.IsNullOrEmpty(feeLevel) && (!string.IsNullOrEmpty(gasPrice) || !string.IsNullOrEmpty(maxFee) || !string.IsNullOrEmpty(priorityFee)))
                throw new ArgumentException("Fee level cannot be used with gasPrice, maxFee, or priorityFee.");
            if (!string.IsNullOrEmpty(maxFee) && (string.IsNullOrEmpty(priorityFee) || string.IsNullOrEmpty(gasLimit)))
                throw new ArgumentException("Max fee requires priorityFee and gasLimit.");
            if (!string.IsNullOrEmpty(priorityFee) && (string.IsNullOrEmpty(maxFee) || string.IsNullOrEmpty(gasLimit)))
                throw new ArgumentException("Priority fee requires maxFee and gasLimit.");
            if (!string.IsNullOrEmpty(gasPrice) && (!string.IsNullOrEmpty(maxFee) || !string.IsNullOrEmpty(priorityFee) || !string.IsNullOrEmpty(feeLevel)))
                throw new ArgumentException("Gas price cannot be used with maxFee, priorityFee, or feeLevel.");

            var entitySecretCiphertext = await _cryptoUtils.GenerateEntitySecretCiphertextAsync(entitySecret);

            var request = new CreateTransferRequest
            {
                WalletId = walletId,
                EntitySecretCiphertext = entitySecretCiphertext,
                DestinationAddress = destinationAddress,
                IdempotencyKey = idempotencyKey,
                Amounts = amounts,
                FeeLevel = feeLevel,
                GasLimit = gasLimit,
                GasPrice = gasPrice,
                MaxFee = maxFee,
                PriorityFee = priorityFee,
                NftTokenIds = nftTokenIds,
                RefId = refId,
                TokenId = tokenId,
                TokenAddress = tokenAddress,
                Blockchain = blockchain
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "developer/transactions/transfer")
            {
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
            };
            if (!string.IsNullOrEmpty(xRequestId))
                requestMessage.Headers.Add("X-Request-Id", xRequestId);

            var response = await _httpClient.SendAsync(requestMessage);
            await HandleResponseAsync(response);

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CreateTransferResponseWrapper>(content).Data;
        }

        /// <summary>
        /// Validates a blockchain address for a specified token and blockchain.
        /// </summary>
        /// <param name="blockchain">The blockchain network.</param>
        /// <param name="address">The blockchain address to validate.</param>
        /// <param name="xRequestId">Optional request identifier for Circle Support.</param>
        /// <returns>A <see cref="ValidateAddressResponse"/> indicating if the address is valid.</returns>
        /// <exception cref="ArgumentException">Thrown when parameters are invalid.</exception>
        /// <exception cref="CircleApiException">Thrown when the API request fails.</exception>
        public async Task<ValidateAddressResponse> ValidateAddressAsync(
            string blockchain,
            string address,
            string xRequestId = null)
        {
            if (string.IsNullOrEmpty(blockchain))
                throw new ArgumentException("Blockchain cannot be null or empty.", nameof(blockchain));
            if (string.IsNullOrEmpty(address))
                throw new ArgumentException("Address cannot be null or empty.", nameof(address));

            var validBlockchains = new HashSet<string>
            {
                "ETH", "ETH-SEPOLIA", "AVAX", "AVAX-FUJI", "MATIC", "MATIC-AMOY",
                "SOL", "SOL-DEVNET", "ARB", "ARB-SEPOLIA", "NEAR", "NEAR-TESTNET",
                "EVM", "EVM-TESTNET", "UNI", "UNI-SEPOLIA", "BASE", "BASE-SEPOLIA",
                "OP", "OP-SEPOLIA"
            };
            if (!validBlockchains.Contains(blockchain))
                throw new ArgumentException("Invalid blockchain specified.", nameof(blockchain));

            var request = new ValidateAddressRequest
            {
                Blockchain = blockchain,
                Address = address
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "transactions/validateAddress")
            {
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
            };
            if (!string.IsNullOrEmpty(xRequestId))
                requestMessage.Headers.Add("X-Request-Id", xRequestId);

            var response = await _httpClient.SendAsync(requestMessage);
            await HandleResponseAsync(response);

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ValidateAddressResponseWrapper>(content).Data;
        }

        /// <summary>
        /// Estimates gas fees for a contract execution transaction.
        /// </summary>
        /// <param name="contractAddress">The blockchain address of the contract.</param>
        /// <param name="blockchain">Blockchain (required if walletId is not provided).</param>
        /// <param name="sourceAddress">Source address (required if walletId is not provided).</param>
        /// <param name="walletId">Wallet ID (required if sourceAddress and blockchain are not provided).</param>
        /// <param name="abiFunctionSignature">Contract ABI function signature (e.g., burn(uint256)).</param>
        /// <param name="abiParameters">ABI function parameters (mutually exclusive with callData).</param>
        /// <param name="callData">Raw transaction data (hex, mutually exclusive with abiFunctionSignature).</param>
        /// <param name="amount">Amount of native token to send (optional).</param>
        /// <param name="xRequestId">Optional request identifier for Circle Support.</param>
        /// <returns>A <see cref="EstimateFeeResponse"/> containing fee estimates.</returns>
        /// <exception cref="ArgumentException">Thrown when parameters are invalid.</exception>
        /// <exception cref="CircleApiException">Thrown when the API request fails.</exception>
        public async Task<EstimateFeeResponse> EstimateContractExecutionFeeAsync(
            string contractAddress,
            string blockchain = null,
            string sourceAddress = null,
            string walletId = null,
            string abiFunctionSignature = null,
            object[] abiParameters = null,
            string callData = null,
            string amount = null,
            string xRequestId = null)
        {
            if (string.IsNullOrEmpty(contractAddress))
                throw new ArgumentException("Contract address cannot be null or empty.", nameof(contractAddress));
            if (string.IsNullOrEmpty(walletId) && (string.IsNullOrEmpty(blockchain) || string.IsNullOrEmpty(sourceAddress)))
                throw new ArgumentException("Wallet ID is required when sourceAddress and blockchain are not provided.");
            if (!string.IsNullOrEmpty(walletId) && (!string.IsNullOrEmpty(blockchain) || !string.IsNullOrEmpty(sourceAddress)))
                throw new ArgumentException("Wallet ID is mutually exclusive with sourceAddress and blockchain.");
            if (string.IsNullOrEmpty(abiFunctionSignature) && string.IsNullOrEmpty(callData))
                throw new ArgumentException("Either abiFunctionSignature or callData must be provided.");
            if (!string.IsNullOrEmpty(abiFunctionSignature) && !string.IsNullOrEmpty(callData))
                throw new ArgumentException("abiFunctionSignature and callData are mutually exclusive.");
            if (!string.IsNullOrEmpty(callData) && (!callData.StartsWith("0x") || callData.Length % 2 != 0 || !CryptoUtils.IsHexString(callData.Substring(2))))
                throw new ArgumentException("callData must be a valid hex string starting with '0x'.", nameof(callData));

            var validBlockchains = new HashSet<string>
            {
                "ETH", "ETH-SEPOLIA", "AVAX", "AVAX-FUJI", "MATIC", "MATIC-AMOY",
                "ARB", "ARB-SEPOLIA", "UNI", "UNI-SEPOLIA", "BASE", "BASE-SEPOLIA",
                "OP", "OP-SEPOLIA"
            };
            if (!string.IsNullOrEmpty(blockchain) && !validBlockchains.Contains(blockchain))
                throw new ArgumentException("Invalid blockchain specified.", nameof(blockchain));

            var request = new EstimateContractExecutionFeeRequest
            {
                ContractAddress = contractAddress,
                Blockchain = blockchain,
                SourceAddress = sourceAddress,
                WalletId = walletId,
                AbiFunctionSignature = abiFunctionSignature,
                AbiParameters = abiParameters,
                CallData = callData,
                Amount = amount
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "transactions/contractExecution/estimateFee")
            {
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
            };
            if (!string.IsNullOrEmpty(xRequestId))
                requestMessage.Headers.Add("X-Request-Id", xRequestId);

            var response = await _httpClient.SendAsync(requestMessage);
            await HandleResponseAsync(response);

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<EstimateFeeResponseWrapper>(content).Data;
        }

        /// <summary>
        /// Estimates gas fees for a transfer transaction.
        /// </summary>
        /// <param name="destinationAddress">The destination blockchain address.</param>
        /// <param name="amounts">Transfer amounts (at least one required).</param>
        /// <param name="nftTokenIds">NFT token IDs for ERC-1155 batch transfers.</param>
        /// <param name="sourceAddress">Source address (required if walletId is not provided).</param>
        /// <param name="tokenId">Token ID (mutually exclusive with tokenAddress and blockchain).</param>
        /// <param name="tokenAddress">Token address (empty for native tokens).</param>
        /// <param name="blockchain">Blockchain (required if tokenId is not provided).</param>
        /// <param name="walletId">Wallet ID (required if sourceAddress and blockchain are not provided).</param>
        /// <param name="xRequestId">Optional request identifier for Circle Support.</param>
        /// <returns>A <see cref="EstimateFeeResponse"/> containing fee estimates.</returns>
        /// <exception cref="ArgumentException">Thrown when parameters are invalid.</exception>
        /// <exception cref="CircleApiException">Thrown when the API request fails.</exception>
        public async Task<EstimateFeeResponse> EstimateTransferFeeAsync(
            string destinationAddress,
            string[] amounts,
            string[] nftTokenIds = null,
            string sourceAddress = null,
            string tokenId = null,
            string tokenAddress = null,
            string blockchain = null,
            string walletId = null,
            string xRequestId = null)
        {
            if (string.IsNullOrEmpty(destinationAddress))
                throw new ArgumentException("Destination address cannot be null or empty.", nameof(destinationAddress));
            if (amounts == null || !amounts.Any())
                throw new ArgumentException("At least one amount is required.", nameof(amounts));
            if (nftTokenIds != null && nftTokenIds.Length != amounts.Length)
                throw new ArgumentException("NFT token IDs length must match amounts length for ERC-1155 transfers.");
            if (string.IsNullOrEmpty(walletId) && (string.IsNullOrEmpty(blockchain) || string.IsNullOrEmpty(sourceAddress)))
                throw new ArgumentException("Wallet ID is required when sourceAddress and blockchain are not provided.");
            if (!string.IsNullOrEmpty(walletId) && (!string.IsNullOrEmpty(blockchain) || !string.IsNullOrEmpty(sourceAddress)))
                throw new ArgumentException("Wallet ID is mutually exclusive with sourceAddress and blockchain.");
            if (!string.IsNullOrEmpty(tokenId) && (!string.IsNullOrEmpty(tokenAddress) || !string.IsNullOrEmpty(blockchain)))
                throw new ArgumentException("Token ID is mutually exclusive with tokenAddress and blockchain.");
            if (string.IsNullOrEmpty(tokenId) && string.IsNullOrEmpty(blockchain))
                throw new ArgumentException("Blockchain is required if tokenId is not provided.");

            var validBlockchains = new HashSet<string>
            {
                "ETH", "ETH-SEPOLIA", "AVAX", "AVAX-FUJI", "MATIC", "MATIC-AMOY",
                "SOL", "SOL-DEVNET", "ARB", "ARB-SEPOLIA", "UNI", "UNI-SEPOLIA",
                "BASE", "BASE-SEPOLIA", "OP", "OP-SEPOLIA"
            };
            if (!string.IsNullOrEmpty(blockchain) && !validBlockchains.Contains(blockchain))
                throw new ArgumentException("Invalid blockchain specified.", nameof(blockchain));

            var request = new EstimateTransferFeeRequest
            {
                DestinationAddress = destinationAddress,
                Amounts = amounts,
                NftTokenIds = nftTokenIds,
                SourceAddress = sourceAddress,
                TokenId = tokenId,
                TokenAddress = tokenAddress,
                Blockchain = blockchain,
                WalletId = walletId
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "transactions/transfer/estimateFee")
            {
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
            };
            if (!string.IsNullOrEmpty(xRequestId))
                requestMessage.Headers.Add("X-Request-Id", xRequestId);

            var response = await _httpClient.SendAsync(requestMessage);
            await HandleResponseAsync(response);

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<EstimateFeeResponseWrapper>(content).Data;
        }

        /// <summary>
        /// Creates a transaction to execute a smart contract.
        /// </summary>
        /// <param name="walletId">Wallet ID (required if sourceAddress and blockchain are not provided).</param>
        /// <param name="entitySecret">Unencrypted Entity Secret for ciphertext generation.</param>
        /// <param name="contractAddress">The blockchain address of the contract.</param>
        /// <param name="idempotencyKey">UUIDv4 idempotency key.</param>
        /// <param name="abiFunctionSignature">Contract ABI function signature (e.g., burn(uint256)).</param>
        /// <param name="abiParameters">ABI function parameters (mutually exclusive with callData).</param>
        /// <param name="callData">Raw transaction data (hex, mutually exclusive with abiFunctionSignature).</param>
        /// <param name="amount">Amount of native token to send (optional).</param>
        /// <param name="feeLevel">Fee level (LOW, MEDIUM, HIGH).</param>
        /// <param name="gasLimit">Gas limit (required if feeLevel is not provided).</param>
        /// <param name="gasPrice">Gas price in gwei (non-EIP-1559 chains).</param>
        /// <param name="maxFee">Max fee in gwei (EIP-1559 chains).</param>
        /// <param name="priorityFee">Priority fee in gwei (EIP-1559 chains).</param>
        /// <param name="refId">Optional reference ID for the transaction.</param>
        /// <param name="xRequestId">Optional request identifier for Circle Support.</param>
        /// <returns>A <see cref="CreateContractExecutionResponse"/> containing the transaction ID and state.</returns>
        /// <exception cref="ArgumentException">Thrown when parameters are invalid.</exception>
        /// <exception cref="CircleApiException">Thrown when the API request fails.</exception>
        public async Task<CreateContractExecutionResponse> CreateContractExecutionTransactionAsync(
            string walletId,
            string entitySecret,
            string contractAddress,
            string idempotencyKey,
            string abiFunctionSignature = null,
            object[] abiParameters = null,
            string callData = null,
            string amount = null,
            string feeLevel = null,
            string gasLimit = null,
            string gasPrice = null,
            string maxFee = null,
            string priorityFee = null,
            string refId = null,
            string xRequestId = null)
        {
            if (string.IsNullOrEmpty(walletId))
                throw new ArgumentException("Wallet ID cannot be null or empty.", nameof(walletId));
            if (string.IsNullOrEmpty(entitySecret) || entitySecret.Length != 64 || !CryptoUtils.IsHexString(entitySecret))
                throw new ArgumentException("Entity Secret must be a 32-byte hex string (64 characters).", nameof(entitySecret));
            if (string.IsNullOrEmpty(contractAddress))
                throw new ArgumentException("Contract address cannot be null or empty.", nameof(contractAddress));
            if (string.IsNullOrEmpty(idempotencyKey) || !Guid.TryParse(idempotencyKey, out _))
                throw new ArgumentException("Idempotency key must be a valid UUIDv4.", nameof(idempotencyKey));
            if (string.IsNullOrEmpty(abiFunctionSignature) && string.IsNullOrEmpty(callData))
                throw new ArgumentException("Either abiFunctionSignature or callData must be provided.");
            if (!string.IsNullOrEmpty(abiFunctionSignature) && !string.IsNullOrEmpty(callData))
                throw new ArgumentException("abiFunctionSignature and callData are mutually exclusive.");
            if (!string.IsNullOrEmpty(callData) && (!callData.StartsWith("0x") || callData.Length % 2 != 0 || !CryptoUtils.IsHexString(callData.Substring(2))))
                throw new ArgumentException("callData must be a valid hex string starting with '0x'.", nameof(callData));
            if (!string.IsNullOrEmpty(feeLevel) && (!new[] { "LOW", "MEDIUM", "HIGH" }.Contains(feeLevel)))
                throw new ArgumentException("Fee level must be LOW, MEDIUM, or HIGH.", nameof(feeLevel));
            if (string.IsNullOrEmpty(feeLevel) && string.IsNullOrEmpty(gasLimit))
                throw new ArgumentException("Gas limit is required if feeLevel is not provided.", nameof(gasLimit));
            if (!string.IsNullOrEmpty(feeLevel) && (!string.IsNullOrEmpty(gasPrice) || !string.IsNullOrEmpty(maxFee) || !string.IsNullOrEmpty(priorityFee)))
                throw new ArgumentException("Fee level cannot be used with gasPrice, maxFee, or priorityFee.");
            if (!string.IsNullOrEmpty(maxFee) && (string.IsNullOrEmpty(priorityFee) || string.IsNullOrEmpty(gasLimit)))
                throw new ArgumentException("Max fee requires priorityFee and gasLimit.");
            if (!string.IsNullOrEmpty(priorityFee) && (string.IsNullOrEmpty(maxFee) || string.IsNullOrEmpty(gasLimit)))
                throw new ArgumentException("Priority fee requires maxFee and gasLimit.");
            if (!string.IsNullOrEmpty(gasPrice) && (!string.IsNullOrEmpty(maxFee) || !string.IsNullOrEmpty(priorityFee) || !string.IsNullOrEmpty(feeLevel)))
                throw new ArgumentException("Gas price cannot be used with maxFee, priorityFee, or feeLevel.");

            var entitySecretCiphertext = await _cryptoUtils.GenerateEntitySecretCiphertextAsync(entitySecret);

            var request = new CreateContractExecutionRequest
            {
                WalletId = walletId,
                EntitySecretCiphertext = entitySecretCiphertext,
                ContractAddress = contractAddress,
                IdempotencyKey = idempotencyKey,
                AbiFunctionSignature = abiFunctionSignature,
                AbiParameters = abiParameters,
                CallData = callData,
                Amount = amount,
                FeeLevel = feeLevel,
                GasLimit = gasLimit,
                GasPrice = gasPrice,
                MaxFee = maxFee,
                PriorityFee = priorityFee,
                RefId = refId
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "developer/transactions/contractExecution")
            {
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
            };
            if (!string.IsNullOrEmpty(xRequestId))
                requestMessage.Headers.Add("X-Request-Id", xRequestId);

            var response = await _httpClient.SendAsync(requestMessage);
            await HandleResponseAsync(response);

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CreateContractExecutionResponseWrapper>(content).Data;
        }

        /// <summary>
        /// Creates a transaction to upgrade a wallet.
        /// </summary>
        /// <param name="walletId">Wallet ID.</param>
        /// <param name="entitySecret">Unencrypted Entity Secret for ciphertext generation.</param>
        /// <param name="newScaCore">The SCA version to upgrade to (e.g., circle_6900_singleowner_v2).</param>
        /// <param name="idempotencyKey">UUIDv4 idempotency key.</param>
        /// <param name="feeLevel">Fee level (LOW, MEDIUM, HIGH).</param>
        /// <param name="gasLimit">Gas limit (required if feeLevel is not provided).</param>
        /// <param name="gasPrice">Gas price in gwei (non-EIP-1559 chains).</param>
        /// <param name="maxFee">Max fee in gwei (EIP-1559 chains).</param>
        /// <param name="priorityFee">Priority fee in gwei (EIP-1559 chains).</param>
        /// <param name="refId">Optional reference ID for the transaction.</param>
        /// <param name="xRequestId">Optional request identifier for Circle Support.</param>
        /// <returns>A <see cref="CreateWalletUpgradeResponse"/> containing the transaction ID and state.</returns>
        /// <exception cref="ArgumentException">Thrown when parameters are invalid.</exception>
        /// <exception cref="CircleApiException">Thrown when the API request fails.</exception>
        public async Task<CreateWalletUpgradeResponse> CreateWalletUpgradeTransactionAsync(
            string walletId,
            string entitySecret,
            string newScaCore,
            string idempotencyKey,
            string feeLevel = null,
            string gasLimit = null,
            string gasPrice = null,
            string maxFee = null,
            string priorityFee = null,
            string refId = null,
            string xRequestId = null)
        {
            if (string.IsNullOrEmpty(walletId))
                throw new ArgumentException("Wallet ID cannot be null or empty.", nameof(walletId));
            if (string.IsNullOrEmpty(entitySecret) || entitySecret.Length != 64 || !CryptoUtils.IsHexString(entitySecret))
                throw new ArgumentException("Entity Secret must be a 32-byte hex string (64 characters).", nameof(entitySecret));
            if (string.IsNullOrEmpty(newScaCore) || newScaCore != "circle_6900_singleowner_v2")
                throw new ArgumentException("newScaCore must be 'circle_6900_singleowner_v2'.", nameof(newScaCore));
            if (string.IsNullOrEmpty(idempotencyKey) || !Guid.TryParse(idempotencyKey, out _))
                throw new ArgumentException("Idempotency key must be a valid UUIDv4.", nameof(idempotencyKey));
            if (!string.IsNullOrEmpty(feeLevel) && (!new[] { "LOW", "MEDIUM", "HIGH" }.Contains(feeLevel)))
                throw new ArgumentException("Fee level must be LOW, MEDIUM, or HIGH.", nameof(feeLevel));
            if (string.IsNullOrEmpty(feeLevel) && string.IsNullOrEmpty(gasLimit))
                throw new ArgumentException("Gas limit is required if feeLevel is not provided.", nameof(gasLimit));
            if (!string.IsNullOrEmpty(feeLevel) && (!string.IsNullOrEmpty(gasPrice) || !string.IsNullOrEmpty(maxFee) || !string.IsNullOrEmpty(priorityFee)))
                throw new ArgumentException("Fee level cannot be used with gasPrice, maxFee, or priorityFee.");
            if (!string.IsNullOrEmpty(maxFee) && (string.IsNullOrEmpty(priorityFee) || string.IsNullOrEmpty(gasLimit)))
                throw new ArgumentException("Max fee requires priorityFee and gasLimit.");
            if (!string.IsNullOrEmpty(priorityFee) && (string.IsNullOrEmpty(maxFee) || string.IsNullOrEmpty(gasLimit)))
                throw new ArgumentException("Priority fee requires maxFee and gasLimit.");
            if (!string.IsNullOrEmpty(gasPrice) && (!string.IsNullOrEmpty(maxFee) || !string.IsNullOrEmpty(priorityFee) || !string.IsNullOrEmpty(feeLevel)))
                throw new ArgumentException("Gas price cannot be used with maxFee, priorityFee, or feeLevel.");

            var entitySecretCiphertext = await _cryptoUtils.GenerateEntitySecretCiphertextAsync(entitySecret);

            var request = new CreateWalletUpgradeRequest
            {
                WalletId = walletId,
                EntitySecretCiphertext = entitySecretCiphertext,
                NewScaCore = newScaCore,
                IdempotencyKey = idempotencyKey,
                FeeLevel = feeLevel,
                GasLimit = gasLimit,
                GasPrice = gasPrice,
                MaxFee = maxFee,
                PriorityFee = priorityFee,
                RefId = refId
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "developer/transactions/walletUpgrade")
            {
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
            };
            if (!string.IsNullOrEmpty(xRequestId))
                requestMessage.Headers.Add("X-Request-Id", xRequestId);

            var response = await _httpClient.SendAsync(requestMessage);
            await HandleResponseAsync(response);

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CreateWalletUpgradeResponseWrapper>(content).Data;
        }

        /// <summary>
        /// Cancels a specified transaction from a developer-controlled wallet.
        /// </summary>
        /// <param name="id">The transaction ID.</param>
        /// <param name="entitySecret">Unencrypted Entity Secret for ciphertext generation.</param>
        /// <param name="idempotencyKey">UUIDv4 idempotency key.</param>
        /// <param name="xRequestId">Optional request identifier for Circle Support.</param>
        /// <returns>A <see cref="CancelTransactionResponse"/> containing the transaction ID and state.</returns>
        /// <exception cref="ArgumentException">Thrown when parameters are invalid.</exception>
        /// <exception cref="CircleApiException">Thrown when the API request fails.</exception>
        public async Task<CancelTransactionResponse> CancelTransactionAsync(
            string id,
            string entitySecret,
            string idempotencyKey,
            string xRequestId = null)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Transaction ID cannot be null or empty.", nameof(id));
            if (string.IsNullOrEmpty(entitySecret) || entitySecret.Length != 64 || !CryptoUtils.IsHexString(entitySecret))
                throw new ArgumentException("Entity Secret must be a 32-byte hex string (64 characters).", nameof(entitySecret));
            if (string.IsNullOrEmpty(idempotencyKey) || !Guid.TryParse(idempotencyKey, out _))
                throw new ArgumentException("Idempotency key must be a valid UUIDv4.", nameof(idempotencyKey));

            var entitySecretCiphertext = await _cryptoUtils.GenerateEntitySecretCiphertextAsync(entitySecret);

            var request = new CancelTransactionRequest
            {
                EntitySecretCiphertext = entitySecretCiphertext,
                IdempotencyKey = idempotencyKey
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"developer/transactions/{Uri.EscapeDataString(id)}/cancel")
            {
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
            };
            if (!string.IsNullOrEmpty(xRequestId))
                requestMessage.Headers.Add("X-Request-Id", xRequestId);

            var response = await _httpClient.SendAsync(requestMessage);
            await HandleResponseAsync(response);

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CancelTransactionResponseWrapper>(content).Data;
        }

        /// <summary>
        /// Accelerates a specified transaction from a developer-controlled wallet.
        /// </summary>
        /// <param name="id">The transaction ID.</param>
        /// <param name="entitySecret">Unencrypted Entity Secret for ciphertext generation.</param>
        /// <param name="idempotencyKey">UUIDv4 idempotency key.</param>
        /// <param name="xRequestId">Optional request identifier for Circle Support.</param>
        /// <returns>An <see cref="AccelerateTransactionResponse"/> containing the transaction ID.</returns>
        /// <exception cref="ArgumentException">Thrown when parameters are invalid.</exception>
        /// <exception cref="CircleApiException">Thrown when the API request fails.</exception>
        public async Task<AccelerateTransactionResponse> AccelerateTransactionAsync(
            string id,
            string entitySecret,
            string idempotencyKey,
            string xRequestId = null)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Transaction ID cannot be null or empty.", nameof(id));
            if (string.IsNullOrEmpty(entitySecret) || entitySecret.Length != 64 || !CryptoUtils.IsHexString(entitySecret))
                throw new ArgumentException("Entity Secret must be a 32-byte hex string (64 characters).", nameof(entitySecret));
            if (string.IsNullOrEmpty(idempotencyKey) || !Guid.TryParse(idempotencyKey, out _))
                throw new ArgumentException("Idempotency key must be a valid UUIDv4.", nameof(idempotencyKey));

            var entitySecretCiphertext = await _cryptoUtils.GenerateEntitySecretCiphertextAsync(entitySecret);

            var request = new AccelerateTransactionRequest
            {
                EntitySecretCiphertext = entitySecretCiphertext,
                IdempotencyKey = idempotencyKey
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"developer/transactions/{Uri.EscapeDataString(id)}/accelerate")
            {
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
            };
            if (!string.IsNullOrEmpty(xRequestId))
                requestMessage.Headers.Add("X-Request-Id", xRequestId);

            var response = await _httpClient.SendAsync(requestMessage);
            await HandleResponseAsync(response);

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<AccelerateTransactionResponseWrapper>(content).Data;
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