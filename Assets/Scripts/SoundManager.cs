using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; set; }
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

    [Header("SoundFX")]
    public AudioSource dropItemSound;
    public AudioSource craftingSound;
    public AudioSource toolSwingSound;
    public AudioSource chopSound;
    public AudioSource pickupItemSound;
    public AudioSource grassWalk;

    [Header("MusicFX")]
    public AudioSource startingZoneBGMusic;
    
    public void PlaySound(AudioSource soundToPlay)
    {
        if (!soundToPlay.isPlaying)
        {
            soundToPlay.Play();
        }
    }
}
