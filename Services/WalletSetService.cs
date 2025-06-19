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
    /// Service for managing wallet sets in the Circle API.
    /// Provides functionality to create, retrieve, and update wallet sets.
    /// </summary>
    public class WalletSetService
    {
        private readonly HttpClient _httpClient;
        private readonly CryptoUtils _cryptoUtils;
        /// <summary>
        /// Initializes a new instance of the <see cref="WalletSetService"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client used to make API requests.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="httpClient"/> is null.</exception>
        public WalletSetService(HttpClient httpClient, CryptoUtils cryptoUtils)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _cryptoUtils = cryptoUtils ?? throw new ArgumentNullException(nameof(cryptoUtils));
        }

        /// <summary>
        /// Creates a new developer-controlled wallet set.
        /// </summary>
        /// <param name="name">Name or description of the wallet set.</param>
        /// <param name="entitySecret">Unencrypted Entity Secret (hex string).</param>
        /// <param name="xRequestId">Optional request ID for tracking with Circle Support.</param>
        /// <returns>A <see cref="WalletSetResponse"/> containing the created wallet set details.</returns>
        /// <exception cref="ArgumentNullException">Thrown when required parameters are null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="entitySecret"/> is invalid.</exception>
        /// <exception cref="CircleApiException">Thrown when the API request fails.</exception>
        /// <remarks>Requires a unique Entity Secret ciphertext, generated internally.</remarks>
        public async Task<WalletSetResponse> CreateWalletSetAsync(string name, string entitySecret, string xRequestId = null)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrEmpty(entitySecret)) throw new ArgumentNullException(nameof(entitySecret));
            if (entitySecret.Length != 64 || !CryptoUtils.IsHexString(entitySecret))
                throw new ArgumentException("Entity Secret must be a 32-byte hex string (64 characters).", nameof(entitySecret));

            var idempotencyKey = Guid.NewGuid().ToString();
            var entitySecretCiphertext = await _cryptoUtils.GenerateEntitySecretCiphertextAsync(entitySecret);

            var request = new CreateWalletSetRequest
            {
                EntitySecretCiphertext = entitySecretCiphertext,
                IdempotencyKey = idempotencyKey,
                Name = name
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "developer/walletSets")
            {
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
            };
            if (!string.IsNullOrEmpty(xRequestId))
                requestMessage.Headers.Add("X-Request-Id", xRequestId);

            var response = await _httpClient.SendAsync(requestMessage);
            await HandleResponseAsync(response);

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<WalletSetResponseWrapper>(content).Data;
        }

        /// <summary>
        /// Retrieves all wallet sets asynchronously with optional filtering and pagination.
        /// </summary>
        /// <param name="from">Optional start date for filtering wallet sets.</param>
        /// <param name="to">Optional end date for filtering wallet sets.</param>
        /// <param name="pageBefore">Optional token for retrieving the previous page of results.</param>
        /// <param name="pageAfter">Optional token for retrieving the next page of results.</param>
        /// <param name="pageSize">Optional number of results per page (1-50).</param>
        /// <param name="xRequestId">Optional request ID for tracking the request.</param>
        /// <returns>An array of <see cref="WalletSetResponse"/> objects representing the wallet sets.</returns>
        /// <exception cref="ArgumentException">Thrown when invalid pagination parameters are provided.</exception>
        public async Task<WalletSetResponse[]> GetAllWalletSetsAsync(DateTime? from = null, DateTime? to = null, string? pageBefore = null, string? pageAfter = null, int? pageSize = null, string? xRequestId = null)
        {
            if (!string.IsNullOrEmpty(pageBefore) && !string.IsNullOrEmpty(pageAfter))
                throw new ArgumentException("Cannot specify both pageBefore and pageAfter.");
            if (pageSize.HasValue && (pageSize < 1 || pageSize > 50))
                throw new ArgumentException("Page size must be between 1 and 50.", nameof(pageSize));

            var queryParams = new List<string>();
            if (from.HasValue) queryParams.Add($"from={Uri.EscapeDataString(from.Value.ToString("o"))}");
            if (to.HasValue) queryParams.Add($"to={Uri.EscapeDataString(to.Value.ToString("o"))}");
            if (!string.IsNullOrEmpty(pageBefore)) queryParams.Add($"pageBefore={Uri.EscapeDataString(pageBefore)}");
            if (!string.IsNullOrEmpty(pageAfter)) queryParams.Add($"pageAfter={Uri.EscapeDataString(pageAfter)}");
            if (pageSize.HasValue) queryParams.Add($"pageSize={pageSize.Value}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"walletSets{queryString}");
            if (!string.IsNullOrEmpty(xRequestId))
                requestMessage.Headers.Add("X-Request-Id", xRequestId);

            var response = await _httpClient.SendAsync(requestMessage);
            await HandleResponseAsync(response);

            var content = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<WalletSetsResponseWrapper>(content).Data;
            return responseData?.WalletSets?.ToArray() ?? Array.Empty<WalletSetResponse>();
        }

        /// <summary>
        /// Retrieves a specific wallet set by ID asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the wallet set to retrieve.</param>
        /// <param name="xRequestId">Optional request ID for tracking the request.</param>
        /// <returns>A <see cref="Task{WalletSetResponse}"/> representing the asynchronous operation, containing the retrieved wallet set response.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is null or empty.</exception>
        /// <exception cref="CircleApiException">Thrown when the API request fails.</exception>
        public async Task<WalletSetResponse> GetWalletSetAsync(string id, string? xRequestId = null)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentException("Wallet set ID cannot be null or empty.", nameof(id));

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"walletSets/{Uri.EscapeDataString(id)}");
            if (!string.IsNullOrEmpty(xRequestId))
                requestMessage.Headers.Add("X-Request-Id", xRequestId);

            var response = await _httpClient.SendAsync(requestMessage);
            await HandleResponseAsync(response);

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<WalletSetResponseWrapper>(content).Data;
        }

        /// <summary>
        /// Updates an existing wallet set asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the wallet set to update.</param>
        /// <param name="name">The new name for the wallet set.</param>
        /// <param name="xRequestId">Optional request ID for tracking the request.</param>
        /// <returns>A <see cref="Task{WalletSetResponse}"/> representing the asynchronous operation, containing the updated wallet set response.</returns>
        /// <exception cref="ArgumentException">Thrown when id or name is null or empty.</exception>
        /// <exception cref="CircleApiException">Thrown when the API request fails.</exception>
        public async Task<WalletSetResponse> UpdateWalletSetAsync(string id, string name, string? xRequestId = null)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentException("Wallet set ID cannot be null or empty.", nameof(id));
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Name cannot be null or empty.", nameof(name));

            var request = new UpdateWalletSetRequest { Name = name };
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"developer/walletSets/{Uri.EscapeDataString(id)}")
            {
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
            };
            if (!string.IsNullOrEmpty(xRequestId))
                requestMessage.Headers.Add("X-Request-Id", xRequestId);

            var response = await _httpClient.SendAsync(requestMessage);
            await HandleResponseAsync(response);

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<WalletSetResponseWrapper>(content).Data;
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