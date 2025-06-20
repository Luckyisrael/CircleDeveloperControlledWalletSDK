# CircleDeveloperControlledWalletSDK

A minimal viable C# SDK for Circle's Developer-Controlled Wallets API, designed to support Entity Secret management and wallet set operations (create, get all, get by ID, update). This SDK is built for simplicity, robustness, and extensibility, targeting .NET 9.0.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Usage](#usage)
  - [Initializing the Client](#initializing-the-client)
  - [Entity Secret Management](#entity-secret-management)
  - [Wallet Set Operations](#wallet-set-operations)
  - [Wallet Operations](#wallet-operations)
  - [Transaction Operations](#transaction-operations)
  - [Token Operations](#token-operations)
- [Testing](#testing)
- [Error Handling](#error-handling)
- [Limitations](#limitations)
- [Contributing](#contributing)
- [License](#license)

## Overview

The CircleDeveloperControlledWalletSDK simplifies interactions with the Circle Programmable Wallets API in the sandbox environment. It supports operations such as creating and managing wallets, executing transactions (transfers, contract executions, wallet upgrades), canceling or accelerating transactions, and retrieving token details. The SDK uses a clean, modular architecture with strongly-typed models for requests and responses.

## Features

**Entity Secret Management:**
- Generate and register Entity Secrets with recovery file support.
- Generate Entity Secret Ciphertext for secure API requests.

**Wallet Set Operations:**
- Create wallet sets with customizable names.

**Wallet Operations:**
- Create Externally Owned Account (EOA) wallets on supported blockchains (e.g., MATIC-AMOY).
- Attach metadata to wallets for identification.

**Transaction Operations:**
- List and retrieve transactions with flexible filters (blockchain, wallet ID, transaction type, etc.).
- Create transfer transactions for digital assets.
- Validate blockchain addresses.
- Estimate gas fees for contract execution and transfer transactions.
- Create contract execution transactions with ABI parameters or raw call data.
- Create wallet upgrade transactions to specific SCA versions.
- Cancel pending transactions (best-effort).
- Accelerate transactions with additional gas fees.

**Token Operations:**
- Retrieve details of specific tokens by ID (e.g., ERC20 tokens on MATIC-AMOY).

**Error Handling:**
- Comprehensive validation for input parameters.
- Detailed exception handling with API error messages and HTTP status codes.

**Testing:**
- A test application demonstrating all SDK functionalities with sample payloads and outputs.

## Prerequisites

- **NET SDK**: Version 9.0 or later.
- **Circle Sandbox API Key**: Obtain from the Circle Developer Console (format: `TEST_API_KEY:<key>:<secret>`).
- **Development Environment**: Visual Studio, VS Code, or any .NET-compatible IDE.
- **Dependencies** (auto-installed via NuGet):
  - Newtonsoft.Json (v13.0.3)
  - System.Security.Cryptography.Algorithms (v4.3.1)

## Installation

1. **Clone or download the repository:**
   ```bash
   git clone <repository-url>
   cd CircleDeveloperControlledWalletSDKSolution
   ```

2. **Restore NuGet packages:**
   ```bash
   dotnet restore
   ```

3. **Build the solution:**
   ```bash
   dotnet build
   ```

## Configuration

**API Key:**
- Obtain a sandbox API key from the Circle Developer Console.
- Update the test application (`CircleDeveloperControlledWalletSDKTest.cs`) with your API key:
  ```csharp
  const string apiKey = "TEST_API_KEY:<your-key>:<your-secret>";
  ```

**Wallet Set, Wallet, and Token IDs:**
- Replace placeholders in `CircleDeveloperControlledWalletSDKTest.cs`:
  - `walletSetId`: Use a valid wallet set ID from `CreateWalletSetAsync`.
  - `testWalletId`: Use a valid wallet ID for transaction tests.
  - `testTokenId`: Use a valid token ID for token tests.
  - Transaction IDs for `GetTransactionAsync`, `CancelTransactionAsync`, and `AccelerateTransactionAsync`.

**Recovery File Path:**
- Ensure write permissions for the directory where the Entity Secret recovery file is saved (default: `entity_secret_recovery.json`).

## Usage

### Initializing the Client

Create a `CircleClient` instance with your API key. The client exposes services for wallets, wallet sets, entity secrets, transactions, and tokens.

```csharp
using CircleDeveloperControlledWalletSDK;

const string apiKey = "TEST_API_KEY:<your-key>:<your-secret>";
using var client = new CircleClient(apiKey);
```

### Entity Secret Management

**Generate and Register Entity Secret:**
Generate a 32-byte hex Entity Secret and register it with Circle, saving a recovery file.

```csharp
var entitySecret = client.EntitySecrets.GenerateEntitySecret();
var registerResponse = await client.EntitySecrets.RegisterEntitySecretAsync(entitySecret, "entity_secret_recovery.json");
Console.WriteLine($"Registration Status: {registerResponse.Status}");
Console.WriteLine($"Recovery File: entity_secret_recovery.json");
```

**Output:** Confirmation of registration status (SUCCESS) and recovery file path.

**⚠️ Warning:** This SDK doesn't support Entity Secret Cipher Text Registration yet. (We are sorting that out) for now, you manaully register your secret on Circle's Dashboard.

**Notes:** The Entity Secret is required for operations like wallet creation and transactions. Store the recovery file securely.

**Generate Entity Secret Ciphertext:**
Generate ciphertext for secure API requests (handled internally by other methods).

```csharp
var ciphertext = await client.EntitySecrets.GenerateEntitySecretCiphertextAsync(entitySecret);
Console.WriteLine($"Ciphertext: {ciphertext}");
```

### Wallet Set Operations

Create a wallet set to group wallets:

```csharp
var walletSet = await client.WalletSets.CreateWalletSetAsync("TestWalletSet", entitySecret);
Console.WriteLine($"Wallet Set ID: {walletSet.Id}, Name: {walletSet.Name}, Created: {walletSet.CreateDate}");
```

**Output:** Wallet set ID, name, and creation date.

**Notes:** Use the returned `Id` as `walletSetId` for wallet creation.

### Wallet Operations

Create an Externally Owned Account (EOA) wallet on a supported blockchain (e.g., MATIC-AMOY):

```csharp
var walletsResponse = await client.Wallets.CreateWalletAsync(
    walletSetId: "<wallet-set-id>",
    blockchains: new[] { "MATIC-AMOY" },
    entitySecret: entitySecret,
    idempotencyKey: Guid.NewGuid().ToString(),
    accountType: "EOA",
    count: 1,
    metadata: new[] { new WalletMetadata { Name = "Test Wallet", RefId = "wallet_ref_001" } }
);
var wallet = walletsResponse.Wallets[0];
Console.WriteLine($"Wallet ID: {wallet.Id}, Address: {wallet.Address}, Blockchain: {wallet.Blockchain}");
```

**Output:** Wallet ID, blockchain address, and blockchain type.

**Notes:** Replace `<wallet-set-id>` with a valid ID. Use the wallet ID for transaction operations.

### Transaction Operations

**List Transactions:**
Retrieve transactions with optional filters:

```csharp
var transactions = await client.Transactions.ListTransactionsAsync(
    blockchain: "MATIC-AMOY",
    walletIds: "<wallet-id>",
    txType: "INBOUND",
    pageSize: 10,
    xRequestId: "request-list-transactions-" + Guid.NewGuid().ToString()
);
foreach (var tx in transactions.Transactions)
{
    Console.WriteLine($"ID: {tx.Id}, Blockchain: {tx.Blockchain}, State: {tx.State}, Amount: {string.Join(", ", tx.Amounts)}");
}
```

**Output:** List of transactions with IDs, blockchain, state, and amounts.

**Notes:** Replace `<wallet-id>` with a valid wallet ID. Filters like `txType` (INBOUND, OUTBOUND) are optional.

**Get Transaction:**
Retrieve details of a specific transaction:

```csharp
var transaction = await client.Transactions.GetTransactionAsync(
    id: "<transaction-id>",
    xRequestId: "request-get-transaction-" + Guid.NewGuid().ToString()
);
Console.WriteLine($"Transaction ID: {transaction.Id}, Blockchain: {transaction.Blockchain}, State: {transaction.State}");
```

**Output:** Transaction details including ID, blockchain, and state.

**Notes:** Replace `<transaction-id>` with a valid transaction ID from a prior operation.

**Create Transfer Transaction:**
Initiate a digital asset transfer:

```csharp
var transfer = await client.Transactions.CreateTransferTransactionAsync(
    walletId: "<wallet-id>",
    entitySecret: entitySecret,
    destinationAddress: "0xca9142d0b9804ef5e239d3bc1c7aa0d1c74e7350",
    idempotencyKey: Guid.NewGuid().ToString(),
    amounts: new[] { "0.01" },
    feeLevel: "MEDIUM",
    blockchain: "MATIC-AMOY",
    refId: "transfer-001",
    xRequestId: "request-create-transfer-" + Guid.NewGuid().ToString()
);
Console.WriteLine($"Transfer ID: {transfer.Id}, State: {transfer.State}");
```

**Output:** Transaction ID and state (INITIATED).

**Notes:** Ensure the wallet has sufficient balance. Replace `<wallet-id>` and adjust amounts as needed.

**Validate Address:**
Validate a blockchain address:

```csharp
var validate = await client.Transactions.ValidateAddressAsync(
    blockchain: "MATIC-AMOY",
    address: "0xca9142d0b9804ef5e239d3bc1c7aa0d1c74e7350",
    xRequestId: "request-validate-address-" + Guid.NewGuid().ToString()
);
Console.WriteLine($"Address Valid: {validate.IsValid}");
```

**Output:** Boolean indicating address validity (true or false).

**Notes:** Use supported blockchains (e.g., ETH, MATIC-AMOY, SOL).

**Estimate Contract Execution Fee:**
Estimate gas fees for a contract execution:

```csharp
var feeEstimate = await client.Transactions.EstimateContractExecutionFeeAsync(
    contractAddress: "0xa0b86991c6218b36c1d19d4a2e9eb0ce3606eb48",
    walletId: "<wallet-id>",
    abiFunctionSignature: "burn(uint256)",
    abiParameters: new object[] { 100 },
    xRequestId: "request-estimate-contract-fee-" + Guid.NewGuid().ToString()
);
Console.WriteLine($"Medium Fee: GasLimit={feeEstimate.Medium.GasLimit}, MaxFee={feeEstimate.Medium.MaxFee}");
```

**Output:** Fee estimates for low, medium, and high levels.

**Notes:** Replace `<wallet-id>` and use valid contract ABI data (e.g., `burn(uint256)` is a sample).

**Estimate Transfer Fee:**
Estimate gas fees for a transfer:

```csharp
var feeEstimate = await client.Transactions.EstimateTransferFeeAsync(
    destinationAddress: "0xca9142d0b9804ef5e239d3bc1c7aa0d1c74e7350",
    amounts: new[] { "6.62607015" },
    walletId: "<wallet-id>",
    blockchain: "MATIC-AMOY",
    xRequestId: "request-estimate-transfer-fee-" + Guid.NewGuid().ToString()
);
Console.WriteLine($"Medium Fee: GasLimit={feeEstimate.Medium.GasLimit}, MaxFee={feeEstimate.Medium.MaxFee}");
```

**Output:** Fee estimates for the transfer.

**Notes:** Replace `<wallet-id>` and adjust amounts based on wallet balance.

**Create Contract Execution Transaction:**
Execute a smart contract function:

```csharp
var contractExecution = await client.Transactions.CreateContractExecutionTransactionAsync(
    walletId: "<wallet-id>",
    entitySecret: entitySecret,
    contractAddress: "0xa0b86991c6218b36c1d19d4a2e9eb0ce3606eb48",
    idempotencyKey: Guid.NewGuid().ToString(),
    abiFunctionSignature: "burn(uint256)",
    abiParameters: new object[] { 100 },
    feeLevel: "MEDIUM",
    refId: "contract-execution-001",
    xRequestId: "request-create-contract-execution-" + Guid.NewGuid().ToString()
);
Console.WriteLine($"Contract Execution ID: {contractExecution.Id}, State: {contractExecution.State}");
```

**Output:** Transaction ID and state (INITIATED).

**Notes:** Replace `<wallet-id>` and use valid contract ABI data.

**Create Wallet Upgrade Transaction:**
Upgrade a wallet to a specific SCA version:

```csharp
var walletUpgrade = await client.Transactions.CreateWalletUpgradeTransactionAsync(
    walletId: "<wallet-id>",
    entitySecret: entitySecret,
    newScaCore: "circle_6900_singleowner_v2",
    idempotencyKey: Guid.NewGuid().ToString(),
    feeLevel: "MEDIUM",
    refId: "wallet-upgrade-001",
    xRequestId: "request-create-wallet-upgrade-" + Guid.NewGuid().ToString()
);
Console.WriteLine($"Wallet Upgrade ID: {walletUpgrade.Id}, State: {walletUpgrade.State}");
```

**Output:** Transaction ID and state (INITIATED).

**Notes:** Replace `<wallet-id>` and verify `newScaCore` compatibility.

**Cancel Transaction:**
Cancel a pending transaction (best-effort):

```csharp
var cancel = await client.Transactions.CancelTransactionAsync(
    id: "<transaction-id>",
    entitySecret: entitySecret,
    idempotencyKey: Guid.NewGuid().ToString(),
    xRequestId: "request-cancel-transaction-" + Guid.NewGuid().ToString()
);
Console.WriteLine($"Cancel ID: {cancel.Id}, State: {cancel.State}");
```

**Output:** Transaction ID and state (CANCELLED if successful).

**Notes:** Replace `<transaction-id>` with a valid ID in INITIATED or QUEUED state.

**Accelerate Transaction:**
Speed up a transaction with additional gas fees:

```csharp
var accelerate = await client.Transactions.AccelerateTransactionAsync(
    id: "<transaction-id>",
    entitySecret: entitySecret,
    idempotencyKey: Guid.NewGuid().ToString(),
    xRequestId: "request-accelerate-transaction-" + Guid.NewGuid().ToString()
);
Console.WriteLine($"Accelerate ID: {accelerate.Id}");
```

**Output:** Transaction ID.

**Notes:** Replace `<transaction-id>` with a valid ID in an acceleratable state.

### Token Operations

Retrieve details of a specific token:

```csharp
var token = await client.Tokens.GetTokenDetailsAsync(
    id: "<token-id>",
    xRequestId: "request-get-token-details-" + Guid.NewGuid().ToString()
);
Console.WriteLine($"Token ID: {token.Id}, Standard: {token.Standard}, Address: {token.TokenAddress}, Blockchain: {token.Blockchain}");
```

**Output:** Token details including ID, standard (e.g., ERC20), address, and blockchain.

**Notes:** Replace `<token-id>` with a valid token ID (e.g., from a transaction involving the token).

## Testing

The `CircleDeveloperControlledWalletSDKTest` project provides a console app to test all SDK functionalities. To run tests:

1. **Update `CircleDeveloperControlledWalletSDKTest.cs` with:**
   - Valid `apiKey` (e.g., `TEST_API_KEY:<key>:<secret>`).
   - Valid `walletSetId` from `CreateWalletSetAsync`.
   - Valid `testWalletId` for transaction tests.
   - Valid `testTokenId` for token tests.
   - Valid transaction IDs for `GetTransactionAsync`, `CancelTransactionAsync`, and `AccelerateTransactionAsync`.
   - Valid contract ABI data for `EstimateContractExecutionFeeAsync` and `CreateContractExecutionTransactionAsync` (replace `burn(uint256)` and `[100]`).

2. **Ensure write permissions for `entity_secret_recovery.json`.**

3. **Run the test app:**
   ```bash
   cd CircleDeveloperControlledWalletSDKTest
   dotnet run
   ```

### Example Test Output

```
Generating and registering Entity Secret...
Generated Entity Secret: 7ae43b03d7e48795cbf39ddad2f58dc8e186eb3d2dab3a5ec5bb3b33946639a4
Registration Status: SUCCESS
Recovery File: entity_secret_recovery.json

Creating Wallet Set...
Wallet Set ID: <uuid>, Name: TestWalletSet, Created: 2025-06-19T19:53:00Z

Creating Wallet...
Wallet ID: <uuid>, Address: 0xca9142d0b9804ef5e239d3bc1c7aa0d1c74e7350, Blockchain: MATIC-AMOY

Testing ListTransactionsAsync...
Retrieved 1 Transactions:
ID: <uuid>, Blockchain: MATIC-AMOY, State: CANCELLED, Amount: 6.62607015

Testing GetTransactionAsync...
Transaction ID: <uuid>, Blockchain: MATIC-AMOY, State: CANCELLED

Testing CreateTransferTransactionAsync...
Transfer ID: <uuid>, State: INITIATED

Testing ValidateAddressAsync...
Address Valid: true

Testing EstimateContractExecutionFeeAsync...
Medium Fee: GasLimit=21000, MaxFee=5.935224468

Testing EstimateTransferFeeAsync...
Medium Fee: GasLimit=21000, MaxFee=5.935224468

Testing CreateContractExecutionTransactionAsync...
Contract Execution ID: <uuid>, State: INITIATED

Testing CreateWalletUpgradeTransactionAsync...
Wallet Upgrade ID: <uuid>, State: INITIATED

Testing CancelTransactionAsync...
Cancel ID: <uuid>, State: CANCELLED

Testing AccelerateTransactionAsync...
Accelerate ID: <uuid>

Testing GetTokenDetailsAsync...
Token ID: <uuid>, Standard: ERC20, Address: 0xca9142d0b9804ef5e239d3bc1c7aa0d1c74e7350, Blockchain: MATIC-AMOY
```

### Test Notes

- **Placeholders:** Replace `testWalletId`, `testTokenId`, and transaction IDs with valid values from your Circle sandbox environment.
- **Contract Data:** The `burn(uint256)` and `[100]` in contract-related tests are samples. Use valid ABI data for your contract.
- **Transaction States:** Ensure transactions are in appropriate states (e.g., INITIATED or QUEUED) for cancellation or acceleration.
- **Error Debugging:** Check console output for error details (e.g., HTTP status codes, raw API responses) if tests fail.

## Error Handling

The SDK throws `CircleApiException` for API errors, providing:

- HTTP status code (e.g., 400, 401, 404).
- Error message from the API.
- Raw response content for debugging.

Handle errors as follows:

```csharp
try
{
    var transaction = await client.Transactions.GetTransactionAsync("invalid-id");
}
catch (CircleApiException ex)
{
    Console.WriteLine($"API Error (Status {ex.StatusCode}): {ex.Message}");
    Console.WriteLine($"Details: {ex.RawResponse}");
}
```

### Common errors:

- **400 Bad Request:** Invalid parameters (e.g., malformed Entity Secret, invalid wallet/token ID, incorrect ABI data).
- **401 Unauthorized:** Invalid or missing API key.
- **404 Not Found:** Non-existent wallet, transaction, or token ID.
- **CryptographicException:** Invalid Entity Secret format (must be 32-byte hex).
- **IOException:** Issues writing the recovery file.

## Limitations

- **Sandbox Only:** Configured for the Circle sandbox environment (`https://api.circle.com/v1/w3s/`).
- **Sample Data:** Contract-related tests use placeholder ABI data (`burn(uint256)`, `[100]`). Replace with valid contract data.
- **Transaction IDs:** Tests for `GetTransactionAsync`, `CancelTransactionAsync`, and `AccelerateTransactionAsync` require valid transaction IDs.
- **Token IDs:** The token test requires a valid token ID associated with your wallets.
- **SCA Version:** Wallet upgrades are limited to `circle_6900_singleowner_v2`. Verify compatibility.
- **No Batch Transactions:** Batch transaction submissions are not supported.
- **Error Messages:** Some API errors may be generic (e.g., "Unknown error"). Use `RawResponse` for debugging.

## Contributing

Contributions are welcome! To contribute:

1. Fork the repository.
2. Create a feature branch (`git checkout -b feature/<name>`).
3. Commit changes (`git commit -m "Add feature"`).
4. Push to the branch (`git push origin feature/<name>`).
5. Open a pull request with a detailed description.

Include:

- Unit tests for new features.
- Updated documentation in this README.
- Adherence to existing coding style (e.g., async methods, parameter validation).

## License

This project is licensed under the MIT License. See the LICENSE file for details.
