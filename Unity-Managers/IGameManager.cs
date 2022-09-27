// Filename: IGameManager.cs
// Author: 0xFirekeeper
// Description: Game Manager Interface

public interface IGameManager
{
    int Score { get; set; }
    int Coins { get; set; }
    int Level { get; set; }
    int AccumulatedCoin { get; set; }
    string Version { get; set; }
    bool SoundOn { get; set; }
    bool VibrationOn { get; set; }

    bool IsScene(SceneName sceneName);
    bool IsGameState(GameState gameState);
    void SetGameState(GameState gameState);

    void LoadSceneAsyncByID(int id);
    void LoadSceneAsync(SceneName sceneName);
    void DecreaseLevel();
    void ReloadLevel();
    void AdvanceLevel();

    void SaveBinaryData();
    void LoadBinaryData();

}
