using Newtonsoft.Json;
using System;

namespace CircleDeveloperControlledWalletSDK.Models
{
    /// <summary>
    /// Represents a response containing token information from the Circle API.
    /// </summary>
    public class TokenResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("standard")]
        public string Standard { get; set; }

        [JsonProperty("blockchain")]
        public string Blockchain { get; set; }

        [JsonProperty("decimals")]
        public int Decimals { get; set; }

        [JsonProperty("isNative")]
        public bool IsNative { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("tokenAddress")]
        public string TokenAddress { get; set; }

        [JsonProperty("updateDate")]
        public DateTime UpdateDate { get; set; }

        [JsonProperty("createDate")]
        public DateTime CreateDate { get; set; }
    }

    /// <summary>
    /// Wrapper class for token response data from the Circle API.
    /// </summary>
    public class TokenResponseWrapper
    {
        [JsonProperty("data")]
        public TokenResponse Data { get; set; }
    }
}