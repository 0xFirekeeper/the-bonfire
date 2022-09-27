// Filename: PlayerData.cs
// Author: 0xFirekeeper
// Description: Binary Data Parsing Type

[System.Serializable]
public class PlayerData
{
    // Generic
    public int _score;
    public string _version;
    public int _level;
    public int _coins;


    public PlayerData(GameManager gameManager)
    {
        // Generic
        _score = gameManager.Score;
        _version = gameManager.Version;
        _level = gameManager.Level;
        _coins = gameManager.Coins;
    }
}
