// Filename: DownloadIPFS.cs
// Author: 0xFirekeeper
// Description: Download any single supported file hosted on IPFS. 

using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class DownloadIPFS : MonoBehaviour
{
    [Header("IPFS Details")]
    public string httpsGateway = "https://nftstorage.link/ipfs/";

    public static DownloadIPFS Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public async Task<Sprite> DownloadIPFSImage(string ipfsPath)
    {
        if (ipfsPath.StartsWith("ipfs://"))
            ipfsPath = ipfsPath.Replace("ipfs://", httpsGateway);
        else
            throw new InvalidDataException("Please provide the full IPFS path to your file.");

        UnityWebRequest webRequest;

        // // If using Pinata style dedicated gateway, you can add a custom query parameters
        // uri += "?img-width=2048&img-height=2348";
        webRequest = UnityWebRequestTexture.GetTexture(ipfsPath);

        Debug.Log($"REQUESTING: {ipfsPath}");

        await webRequest.SendWebRequest();

        /// REQUEST FAIL
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"REQUEST FAIL: {ipfsPath} - ERROR: {webRequest.error}");
            return null;
        }
        /// REQUEST SUCCESS
        else
        {
            Debug.Log($"REQUEST SUCCESS: {ipfsPath}");

            Texture2D itemTexture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
            Sprite itemSprite = Sprite.Create(itemTexture, new Rect(0.0f, 0.0f, itemTexture.width, itemTexture.height), new UnityEngine.Vector2(0.5f, 0.5f), 100.0f);

            return itemSprite;
        }
    }

    public async Task<string> DownloadIPFSText(string ipfsPath)
    {
        if (ipfsPath.StartsWith("ipfs://"))
            ipfsPath = ipfsPath.Replace("ipfs://", httpsGateway);
        else
            throw new InvalidDataException("Please provide the full IPFS path to your file.");

        UnityWebRequest webRequest;

        // // If using Pinata style dedicated gateway, you can add a custom query parameters
        // uri += "?img-width=2048&img-height=2348";
        webRequest = UnityWebRequest.Get(ipfsPath);

        Debug.Log($"REQUESTING: {ipfsPath}");

        await webRequest.SendWebRequest();

        /// REQUEST FAIL
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"REQUEST FAIL: {ipfsPath} - ERROR: {webRequest.error}");
            return null;
        }
        /// REQUEST SUCCESS
        else
        {
            Debug.Log($"REQUEST SUCCESS: {ipfsPath}");

            string textData = webRequest.downloadHandler.text;

            return textData;
        }
    }
}