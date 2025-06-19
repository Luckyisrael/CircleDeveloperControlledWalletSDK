using Newtonsoft.Json;

namespace CircleDeveloperControlledWalletSDK.Models
{
    /// <summary>
    /// Request model for registering an entity secret.
    /// </summary>
    public class RegisterEntitySecretRequest
    {
        [JsonProperty("entitySecret")]
        public string? EntitySecret { get; set; }

        [JsonProperty("idempotencyKey")]
        public string? IdempotencyKey { get; set; }
    }

    /// <summary>
    /// Response model for registering an entity secret.
    /// </summary>
    public class RegisterEntitySecretResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }
    }

    /// <summary>
    /// Wrapper class for the register entity secret response.
    /// </summary>
    public class RegisterEntitySecretResponseWrapper
    {
        [JsonProperty("data")]
        public RegisterEntitySecretResponse Data { get; set; }
    }

    /// <summary>
    /// Response model containing the public key.
    /// </summary>
    public class PublicKeyResponse
    {
        [JsonProperty("publicKey")]
        public string PublicKey { get; set; }
    }

    /// <summary>
    /// Wrapper class for the public key response.
    /// </summary>
    public class PublicKeyResponseWrapper
    {
        [JsonProperty("data")]
        public PublicKeyResponse Data { get; set; }
    }
}