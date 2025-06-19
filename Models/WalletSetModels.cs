using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CircleDeveloperControlledWalletSDK.Models
{
    /// <summary>
    /// Represents a request to create a new wallet set.
    /// </summary>
    public class CreateWalletSetRequest
    {
        [JsonProperty("entitySecretCiphertext")]
        public string EntitySecretCiphertext { get; set; }

        [JsonProperty("idempotencyKey")]
        public string IdempotencyKey { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    /// <summary>
    /// Represents a request to update an existing wallet set.
    /// </summary>
    public class UpdateWalletSetRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    /// <summary>
    /// Represents the response containing wallet set information.
    /// </summary>
    public class WalletSetResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("createDate")]
        public DateTime CreateDate { get; set; }

        [JsonProperty("updateDate")]
        public DateTime UpdateDate { get; set; }

        [JsonProperty("custodyType")]
        public string CustodyType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    /// <summary>
    /// Represents a response containing a collection of wallet sets.
    /// </summary>
    public class WalletSetsResponse
    {
        [JsonProperty("walletSets")]
        public List<WalletSetResponse> WalletSets { get; set; }
    }

    /// <summary>
    /// Represents a wrapper class for the wallet set response data.
    /// </summary>
    public class WalletSetResponseWrapper
    {
        [JsonProperty("data")]
        public WalletSetResponse Data { get; set; }
    }

    /// <summary>
    /// Represents a wrapper class for the wallet sets response data.
    /// </summary>
    public class WalletSetsResponseWrapper
    {
        [JsonProperty("data")]
        public WalletSetsResponse Data { get; set; }
    }
}