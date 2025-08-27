using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance { get; set; }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public bool isSavingToJson;


    #region  || -- General Section -- ||

    public void SaveGame()
    {
        AllGameData data = new AllGameData();

        data.playerData = GetPlayerData();

        SaveAllGameData(data);
    }

    private PlayerData GetPlayerData()
    {
        float[] playerStats = new float[3];
        playerStats[0] = PlayerState.instance.currentHealth;
        playerStats[1] = PlayerState.instance.currentCalories;
        playerStats[2] = PlayerState.instance.currentHydrationPercent;


        float[] playerPosAndRot = new float[6];
        //Player Position XYZ
        playerPosAndRot[0] = PlayerState.instance.playerModel.transform.position.x;
        playerPosAndRot[1] = PlayerState.instance.playerModel.transform.position.y;
        playerPosAndRot[2] = PlayerState.instance.playerModel.transform.position.z;
        //Player Rotation XYZ
        playerPosAndRot[3] = PlayerState.instance.playerModel.transform.rotation.x;
        playerPosAndRot[4] = PlayerState.instance.playerModel.transform.rotation.y;
        playerPosAndRot[5] = PlayerState.instance.playerModel.transform.rotation.z;

        return new PlayerData(playerStats, playerPosAndRot);
    }

    public void SaveAllGameData(AllGameData gameData)
    {
        if (isSavingToJson)
        {
            //SaveGameDataToJsonFile(gameData);
        }
        else
        {
            SaveGameToBinaryFile(gameData);
        }
    }

    #endregion

    #region  || -- ToJson Section -- ||

    #endregion

    #region  || -- ToBinary Section -- ||
    public void SaveGameToBinaryFile(AllGameData gameData)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/save_game.bin";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, gameData);
        stream.Close();

        print("Game Saved to: " + path);
    }

    public AllGameData LoadGameFromBinaryFile()
    {
        string path = Application.persistentDataPath + "/save_game.bin";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            AllGameData gameData = formatter.Deserialize(stream) as AllGameData;
            stream.Close();

            print("Game Loaded from: " + path);
            return gameData;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    #endregion

    #region || -- Settings Section -- ||

    #region || - Volume Settings - ||
    [System.Serializable]
    public class VolumeSettings
    {
        public float music;
        public float effects;
        public float master;
    }

    public void SaveVolumeSettings(float _music, float _effects, float _master)
    {
        VolumeSettings volumeSettings = new VolumeSettings
        {
            music = _music,
            effects = _effects,
            master = _master
        };

        string json = JsonUtility.ToJson(volumeSettings);
        PlayerPrefs.SetString("VolumeSettings", json);
        PlayerPrefs.Save();
    }

    public VolumeSettings LoadVolumeSettings()
    {
        return JsonUtility.FromJson<VolumeSettings>(PlayerPrefs.GetString("VolumeSettings"));
    }
    #endregion

    #endregion


}
