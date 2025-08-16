using UnityEngine;

public class GlobalState : MonoBehaviour
{
    #region Singleton
    public static GlobalState instance { get; set; }
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
    #endregion

    public float resourceHealth;
    public float resourceMaxHealth;
}
