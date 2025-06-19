using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CircleDeveloperControlledWalletSDK.Models
{
    /// <summary>
    /// Represents estimated transaction fee information including gas limits, prices and various fee components.
    /// </summary>
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

    /// <summary>
    /// Represents the response containing estimated transaction fee information with different fee levels (high, medium, low) and gas limits.
    /// </summary>
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

    /// <summary>
    /// Represents a wrapper class for the EstimateFeeResponse that contains the transaction fee estimation data.
    /// </summary>
    public class EstimateFeeResponseWrapper
    {
        [JsonProperty("data")]
        public EstimateFeeResponse Data { get; set; }
    }

    /// <summary>
    /// Represents a reason for transaction screening, including source information, risk scores, and categories.
    /// </summary>
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

    /// <summary>
    /// Represents the evaluation results of a transaction screening, including rule name, actions taken, screening date and reasons.
    /// </summary>
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

    /// <summary>
    /// Represents a response containing transaction details including identifiers, amounts, blockchain information, fees, and status.
    /// </summary>
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

    /// <summary>
    /// Represents a collection of transaction responses returned from the API.
    /// </summary>
    public class TransactionsResponse
    {
        [JsonProperty("transactions")]
        public List<TransactionResponse> Transactions { get; set; }
    }

    /// <summary>
    /// Represents a wrapper class for the TransactionsResponse that contains a collection of transaction data.
    /// </summary>
    public class TransactionsResponseWrapper
    {
        [JsonProperty("data")]
        public TransactionsResponse Data { get; set; }
    }

    /// <summary>
    /// Represents a wrapper class for the TransactionResponse that contains a single transaction's data.
    /// </summary>
    public class TransactionResponseWrapper
    {
        [JsonProperty("data")]
        public TransactionResponse Data { get; set; }
    }

    /// <summary>
    /// Represents a request to create a new transfer transaction, including wallet details, destination, amounts and fee settings.
    /// </summary>
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

    /// <summary>
    /// Represents a response from creating a transfer transaction, containing the transaction ID and state.
    /// </summary>
    public class CreateTransferResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
    }

    /// <summary>
    /// Represents a wrapper class for the CreateTransferResponse that contains the transfer transaction creation data.
    /// </summary>
    public class CreateTransferResponseWrapper
    {
        [JsonProperty("data")]
        public CreateTransferResponse Data { get; set; }
    }

    /// <summary>
    /// Represents a request to validate a blockchain address, containing the blockchain type and address to validate.
    /// </summary>
    public class ValidateAddressRequest
    {
        [JsonProperty("blockchain")]
        public string Blockchain { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }
    }

    /// <summary>
    /// Represents a response from validating a blockchain address, containing a boolean indicating if the address is valid.
    /// </summary>
    public class ValidateAddressResponse
    {
        [JsonProperty("isValid")]
        public bool IsValid { get; set; }
    }

    /// <summary>
    /// Represents a wrapper class for the ValidateAddressResponse that contains the address validation result.
    /// </summary>
    public class ValidateAddressResponseWrapper
    {
        [JsonProperty("data")]
        public ValidateAddressResponse Data { get; set; }
    }

    /// <summary>
    /// Represents a request to estimate the fee for executing a smart contract, including contract details, blockchain information, and execution parameters.
    /// </summary>
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

    /// <summary>
    /// Represents a request to estimate the fee for a transfer transaction, including destination address, amounts, and blockchain details.
    /// </summary>
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

    /// <summary>
    /// Represents a request to execute a smart contract, including wallet details, contract information, and execution parameters.
    /// </summary>
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

    /// <summary>
    /// Represents a response from executing a smart contract, containing the transaction ID and state.
    /// </summary>
    public class CreateContractExecutionResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
    }

    /// <summary>
    /// Represents a wrapper class for the CreateContractExecutionResponse that contains the contract execution result data.
    /// </summary>
    public class CreateContractExecutionResponseWrapper
    {
        [JsonProperty("data")]
        public CreateContractExecutionResponse Data { get; set; }
    }

    /// <summary>
    /// Represents a request to upgrade a wallet, including wallet ID, security parameters, and fee settings.
    /// </summary>
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

    /// <summary>
    /// Represents a response from upgrading a wallet, containing the transaction ID and state.
    /// </summary>
    public class CreateWalletUpgradeResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
    }

    /// <summary>
    /// Represents a wrapper class for the CreateWalletUpgradeResponse that contains the wallet upgrade result data.
    /// </summary>
    public class CreateWalletUpgradeResponseWrapper
    {
        [JsonProperty("data")]
        public CreateWalletUpgradeResponse Data { get; set; }
    }

    /// <summary>
    /// Represents a request to cancel a pending transaction, including security parameters and idempotency key.
    /// </summary>
    public class CancelTransactionRequest
    {
        [JsonProperty("entitySecretCiphertext")]
        public string EntitySecretCiphertext { get; set; }

        [JsonProperty("idempotencyKey")]
        public string IdempotencyKey { get; set; }
    }

    /// <summary>
    /// Represents a response from canceling a transaction, containing the transaction ID and state.
    /// </summary>
    public class CancelTransactionResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
    }

    /// <summary>
    /// Represents a wrapper class for the CancelTransactionResponse that contains the transaction cancellation result data.
    /// </summary>
    public class CancelTransactionResponseWrapper
    {
        [JsonProperty("data")]
        public CancelTransactionResponse Data { get; set; }
    }

    /// <summary>
    /// Represents a request to accelerate a pending transaction by increasing its gas fee, including security parameters and idempotency key.
    /// </summary>
    public class AccelerateTransactionRequest
    {
        [JsonProperty("entitySecretCiphertext")]
        public string EntitySecretCiphertext { get; set; }

        [JsonProperty("idempotencyKey")]
        public string IdempotencyKey { get; set; }
    }

    /// <summary>
    /// Represents a response from accelerating a transaction, containing the transaction ID.
    /// </summary>
    public class AccelerateTransactionResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    /// <summary>
    /// Represents a wrapper class for the AccelerateTransactionResponse that contains the transaction acceleration result data.
    /// </summary>
    public class AccelerateTransactionResponseWrapper
    {
        [JsonProperty("data")]
        public AccelerateTransactionResponse Data { get; set; }
    }
}