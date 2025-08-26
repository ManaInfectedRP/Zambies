using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static MainMenuSaveManager;
public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance { get; set; }
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

    public Button backButton;

    public Slider masterSlider;
    public GameObject masterValue;

    public Slider musicSlider;
    public GameObject musicValue;

    public Slider effectsSlider;
    public GameObject effectsValue;

    void Start()
    {
        backButton.onClick.AddListener(() =>
        {
            MainMenuSaveManager.instance.SaveVolumeSettings(musicSlider.value, effectsSlider.value, masterSlider.value);

            print("Settings Saved to PlayerPrefs");
        });

        StartCoroutine(LoadAndApplySettings());
    }

    IEnumerator LoadAndApplySettings()
    {
        LoadAndSetVolume();
        //Laod Grapihics Settings
        //Load Mouse Sensitivity
        //Load Keybindings
        yield return new WaitForSeconds(0.1f);
    }

    void LoadAndSetVolume()
    {
        VolumeSettings volumeSettings = MainMenuSaveManager.instance.LoadVolumeSettings();

        if (volumeSettings != null)
        {
            musicSlider.value = volumeSettings.music;
            effectsSlider.value = volumeSettings.effects;
            masterSlider.value = volumeSettings.master;
        }
        else
        {
            musicSlider.value = 100;
            effectsSlider.value = 100;
            masterSlider.value = 100;
        }

        print("Volume Settings Loaded from PlayerPrefs");
    }

    void Update()
    {
        masterValue.GetComponent<TextMeshProUGUI>().text = "" + (masterSlider.value) + "";
        musicValue.GetComponent<TextMeshProUGUI>().text = "" + (musicSlider.value) + "";
        effectsValue.GetComponent<TextMeshProUGUI>().text = "" + (effectsSlider.value) + "";
    }


}
