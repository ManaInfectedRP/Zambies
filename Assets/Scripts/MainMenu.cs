using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    public Button loadGameButton;

    void Start()
    {
        loadGameButton.onClick.AddListener(() =>
        {
            SaveManager.instance.StartLoadedGame();
        });
    }

    public void NewGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ExitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}
