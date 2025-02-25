using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Slider volumeSlider;

    [Header("Audio Settings")]
    public AudioMixer audioMixer;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions = new List<Resolution>();

    void Start()
    {
        LoadResolutions();
        LoadSettings();
    }

    void LoadResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        filteredResolutions.Clear();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            // Prevent duplicate resolutions
            if (!filteredResolutions.Exists(r => r.width == resolutions[i].width && r.height == resolutions[i].height))
            {
                filteredResolutions.Add(resolutions[i]);
                options.Add(resolutions[i].width + " x " + resolutions[i].height);

                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = filteredResolutions.Count - 1;
                }
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void ApplySettings()
    {
        int selectedResolutionIndex = resolutionDropdown.value;
        bool isFullscreen = fullscreenToggle.isOn;

        Resolution selectedResolution = filteredResolutions[selectedResolutionIndex];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, isFullscreen ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed);

        SaveSettings(selectedResolutionIndex, isFullscreen);
    }

    void LoadSettings()
    {
        int savedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", resolutionDropdown.value);
        bool savedFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        float savedVolume = PlayerPrefs.GetFloat("Volume", 0.5f);

        resolutionDropdown.value = savedResolutionIndex;
        fullscreenToggle.isOn = savedFullscreen;
        volumeSlider.value = savedVolume;

        audioMixer.SetFloat("Volume", savedVolume);
    }

    void SaveSettings(int resolutionIndex, bool isFullscreen)
    {
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
        PlayerPrefs.SetFloat("Volume", volume);
    }
}
