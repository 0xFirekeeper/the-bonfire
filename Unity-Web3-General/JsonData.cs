// Filename: JsonData.cs
// Author: 0xFirekeeper
// Description: Just a place for some JSON classes to test response deserializing with.

using System.Collections.Generic;

[System.Serializable]
public class JsonData_NFT
{
    public string contract;
    public string tokenId;
    public string uri;
    public int balance;
}

// GetURI response
[System.Serializable]
public class JsonData_URI
{
    public string uri;
}

// For getting the image from the URI
[System.Serializable]
public class JsonData_IMAGE
{
    public string image;
}