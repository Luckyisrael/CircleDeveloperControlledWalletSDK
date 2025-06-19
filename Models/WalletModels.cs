using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CircleDeveloperControlledWalletSDK.Models
{
    /// <summary>
    /// Represents a request to create a new wallet.
    /// </summary>
    public class CreateWalletRequest
    {
        [JsonProperty("walletSetId")]
        public string WalletSetId { get; set; }

        [JsonProperty("blockchains")]
        public List<string> Blockchains { get; set; }

        [JsonProperty("entitySecretCiphertext")]
        public string EntitySecretCiphertext { get; set; }

        [JsonProperty("idempotencyKey")]
        public string IdempotencyKey { get; set; }

        [JsonProperty("accountType")]
        public string AccountType { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("metadata")]
        public List<WalletMetadata> Metadata { get; set; }
    }

    /// <summary>
    /// Represents metadata associated with a wallet, including name and reference ID.
    /// </summary>
    public class WalletMetadata
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("refId")]
        public string RefId { get; set; }
    }

    /// <summary>
    /// Represents a request to update an existing wallet's metadata.
    /// </summary>
    public class UpdateWalletRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("refId")]
        public string RefId { get; set; }
    }

    /// <summary>
    /// Represents a response containing wallet information from the Circle API.
    /// </summary>
    public class WalletResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("blockchain")]
        public string Blockchain { get; set; }

        [JsonProperty("createDate")]
        public DateTime CreateDate { get; set; }

        [JsonProperty("updateDate")]
        public DateTime UpdateDate { get; set; }

        [JsonProperty("custodyType")]
        public string CustodyType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("refId")]
        public string RefId { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("walletSetId")]
        public string WalletSetId { get; set; }

        [JsonProperty("initialPublicKey")]
        public string InitialPublicKey { get; set; }

        [JsonProperty("accountType")]
        public string AccountType { get; set; }
    }

    /// <summary>
    /// Represents a response containing a list of wallets from the Circle API.
    /// </summary>
    public class WalletsResponse
    {
        [JsonProperty("wallets")]
        public List<WalletResponse> Wallets { get; set; }
    }

    /// <summary>
    /// Represents a wrapper class for a single wallet response from the Circle API.
    /// </summary>
    public class WalletResponseWrapper
    {
        [JsonProperty("data")]
        public WalletResponse Data { get; set; }
    }

    /// <summary>
    /// Represents a wrapper class for a list of wallets response from the Circle API.
    /// </summary>
    public class WalletsResponseWrapper
    {
        [JsonProperty("data")]
        public WalletsResponse Data { get; set; }
    }

    /// <summary>
    /// Represents a request to derive a new wallet from an existing wallet set.
    /// </summary>
    public class DeriveWalletRequest
    {
        [JsonProperty("metadata")]
        public WalletMetadata Metadata { get; set; }
    }

    /// <summary>
    /// Represents a token with its associated properties like name, standard, blockchain details, and other metadata.
    /// </summary>
    public class Token
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
    /// Represents a token balance with amount, token details, and update timestamp.
    /// </summary>
    public class TokenBalance
    {
        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("token")]
        public Token Token { get; set; }

        [JsonProperty("updateDate")]
        public DateTime UpdateDate { get; set; }
    }

    /// <summary>
    /// Represents a response containing wallet information along with token balance details from the Circle API.
    /// </summary>
    public class WalletWithBalancesResponse : WalletResponse
    {
        [JsonProperty("tokenBalances")]
        public List<TokenBalance> TokenBalances { get; set; }
    }

    /// <summary>
    /// Represents a response containing a list of wallets with their associated token balance details from the Circle API.
    /// </summary>
    public class WalletsWithBalancesResponse
    {
        [JsonProperty("wallets")]
        public List<WalletWithBalancesResponse> Wallets { get; set; }
    }

    /// <summary>
    /// Represents a wrapper class for a list of wallets with balances response from the Circle API.
    /// </summary>
    public class WalletsWithBalancesResponseWrapper
    {
        [JsonProperty("data")]
        public WalletsWithBalancesResponse Data { get; set; }
    }

    /// <summary>
    /// Represents an NFT (Non-Fungible Token) with its associated properties including amount, metadata, token ID and token details.
    /// </summary>
    public class Nft
    {
        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("metadata")]
        public string Metadata { get; set; }

        [JsonProperty("nftTokenId")]
        public string NftTokenId { get; set; }

        [JsonProperty("token")]
        public Token Token { get; set; }

        [JsonProperty("updateDate")]
        public DateTime UpdateDate { get; set; }
    }

    /// <summary>
    /// Represents a response containing a list of NFTs from the Circle API.
    /// </summary>
    public class NftsResponse
    {
        [JsonProperty("nfts")]
        public List<Nft> Nfts { get; set; }
    }

    /// <summary>
    /// Represents a wrapper class for a list of NFTs response from the Circle API.
    /// </summary>
    public class NftsResponseWrapper
    {
        [JsonProperty("data")]
        public NftsResponse Data { get; set; }
    }

    /// <summary>
    /// Represents a request to sign a message using a wallet.
    /// </summary>
    public class SignMessageRequest
    {
        [JsonProperty("walletId")]
        public string WalletId { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("entitySecretCiphertext")]
        public string EntitySecretCiphertext { get; set; }

        [JsonProperty("encodedByHex")]
        public bool EncodedByHex { get; set; }

        [JsonProperty("memo")]
        public string Memo { get; set; }
    }

    /// <summary>
    /// Represents a response containing the signature generated from signing a message using a wallet.
    /// </summary>
    public class SignMessageResponse
    {
        [JsonProperty("signature")]
        public string Signature { get; set; }
    }

    /// <summary>
    /// Represents a wrapper class for a sign message response from the Circle API.
    /// </summary>
    public class SignMessageResponseWrapper
    {
        [JsonProperty("data")]
        public SignMessageResponse Data { get; set; }
    }

    /// <summary>
    /// Represents a request to sign typed data using a wallet.
    /// </summary>
    public class SignTypedDataRequest
    {
        [JsonProperty("walletId")]
        public string WalletId { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("entitySecretCiphertext")]
        public string EntitySecretCiphertext { get; set; }

        [JsonProperty("memo")]
        public string Memo { get; set; }
    }

    /// <summary>
    /// Represents a response containing the signature generated from signing typed data using a wallet.
    /// </summary>
    public class SignTypedDataResponse
    {
        [JsonProperty("signature")]
        public string Signature { get; set; }
    }

    /// <summary>
    /// Represents a wrapper class for a sign typed data response from the Circle API.
    /// </summary>
    public class SignTypedDataResponseWrapper
    {
        [JsonProperty("data")]
        public SignTypedDataResponse Data { get; set; }
    }

    /// <summary>
    /// Represents a request to sign a blockchain transaction using a wallet.
    /// </summary>
    public class SignTransactionRequest
    {
        [JsonProperty("walletId")]
        public string WalletId { get; set; }

        [JsonProperty("entitySecretCiphertext")]
        public string EntitySecretCiphertext { get; set; }

        [JsonProperty("rawTransaction")]
        public string RawTransaction { get; set; }

        [JsonProperty("transaction")]
        public string Transaction { get; set; }

        [JsonProperty("memo")]
        public string Memo { get; set; }
    }

    /// <summary>
    /// Represents a response containing the signature and transaction details after signing a blockchain transaction.
    /// </summary>
    public class SignTransactionResponse
    {
        [JsonProperty("signature")]
        public string Signature { get; set; }

        [JsonProperty("signedTransaction")]
        public string SignedTransaction { get; set; }

        [JsonProperty("txHash")]
        public string TxHash { get; set; }
    }

    /// <summary>
    /// Represents a wrapper class for a sign transaction response from the Circle API.
    /// </summary>
    public class SignTransactionResponseWrapper
    {
        [JsonProperty("data")]
        public SignTransactionResponse Data { get; set; }
    }

    /// <summary>
    /// Represents a request to sign a delegate action using a wallet.
    /// </summary>
    public class SignDelegateActionRequest
    {
        [JsonProperty("walletId")]
        public string WalletId { get; set; }

        [JsonProperty("unsignedDelegateAction")]
        public string UnsignedDelegateAction { get; set; }

        [JsonProperty("entitySecretCiphertext")]
        public string EntitySecretCiphertext { get; set; }
    }

    /// <summary>
    /// Represents a response containing the signature and signed delegate action after signing a delegate action using a wallet.
    /// </summary>
    public class SignDelegateActionResponse
    {
        [JsonProperty("signature")]
        public string Signature { get; set; }

        [JsonProperty("signedDelegateAction")]
        public string SignedDelegateAction { get; set; }
    }

    /// <summary>
    /// Represents a wrapper class for a sign delegate action response from the Circle API.
    /// </summary>
    public class SignDelegateActionResponseWrapper
    {
        [JsonProperty("data")]
        public SignDelegateActionResponse Data { get; set; }
    }
}