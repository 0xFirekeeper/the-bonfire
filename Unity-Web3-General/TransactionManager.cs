// Filename: TransactionManager.cs
// Author: 0xFirekeeper
// Description: One place to set up and call any kind of supported EVM transaction.

using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;

// Verified transactions that we will use within our app
public enum Transaction
{
    CustomContract_getUser,
    CustomContract_createUser,
    ERC721_GetNfts,
    ERC721_GetUri,
    ERC721_GetBalanceOf,
    ERC1155_GetNfts,
    ERC1155_GetURI,
    ERC1155_GetBalanceOf,
    ERC1155_GetBalanceOfBatch
}

// Event to use the response in-app
[System.Serializable]
public class ResponseEvent : UnityEvent<string> { }

// Simple way to call any supported transaction
public class TransactionManager : MonoBehaviour
{
    // Persistent Static Instance
    public static TransactionManager Instance;

    // Initializes Instance
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// CUSTOM CONTRACT READ/WRITE ///

    public async Task<string> ReadContract(Transaction transaction, Contract contract, string args = "[]")
    {
        print($"TRANSACTION: ReadContract - {transaction.ToString()}");
        // Get stored contract information
        ContractInfo info = new ContractInfo(contract);
        // Get method string off of enum syntax
        string method = transaction.ToString().Substring(transaction.ToString().IndexOf('_') + 1);
        // EVM.Call
        string response = await EVM.Call(info.chain, info.network, info.contract, info.abi, method, args, info.rpc);
        // Print response and return
        print($"RESPONSE: {response}");
        return response;
    }

    public async Task<string> WriteContract(Transaction transaction, Contract contract, string args = "[]", string value = "0", string gaslimit = "", string gasPrice = "")
    {
        print($"TRANSACTION: WriteContract - {transaction.ToString()}");
        // Get stored contract information
        ContractInfo info = new ContractInfo(contract);
        // Get method string off of enum syntax
        string method = transaction.ToString().Substring(transaction.ToString().IndexOf('_') + 1);
        // Create custom contract data
        string contractData = await EVM.CreateContractData(info.abi, method, args);
        // Web3GL.SendContract
        string response = await Web3GL.SendContract(method, info.abi, info.contract, args, value, gaslimit, gasPrice);
        // Print response and return
        print($"RESPONSE: {response}");
        return response;
    }

    /// ERC721 TRANSACTIONS ///

    public async Task<string> ERC721_GetNfts(Transaction transaction, Contract contract = Contract.PolygonTestnet, int first = 500, int skip = 0)
    {
        print($"TRANSACTION: ERC721_GetNfts - {transaction.ToString()}");
        // Get stored contract information
        ContractInfo info = new ContractInfo(contract);
        // EVM.AllErc721
        string response = await EVM.AllErc721(info.chain, info.network, PlayerPrefs.GetString("Account"), info.contract, first, skip);
        // Print response and return
        print($"RESPONSE: {response}");
        return response;
    }

    public async Task<string> ERC721_GetURI(Transaction transaction, Contract contract, string tokenId)
    {
        print($"TRANSACTION: _ERC721_GetURI - {transaction.ToString()}");
        // Get stored contract information
        ContractInfo info = new ContractInfo(contract);
        // ERC721.URI
        string response = await ERC721.URI(info.chain, info.network, info.contract, tokenId, info.rpc);
        // Print response and return
        print($"RESPONSE: {response}");
        return response;
    }

    public async Task<int> ERC721_GetBalanceOf(Transaction transaction, Contract contract)
    {
        print($"TRANSACTION: ERC721_GetNfts - {transaction.ToString()}");
        // Get stored contract information
        ContractInfo info = new ContractInfo(contract);
        // ERC721.BalanceOf
        int balance = await ERC721.BalanceOf(info.chain, info.network, info.contract, PlayerPrefs.GetString("Account"), info.rpc);
        // Print response and return
        print($"RESPONSE: {balance}");
        return balance;
    }

    /// ERC1155 TRANSACTIONS ///

    public async Task<string> ERC1155_GetNfts(Transaction transaction, Contract contract = Contract.PolygonTestnet, int first = 500, int skip = 0)
    {
        print($"TRANSACTION: ERC1155_GetNfts - {transaction.ToString()}");
        // Get stored contract information
        ContractInfo info = new ContractInfo(contract);
        // EVM.AllErc1155
        string response = await EVM.AllErc1155(info.chain, info.network, PlayerPrefs.GetString("Account"), info.contract, first, skip);
        // Print response and return
        print($"RESPONSE: {response}");
        return response;
    }

    public async Task<string> ERC1155_GetURI(Transaction transaction, Contract contract, string tokenId)
    {
        print($"TRANSACTION: _ERC1155_GetURI - {transaction.ToString()}");
        // Get stored contract information
        ContractInfo info = new ContractInfo(contract);
        // ERC1155.URI
        string response = await ERC1155.URI(info.chain, info.network, info.contract, tokenId, info.rpc);
        // Print response and return
        print($"RESPONSE: {response}");
        return response;
    }

    public async Task<BigInteger> ERC1155_GetBalanceOf(Transaction transaction, Contract contract, string tokenId)
    {
        print($"TRANSACTION: ERC1155_GetBalance - {transaction.ToString()}");
        // Get stored contract information
        ContractInfo info = new ContractInfo(contract);
        // ERC1155.BalanceOf
        BigInteger balance = await ERC1155.BalanceOf(info.chain, info.network, info.contract, PlayerPrefs.GetString("Account"), tokenId, info.rpc);
        // Print response and return
        print($"RESPONSE: {balance}");
        return balance;
    }

    public async Task<List<BigInteger>> ERC1155_GetBalanceOfBatch(Transaction transaction, Contract contract, string[] tokenIds)
    {
        print($"TRANSACTION: ERC1155_GetBalance - {transaction.ToString()}");
        // Get stored contract information
        ContractInfo info = new ContractInfo(contract);
        // Just one account for now, but could pass more
        string currentAccount = PlayerPrefs.GetString("Account");
        string[] accounts = { currentAccount };
        // ERC1155.BalanceOf
        List<BigInteger> balances = await ERC1155.BalanceOfBatch(info.chain, info.network, info.contract, accounts, tokenIds, info.rpc);
        // Print response and return
        print("RESPONSE: ");
        foreach (var value in balances)
            print(value + " ");

        return balances;
    }

}