// Filename: WebLogout.cs
// Author: 0xFirekeeper
// Description: WebGL Logout Script

using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_WEBGL
public class WebLogout : MonoBehaviour
{
    public void OnSignOut()
    {
        // Clear Account
        SimpleGameManager.Instance.Account = "0x0000000000000000000000000000000000000001";
        // Do Something
        SceneManager.LoadScene(0);
    }
}
#endif