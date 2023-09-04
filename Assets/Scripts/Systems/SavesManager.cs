using UnityEngine;
using UnityEngine.Serialization;


public static class SavesManager
{
    public static SavePlayerData SavePlayerData { get; private set; }

    private static void Save()
    {
        PlayerPrefs.SetString("SavedPlayer", JsonUtility.ToJson(SavePlayerData));
        PlayerPrefs.Save();
    }

    public static void Load()
    {
        SavePlayerData = JsonUtility.FromJson<SavePlayerData>(PlayerPrefs.GetString("SavedPlayer"));

        if (SavePlayerData == null)
        {
            SavePlayerData = new SavePlayerData(0, 0, 0, 0);
            Save();
        }
    }

    public static void Reset()
    {
        SavePlayerData = new SavePlayerData(0, 0, 0, 0);
        Save();
    }

    /// <summary>
    /// set volume to value and also save the entire savePlayerData
    /// </summary>
    /// <param name="value"></param>
    public static void SetMasterVolume(float value)
    {
        SavePlayerData = new SavePlayerData(value,
            SavePlayerData.musicVolume,
            SavePlayerData.sfxVolume,
            SavePlayerData.bestScore);
        Save();
    }

    public static void SetMusicVolume(float value)
    {
        SavePlayerData = new SavePlayerData(SavePlayerData.masterVolume, value,
            SavePlayerData.sfxVolume, SavePlayerData.bestScore);
        Save();
    }

    public static void SetSFXVolume(float value)
    {
        SavePlayerData = new SavePlayerData(SavePlayerData.masterVolume,
            SavePlayerData.musicVolume,
            value,
            SavePlayerData.bestScore);
        Save();
    }

    public static void SetBestScore(int value)
    {
        SavePlayerData = new SavePlayerData(SavePlayerData.masterVolume, SavePlayerData.musicVolume,
            SavePlayerData.sfxVolume, value);
        Save();
    }
}

[System.Serializable]
public class SavePlayerData
{
    public float masterVolume;
    public float musicVolume, sfxVolume;
    public int bestScore;

    public SavePlayerData(float masterVolume, float musicVolume, float sfxVolume, int bestScore)
    {
        this.masterVolume = masterVolume;
        this.musicVolume = musicVolume;
        this.sfxVolume = sfxVolume;
        this.bestScore = bestScore;
    }
}
