using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Sliders")]
    public Slider bgmSlider;
    public Slider sfxSlider;

    private void Start()
    {
        if (AudioManager.Instance)
        {
            bgmSlider.value = AudioManager.Instance.bgmVolume;
            sfxSlider.value = AudioManager.Instance.sfxVolume;
        }

        bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    public void OnBGMVolumeChanged(float value)
    {
        AudioManager.Instance?.SetBGMVolume(value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance?.SetSFXVolume(value);
    }
}
