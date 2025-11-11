using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer Reference")]
    public AudioMixer mainMixer;

    private const string BGM_PARAM = "BGMVolume";
    private const string SFX_PARAM = "SFXVolume";

    [Range(0f, 1f)] public float bgmVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            bgmVolume = Mathf.Clamp01(PlayerPrefs.GetFloat("BGMVolume", 1f));
            sfxVolume = Mathf.Clamp01(PlayerPrefs.GetFloat("SFXVolume", 1f));
            ApplyVolume();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetBGMVolume(float value)
    {
        bgmVolume = value;
        PlayerPrefs.SetFloat("BGMVolume", value);
        PlayerPrefs.Save();
        ApplyVolume();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
        ApplyVolume();
    }

    private void ApplyVolume()
    {
        // Convert linear 0–1 slider to mixer’s decibel scale
        float bgmDb = Mathf.Log10(Mathf.Max(bgmVolume, 0.0001f)) * 20f;
        float sfxDb = Mathf.Log10(Mathf.Max(sfxVolume, 0.0001f)) * 20f;

        mainMixer.SetFloat(BGM_PARAM, bgmDb);
        mainMixer.SetFloat(SFX_PARAM, sfxDb);
    }
}
