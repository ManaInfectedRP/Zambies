using UnityEngine;

public class MainMenuSaveManager : MonoBehaviour
{
    public static MainMenuSaveManager instance { get; set; }
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
    }

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
}
