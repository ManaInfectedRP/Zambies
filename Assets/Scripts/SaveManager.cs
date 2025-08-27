using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    //--BinaryPath--
    string binaryPath;

    //--Json Project Save Path-- ( Used during Development )
    string jsonPathProject;

    //--Json External/Real Save Path-- ( Change to this when making a build/release )
    string jsonPathPresistant;

    public bool isSavingToJson = true;

    void Start()
    {
        jsonPathProject = Application.dataPath + Path.AltDirectorySeparatorChar + "SaveGame.json";
        jsonPathPresistant = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "SaveGame.json";
        binaryPath = Application.persistentDataPath + "/save_game.bin";
    }

    #region  || -- General Section -- ||

    #region |-- Saving --|
    public void SaveGame()
    {
        AllGameData data = new AllGameData();

        data.playerData = GetPlayerData();

        SavingTypeSwitch(data);
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
    public void SavingTypeSwitch(AllGameData gameData)
    {
        if (isSavingToJson)
        {
            SaveGameDataToJsonFile(gameData);
        }
        else
        {
            SaveGameDataToBinaryFile(gameData);
        }
    }

    #endregion

    #region |-- Loading --|

    public AllGameData LoadingTypeSwitch()
    {
        if (isSavingToJson)
        {
            AllGameData gameData = LoadGameDataFromJsonFile();
            return gameData;
        }
        else
        {
            AllGameData gameData = LoadGameDataFromBinaryFile();
            return gameData;
        }
    }

    public void LoadGame()
    {
        //Player Data
        SetPlayerData(LoadingTypeSwitch().playerData);

        //Enviroment Data
        //SetEnviromentData(LoadingTypeSwitch().enviromentData);
        //...
    }

    public void SetPlayerData(PlayerData playerData)
    {
        //Player Stats
        PlayerState.instance.currentHealth = playerData.playerStats[0];
        PlayerState.instance.currentCalories = playerData.playerStats[1];
        PlayerState.instance.currentHydrationPercent = playerData.playerStats[2];

        //Player Position
        Vector3 loadPosition;
        loadPosition.x = playerData.playerPositionAndRotation[0];
        loadPosition.y = playerData.playerPositionAndRotation[1];
        loadPosition.z = playerData.playerPositionAndRotation[2];
        PlayerState.instance.playerModel.transform.position = loadPosition;

        Vector3 loadRotation;
        loadRotation.x = playerData.playerPositionAndRotation[3];
        loadRotation.y = playerData.playerPositionAndRotation[4];
        loadRotation.z = playerData.playerPositionAndRotation[5];
        PlayerState.instance.playerModel.transform.rotation = Quaternion.Euler(loadRotation);
    }

    public void StartLoadedGame()
    {
        SceneManager.LoadScene("GameScene");

        StartCoroutine(DelayedLoading());
    }

    private IEnumerator DelayedLoading()
    {
        yield return new WaitForSeconds(1f);
        LoadGame();
    }

    #endregion

    //general EndRegion
    #endregion


    #region  || -- ToJson Section -- ||
    public void SaveGameDataToJsonFile(AllGameData gameData)
    {
        string json = JsonUtility.ToJson(gameData);

        string encrypted = EncryptionDecryption(json);

        using (StreamWriter writer = new StreamWriter(jsonPathProject))
        {
            writer.Write(encrypted);
            print("Game Saved to: " + jsonPathProject);
        }

        //print("Game Saved to: "  /*+ path*/);
    }

    public AllGameData LoadGameDataFromJsonFile()
    {
        using (StreamReader reader = new StreamReader(jsonPathProject))
        {
            string json = reader.ReadToEnd();

            string decrypted = EncryptionDecryption(json);

            AllGameData data = JsonUtility.FromJson<AllGameData>(decrypted);

            print("Game Loaded from: " + jsonPathProject);
            return data;
        }

    }

    #endregion

    #region  || -- ToBinary Section -- ||
    public void SaveGameDataToBinaryFile(AllGameData gameData)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        FileStream stream = new FileStream(binaryPath, FileMode.Create);

        formatter.Serialize(stream, gameData);
        stream.Close();

        print("Game Saved to: " + binaryPath);
    }

    public AllGameData LoadGameDataFromBinaryFile()
    {
        if (File.Exists(binaryPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(binaryPath, FileMode.Open);

            AllGameData gameData = formatter.Deserialize(stream) as AllGameData;
            stream.Close();

            print("Game Loaded from: " + binaryPath);
            return gameData;
        }
        else
        {
            Debug.LogError("Save file not found in " + binaryPath);
            return null;
        }
    }



    #endregion

    #region || -- Settings Section -- ||

    #region |-- Volume Settings --|
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

    #region || -- Encryption Section -- ||
    public string EncryptionDecryption(string jsonString)
    {
        string keyword = "1234567";
        string result = "";

        for (int i = 0; i < jsonString.Length; i++)
        {
            result += (char)(jsonString[i] ^ keyword[i & keyword.Length]);
        }

        return result;

        /*
            XOR ^ = "Is there a difference"

            -- Encrypt --
            Mike - 01101101 01101001 01101011 01100101
            M -    01101101
            Key -  00000001

            Encrypted 01101100
            
            -- Decrypt --
            Encrypted 01101100
            Key -     00000001
            =
            M -       01101101
        */
    }

    #endregion
}
