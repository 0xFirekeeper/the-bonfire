// Filename: SaveSystem.cs
// Author: 0xFirekeeper
// Description: Handles Saving and Loading Binary Data

using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveGameState(GameManager gameManager)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/controller.state";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(gameManager);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadGameState()
    {
        string path = Application.persistentDataPath + "/controller.state";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.Log("Save file not found in " + path);
            return null;
        }
    }

    public static void Delete()
    {
        string path = Application.persistentDataPath + "/controller.state";

        try
        {
            File.Delete(path);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
}
