using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Settings")]
    public AudioMixer audioMixer;
    public AudioSource musicSource;
    public Slider volumeSlider;

    [Header("Background Music Clips")]
    public AudioClip mainMenuMusic;
    public AudioClip townMusic;
    public AudioClip dungeonMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Load saved volume settings
        float savedVolume = PlayerPrefs.GetFloat("Volume", 0.5f);
        volumeSlider.value = savedVolume;
        audioMixer.SetFloat("Volume", savedVolume);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip) return; // Prevent restarting the same music

        musicSource.clip = clip;
        musicSource.Play();
    }
}
