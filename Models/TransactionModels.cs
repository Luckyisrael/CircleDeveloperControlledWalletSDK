using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CircleDeveloperControlledWalletSDK.Models
{
    public class EstimatedFee
    {
        [JsonProperty("gasLimit")]
        public string GasLimit { get; set; }

        [JsonProperty("gasPrice")]
        public string GasPrice { get; set; }

        [JsonProperty("maxFee")]
        public string MaxFee { get; set; }

        [JsonProperty("priorityFee")]
        public string PriorityFee { get; set; }

        [JsonProperty("baseFee")]
        public string BaseFee { get; set; }

        [JsonProperty("networkFee")]
        public string NetworkFee { get; set; }
    }

    public class EstimateFeeResponse
    {
        [JsonProperty("high")]
        public EstimatedFee High { get; set; }

        [JsonProperty("medium")]
        public EstimatedFee Medium { get; set; }

        [JsonProperty("low")]
        public EstimatedFee Low { get; set; }

        [JsonProperty("callGasLimit")]
        public string CallGasLimit { get; set; }

        [JsonProperty("verificationGasLimit")]
        public string VerificationGasLimit { get; set; }

        [JsonProperty("preVerificationGas")]
        public string PreVerificationGas { get; set; }
    }

    public class EstimateFeeResponseWrapper
    {
        [JsonProperty("data")]
        public EstimateFeeResponse Data { get; set; }
    }

    public class ScreeningReason
    {
        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("sourceValue")]
        public string SourceValue { get; set; }

        [JsonProperty("riskScore")]
        public string RiskScore { get; set; }

        [JsonProperty("riskCategories")]
        public List<string> RiskCategories { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class TransactionScreeningEvaluation
    {
        [JsonProperty("ruleName")]
        public string RuleName { get; set; }

        [JsonProperty("actions")]
        public List<string> Actions { get; set; }

        [JsonProperty("screeningDate")]
        public DateTime ScreeningDate { get; set; }

        [JsonProperty("reasons")]
        public List<ScreeningReason> Reasons { get; set; }
    }

    public class TransactionResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("abiFunctionSignature")]
        public string AbiFunctionSignature { get; set; }

        [JsonProperty("abiParameters")]
        public List<string> AbiParameters { get; set; }

        [JsonProperty("amounts")]
        public List<string> Amounts { get; set; }

        [JsonProperty("amountInUSD")]
        public string AmountInUSD { get; set; }

        [JsonProperty("blockHash")]
        public string BlockHash { get; set; }

        [JsonProperty("blockHeight")]
        public long BlockHeight { get; set; }

        [JsonProperty("blockchain")]
        public string Blockchain { get; set; }

        [JsonProperty("contractAddress")]
        public string ContractAddress { get; set; }

        [JsonProperty("createDate")]
        public DateTime CreateDate { get; set; }

        [JsonProperty("custodyType")]
        public string CustodyType { get; set; }

        [JsonProperty("destinationAddress")]
        public string DestinationAddress { get; set; }

        [JsonProperty("errorReason")]
        public string ErrorReason { get; set; }

        [JsonProperty("errorDetails")]
        public string ErrorDetails { get; set; }

        [JsonProperty("estimatedFee")]
        public EstimatedFee EstimatedFee { get; set; }

        [JsonProperty("feeLevel")]
        public string FeeLevel { get; set; }

        [JsonProperty("firstConfirmDate")]
        public DateTime FirstConfirmDate { get; set; }

        [JsonProperty("networkFee")]
        public string NetworkFee { get; set; }

        [JsonProperty("networkFeeInUSD")]
        public string NetworkFeeInUSD { get; set; }

        [JsonProperty("nfts")]
        public List<string> Nfts { get; set; }

        [JsonProperty("operation")]
        public string Operation { get; set; }

        [JsonProperty("refId")]
        public string RefId { get; set; }

        [JsonProperty("sourceAddress")]
        public string SourceAddress { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("tokenId")]
        public string TokenId { get; set; }

        [JsonProperty("transactionType")]
        public string TransactionType { get; set; }

        [JsonProperty("txHash")]
        public string TxHash { get; set; }

        [JsonProperty("updateDate")]
        public DateTime UpdateDate { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("walletId")]
        public string WalletId { get; set; }

        [JsonProperty("transactionScreeningEvaluation")]
        public TransactionScreeningEvaluation TransactionScreeningEvaluation { get; set; }
    }

    public class TransactionsResponse
    {
        [JsonProperty("transactions")]
        public List<TransactionResponse> Transactions { get; set; }
    }

    public class TransactionsResponseWrapper
    {
        [JsonProperty("data")]
        public TransactionsResponse Data { get; set; }
    }

    public class TransactionResponseWrapper
    {
        [JsonProperty("data")]
        public TransactionResponse Data { get; set; }
    }

    public class CreateTransferRequest
    {
        [JsonProperty("walletId")]
        public string WalletId { get; set; }

        [JsonProperty("entitySecretCiphertext")]
        public string EntitySecretCiphertext { get; set; }

        [JsonProperty("destinationAddress")]
        public string DestinationAddress { get; set; }

        [JsonProperty("idempotencyKey")]
        public string IdempotencyKey { get; set; }

        [JsonProperty("amounts")]
        public string[] Amounts { get; set; }

        [JsonProperty("feeLevel")]
        public string FeeLevel { get; set; }

        [JsonProperty("gasLimit")]
        public string GasLimit { get; set; }

        [JsonProperty("gasPrice")]
        public string GasPrice { get; set; }

        [JsonProperty("maxFee")]
        public string MaxFee { get; set; }

        [JsonProperty("priorityFee")]
        public string PriorityFee { get; set; }

        [JsonProperty("nftTokenIds")]
        public string[] NftTokenIds { get; set; }

        [JsonProperty("refId")]
        public string RefId { get; set; }

        [JsonProperty("tokenId")]
        public string TokenId { get; set; }

        [JsonProperty("tokenAddress")]
        public string TokenAddress { get; set; }

        [JsonProperty("blockchain")]
        public string Blockchain { get; set; }
    }

    public class CreateTransferResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
    }

    public class CreateTransferResponseWrapper
    {
        [JsonProperty("data")]
        public CreateTransferResponse Data { get; set; }
    }

    public class ValidateAddressRequest
    {
        [JsonProperty("blockchain")]
        public string Blockchain { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }
    }

    public class ValidateAddressResponse
    {
        [JsonProperty("isValid")]
        public bool IsValid { get; set; }
    }

    public class ValidateAddressResponseWrapper
    {
        [JsonProperty("data")]
        public ValidateAddressResponse Data { get; set; }
    }

    public class EstimateContractExecutionFeeRequest
    {
        [JsonProperty("contractAddress")]
        public string ContractAddress { get; set; }

        [JsonProperty("blockchain")]
        public string Blockchain { get; set; }

        [JsonProperty("sourceAddress")]
        public string SourceAddress { get; set; }

        [JsonProperty("walletId")]
        public string WalletId { get; set; }

        [JsonProperty("abiFunctionSignature")]
        public string AbiFunctionSignature { get; set; }

        [JsonProperty("abiParameters")]
        public object[] AbiParameters { get; set; }

        [JsonProperty("callData")]
        public string CallData { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }
    }

    public class EstimateTransferFeeRequest
    {
        [JsonProperty("destinationAddress")]
        public string DestinationAddress { get; set; }

        [JsonProperty("amounts")]
        public string[] Amounts { get; set; }

        [JsonProperty("nftTokenIds")]
        public string[] NftTokenIds { get; set; }

        [JsonProperty("sourceAddress")]
        public string SourceAddress { get; set; }

        [JsonProperty("tokenId")]
        public string TokenId { get; set; }

        [JsonProperty("tokenAddress")]
        public string TokenAddress { get; set; }

        [JsonProperty("blockchain")]
        public string Blockchain { get; set; }

        [JsonProperty("walletId")]
        public string WalletId { get; set; }
    }

    public class CreateContractExecutionRequest
    {
        [JsonProperty("walletId")]
        public string WalletId { get; set; }

        [JsonProperty("entitySecretCiphertext")]
        public string EntitySecretCiphertext { get; set; }

        [JsonProperty("contractAddress")]
        public string ContractAddress { get; set; }

        [JsonProperty("idempotencyKey")]
        public string IdempotencyKey { get; set; }

        [JsonProperty("abiFunctionSignature")]
        public string AbiFunctionSignature { get; set; }

        [JsonProperty("abiParameters")]
        public object[] AbiParameters { get; set; }

        [JsonProperty("callData")]
        public string CallData { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("feeLevel")]
        public string FeeLevel { get; set; }

        [JsonProperty("gasLimit")]
        public string GasLimit { get; set; }

        [JsonProperty("gasPrice")]
        public string GasPrice { get; set; }

        [JsonProperty("maxFee")]
        public string MaxFee { get; set; }

        [JsonProperty("priorityFee")]
        public string PriorityFee { get; set; }

        [JsonProperty("refId")]
        public string RefId { get; set; }
    }

    public class CreateContractExecutionResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
    }

    public class CreateContractExecutionResponseWrapper
    {
        [JsonProperty("data")]
        public CreateContractExecutionResponse Data { get; set; }
    }

    public class CreateWalletUpgradeRequest
    {
        [JsonProperty("walletId")]
        public string WalletId { get; set; }

        [JsonProperty("entitySecretCiphertext")]
        public string EntitySecretCiphertext { get; set; }

        [JsonProperty("newScaCore")]
        public string NewScaCore { get; set; }

        [JsonProperty("idempotencyKey")]
        public string IdempotencyKey { get; set; }

        [JsonProperty("feeLevel")]
        public string FeeLevel { get; set; }

        [JsonProperty("gasLimit")]
        public string GasLimit { get; set; }

        [JsonProperty("gasPrice")]
        public string GasPrice { get; set; }

        [JsonProperty("maxFee")]
        public string MaxFee { get; set; }

        [JsonProperty("priorityFee")]
        public string PriorityFee { get; set; }

        [JsonProperty("refId")]
        public string RefId { get; set; }
    }

    public class CreateWalletUpgradeResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
    }

    public class CreateWalletUpgradeResponseWrapper
    {
        [JsonProperty("data")]
        public CreateWalletUpgradeResponse Data { get; set; }
    }

    public class CancelTransactionRequest
    {
        [JsonProperty("entitySecretCiphertext")]
        public string EntitySecretCiphertext { get; set; }

        [JsonProperty("idempotencyKey")]
        public string IdempotencyKey { get; set; }
    }

    public class CancelTransactionResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
    }

    public class CancelTransactionResponseWrapper
    {
        [JsonProperty("data")]
        public CancelTransactionResponse Data { get; set; }
    }

    public class AccelerateTransactionRequest
    {
        [JsonProperty("entitySecretCiphertext")]
        public string EntitySecretCiphertext { get; set; }

        [JsonProperty("idempotencyKey")]
        public string IdempotencyKey { get; set; }
    }

    public class AccelerateTransactionResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class AccelerateTransactionResponseWrapper
    {
        [JsonProperty("data")]
        public AccelerateTransactionResponse Data { get; set; }
    }
}