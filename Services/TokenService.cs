using System;
using System.Net.Http;
using System.Threading.Tasks;
using CircleDeveloperControlledWalletSDK.Exceptions;
using CircleDeveloperControlledWalletSDK.Models;
using Newtonsoft.Json;

namespace CircleDeveloperControlledWalletSDK.Services
{
    /// <summary>
    /// Service for managing token-related operations and API calls.
    /// </summary>
    public class TokenService
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenService"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client used for making API requests.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="httpClient"/> is null.</exception>
        public TokenService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Fetches details of a specific token by its ID.
        /// </summary>
        /// <param name="id">The token ID (UUID).</param>
        /// <param name="xRequestId">Optional request identifier for Circle Support.</param>
        /// <returns>A <see cref="TokenResponse"/> containing the token details.</returns>
        /// <exception cref="ArgumentException">Thrown when the token ID is invalid.</exception>
        /// <exception cref="CircleApiException">Thrown when the API request fails.</exception>
        public async Task<TokenResponse> GetTokenDetailsAsync(string id, string xRequestId = null)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Token ID cannot be null or empty.", nameof(id));

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"tokens/{Uri.EscapeDataString(id)}");
            if (!string.IsNullOrEmpty(xRequestId))
                requestMessage.Headers.Add("X-Request-Id", xRequestId);

            var response = await _httpClient.SendAsync(requestMessage);
            await HandleResponseAsync(response);

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TokenResponseWrapper>(content).Data;
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