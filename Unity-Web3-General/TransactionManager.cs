// Filename: TransactionManager.cs
// Author: 0xFirekeeper
// Description: One place to set up and call any kind of supported EVM transaction.

using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;

// Verified transactions that we will use within our app
public enum Transaction
{
    CustomContract_getUser,
    CustomContract_createUser,
    ERC721_GetNfts,
    ERC721_GetUri,
    ERC721_GetBalance,
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

    /// CUSTOM TRANSACTIONS

    public async Task<string> ReadContract(Transaction transaction, Contract contract, string args = "[]")
    {
        print($"TRANSACTION: ReadContract - {transaction.ToString()}");
        // Get stored contract information
        ContractInfo info = new ContractInfo(contract);
        // Get method string off of enum syntax
        string method = transaction.ToString().Substring(transaction.ToString().IndexOf('_') + 1);
        // EVM.Call
        string response = await EVM.Call(info.chain, info.network, info.contract, info.abi, method, args, info.rpc);
        // Return response string
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
        // Return response string
        print($"RESPONSE: {response}");
        return response;
    }

    /// ERC721 TRANSACTIONS

    public async Task<string> ERC721_GetNfts(Transaction transaction, Contract contract = Contract.PolygonTestnet, int first = 500, int skip = 0)
    {
        print($"TRANSACTION: ERC721_GetNfts - {transaction.ToString()}");
        // Get stored contract information
        ContractInfo info = new ContractInfo(contract);
        // EVM.AllErc721
        string response = await EVM.AllErc721(info.chain, info.network, PlayerPrefs.GetString("Account"), info.contract, first, skip);
        // Return response string
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
        // Return response string
        print($"RESPONSE: {response}");
        return response;
    }

    public async Task<string> ERC721_GetBalance(Transaction transaction, Contract contract)
    {
        print($"TRANSACTION: ERC721_GetNfts - {transaction.ToString()}");
        // Get stored contract information
        ContractInfo info = new ContractInfo(contract);
        // ERC721.BalanceOf
        int balance = await ERC721.BalanceOf(info.chain, info.network, info.contract, PlayerPrefs.GetString("Account"), info.rpc);
        // Convert int to string
        string response = balance.ToString();
        // Return response string
        print($"RESPONSE: {response}");
        return response;
    }

}