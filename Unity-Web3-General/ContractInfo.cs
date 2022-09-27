// Filename: ContractInfo.cs
// Author: 0xFirekeeper
// Description: One place to set up all verified contracts to be used in your app.

// Verified contracts, used to fetch on-chain data
public enum Contract
{
    PolygonTestnet,
    CustomContract,
    NFTCollection
}

// Set the general on-chain info for each case for better organization
[System.Serializable]
public class ContractInfo
{
    public Contract identifier; // identifier for the constructor
    public string chain; // set chain: ethereum, moonbeam, polygon etc]
    public string network; // set network mainnet, testnet
    public string chainId; // chain ID needed for write transactions
    public string rpc; // optional rpc
    public string contract; // address of contract
    public string abi; // abi in json format
    public string cidv0; // usually starts with Q and has lower and upper case
    public string cidv1; // usuallly starts with b and is only lower case

    public ContractInfo(Contract _identifier)
    {
        identifier = _identifier;

        switch (identifier)
        {
            case (Contract.PolygonTestnet):
                chain = "polygon";
                network = "testnet";
                chainId = "80001";
                rpc = "";
                contract = "";
                abi = "";
                break;
            case (Contract.CustomContract):
                chain = "polygon";
                network = "testnet";
                chainId = "80001";
                rpc = "";
                contract = "0xC88EdcCfada9755A1d86e521a63ffB37cA4a1106";
                abi = "[  {   \"inputs\": [    {     \"internalType\": \"string\",     \"name\": \"_nameString\",     \"type\": \"string\"    }   ],   \"name\": \"createUser\",   \"outputs\": [],   \"stateMutability\": \"nonpayable\",   \"type\": \"function\"  },  {   \"inputs\": [],   \"name\": \"deleteUser\",   \"outputs\": [],   \"stateMutability\": \"nonpayable\",   \"type\": \"function\"  },  {   \"inputs\": [    {     \"internalType\": \"address\",     \"name\": \"_address\",     \"type\": \"address\"    }   ],   \"name\": \"getUser\",   \"outputs\": [    {     \"internalType\": \"enum HelloWeb3.UserStatus\",     \"name\": \"_status\",     \"type\": \"uint8\"    },    {     \"internalType\": \"string\",     \"name\": \"_name\",     \"type\": \"string\"    }   ],   \"stateMutability\": \"view\",   \"type\": \"function\"  } ]";
                break;
            case (Contract.NFTCollection):
                chain = "ethereum";
                network = "mainnet";
                chainId = "1";
                rpc = "";
                contract = "0x373ffdf2a50003fb7e10282cf50e03921828e1a7";
                abi = "";
                cidv0 = "QmbHMjJXVYicKcqshhjpH8xGZbmbsZg5JA6YxSkEqx5DAG";
                cidv1 = "bafybeigaj76s3ezefx3ryeropcoczwpud4auaxh673c5s7pdrfa77lu4te";
                break;
        }
    }

}