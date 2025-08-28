using System;
using System.Collections;
using System.Collections.Generic;
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

    string fileName = "SaveGame";

    public bool isSavingToJson = true;
    public bool isLoading;

    public Canvas loadingScreen;

    void Start()
    {
        jsonPathProject = Application.dataPath + Path.AltDirectorySeparatorChar;
        jsonPathPresistant = Application.persistentDataPath + Path.AltDirectorySeparatorChar;
        binaryPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar;
    }

    #region  || -- General Section -- ||

    #region |-- Saving --|
    public void SaveGame(int _slotNumber)
    {
        AllGameData data = new AllGameData();

        data.playerData = GetPlayerData();
        data.environmentData = GetEnviromentData();

        SavingTypeSwitch(data, _slotNumber);
    }

    private EnviromentData GetEnviromentData()
    {
        List<string> itemsPickedUp = InventorySystem.instance.itemsPickedUp;
        return new EnviromentData(itemsPickedUp);
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

        string[] inventoryContent = InventorySystem.instance.itemList.ToArray();
        string[] hotbarContent = GetQuickSlotsContent();

        return new PlayerData(playerStats, playerPosAndRot, inventoryContent, hotbarContent);
    }

    private string[] GetQuickSlotsContent()
    {
        List<string> temp = new List<string>();

        foreach (GameObject slot in EquipSystem.instance.quickSlotsList)
        {
            if (slot.transform.childCount != 0)
            {
                string name = slot.transform.GetChild(0).name;
                string str2 = "(Clone)";
                string cleanName = name.Replace(str2, "");
                temp.Add(cleanName);
            }
        }
        return temp.ToArray();
    }

    public void SavingTypeSwitch(AllGameData gameData, int _slotNumber)
    {
        if (isSavingToJson)
        {
            SaveGameDataToJsonFile(gameData, _slotNumber);
        }
        else
        {
            SaveGameDataToBinaryFile(gameData, _slotNumber);
        }
    }

    #endregion

    #region |-- Loading --|

    public AllGameData LoadingTypeSwitch(int _slotNumber)
    {
        if (isSavingToJson)
        {
            AllGameData gameData = LoadGameDataFromJsonFile(_slotNumber);
            return gameData;
        }
        else
        {
            AllGameData gameData = LoadGameDataFromBinaryFile(_slotNumber);
            return gameData;
        }
    }

    public void LoadGame(int _slotNumber)
    {
        //Player Data
        SetPlayerData(LoadingTypeSwitch(_slotNumber).playerData);

        //Enviroment Data
        SetEnviromentData(LoadingTypeSwitch(_slotNumber).environmentData);
        //...

        isLoading = false;
        DisableLoadingScreen();
    }

    private void SetEnviromentData(EnviromentData enviromentData)
    {
        foreach (Transform itemType in EnviromentManager.instance.allItems.transform)
        {
            foreach (Transform item in itemType.transform)
            {
                if (enviromentData.pickedUpItems.Contains(item.name))
                {
                    Destroy(item.gameObject);
                }
            }
        }

        InventorySystem.instance.itemsPickedUp = enviromentData.pickedUpItems;

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

        // Inventory Content
        foreach (var item in playerData.inventoryContent)
        {
            InventorySystem.instance.AddToInventory(item);
        }

        foreach (var item in playerData.hotbarContent)
        {
            // Find next free QuickSlot
            GameObject availableSlot = EquipSystem.instance.FindNextEmptySlot();

            var itemToAdd = Instantiate(Resources.Load<GameObject>(item));

            itemToAdd.transform.SetParent(availableSlot.transform, false);
        }
    }

    public void StartLoadedGame(int _slotNumber)
    {
        ActivateLoadingScreen();
        isLoading = true;

        SceneManager.LoadScene("GameScene");

        StartCoroutine(DelayedLoading(_slotNumber));
    }

    private IEnumerator DelayedLoading(int _slotNumber)
    {
        yield return new WaitForSeconds(1f);
        LoadGame(_slotNumber);
    }

    #endregion

    //general EndRegion
    #endregion

    #region  || -- ToJson Section -- ||
    public void SaveGameDataToJsonFile(AllGameData _gameData, int _slotNumber)
    {
        string json = JsonUtility.ToJson(_gameData);

        //string encrypted = EncryptionDecryption(json);

        using (StreamWriter writer = new StreamWriter(jsonPathProject + fileName + _slotNumber + ".json"))
        {
            writer.Write(json);
            print("Game Saved to: " + jsonPathProject + fileName + _slotNumber + ".json");
        }

        //print("Game Saved to: "  /*+ path*/);
    }

    public AllGameData LoadGameDataFromJsonFile(int _slotNumber)
    {
        using (StreamReader reader = new StreamReader(jsonPathProject + fileName + _slotNumber + ".json"))
        {
            string json = reader.ReadToEnd();

            //string decrypted = EncryptionDecryption(json);

            AllGameData data = JsonUtility.FromJson<AllGameData>(json);

            print("Game Loaded from: " + jsonPathProject + fileName + _slotNumber + ".json");
            return data;
        }

    }

    #endregion

    #region  || -- ToBinary Section -- ||
    public void SaveGameDataToBinaryFile(AllGameData gameData, int _slotNumber)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        FileStream stream = new FileStream(binaryPath + fileName + _slotNumber + ".bin", FileMode.Create);

        formatter.Serialize(stream, gameData);
        stream.Close();

        print("Game Saved to: " + binaryPath + fileName + _slotNumber + ".bin");
    }

    public AllGameData LoadGameDataFromBinaryFile(int _slotNumber)
    {
        if (File.Exists(binaryPath + fileName + _slotNumber + ".bin"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(binaryPath + fileName + _slotNumber + ".bin", FileMode.Open);

            AllGameData gameData = formatter.Deserialize(stream) as AllGameData;
            stream.Close();

            print("Game Loaded from: " + binaryPath + fileName + _slotNumber + ".bin");
            return gameData;
        }
        else
        {
            Debug.LogError("Save file not found in " + binaryPath + fileName + _slotNumber + ".json");
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
            result += (char)(jsonString[i] ^ keyword[i % keyword.Length]);
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

    #region  || -- Loading Section -- ||
    public void ActivateLoadingScreen()
    {
        loadingScreen.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Music During Loadingscreen?
        //SoundManager.instance.loadingScreen?

        //Animate Loading Screen?
        // * anim.Play();?

        // Show Tooltips
        // * List<string> loadingTips;
        // * loadingTips.text = loadingTips;
    }
    
    public void DisableLoadingScreen()
    {
        loadingScreen.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    #endregion

    #region || -- Utility -- ||

    public bool DoesFileExists(int slotNumber)
    {
        if (isSavingToJson)
        {
            if (System.IO.File.Exists(jsonPathProject + fileName + slotNumber + ".json"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (System.IO.File.Exists(binaryPath + fileName + slotNumber + ".bin"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public bool isSlotEmpty(int _slotNumber)
    {
        if (DoesFileExists(_slotNumber))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public void DeSelectButton()
    {
        GameObject myEventSystem = GameObject.Find("EventSystem");
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
    }
    
    #endregion
}
