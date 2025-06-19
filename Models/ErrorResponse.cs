using Newtonsoft.Json;

namespace CircleDeveloperControlledWalletSDK.Models
{
    /// <summary>
    /// Represents an error response from the Circle API.
    /// </summary>
    public class ErrorResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}