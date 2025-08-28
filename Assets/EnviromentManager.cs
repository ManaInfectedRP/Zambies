using UnityEngine;

public class EnviromentManager : MonoBehaviour
{
    public static EnviromentManager instance { get; set; }
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

    public GameObject allItems;
}
