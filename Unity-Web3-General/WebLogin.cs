// Filename: WebLogin.cs
// Author: 0xFirekeeper
// Description: WebGL Login Script

using System.Runtime.InteropServices;
using UnityEngine;

#if UNITY_WEBGL
public class WebLogin : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void Web3Connect();

    [DllImport("__Internal")]
    private static extern string ConnectAccount();

    [DllImport("__Internal")]
    private static extern void SetConnectAccount(string value);

    private int expirationTime;
    private string account;

    public void OnLogin()
    {
        Web3Connect();
        OnConnected();
    }

    async private void OnConnected()
    {
        account = ConnectAccount();
        while (account == "")
        {
            await new WaitForSeconds(1f);
            account = ConnectAccount();
        };

        SetConnectAccount(""); // Reset login message
        SimpleGameManager.Instance.Account = account; // Save account for next scene

        OnLoggedIn();
    }

    public void OnSkip()
    {
#if UNITY_EDITOR
        SimpleGameManager.Instance.Account = ""; // add test account for web3 features for possible editor web3 tests
#else
        SimpleGameManager.Instance.Account = "";
#endif

        OnLoggedIn();
    }

    void OnLoggedIn()
    {
        // Do something after logging in
    }
}
#endif