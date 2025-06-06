using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AudioManager : Singleton<AudioManager>
{
    public enum AudioType
    {
        Master,
        SFX,
        BGM,
    }
    private readonly string audioSOAssetName = "AudioSO";
    private static readonly string themeBGM = "Theme{0}BGM";
    private AudioMixer audioMixer;
    private AudioSource audioBGMSource, audioSFXSource;
    private AudioSO audioSO;
    public void Init()
    {
        LoadAudioSO();
        AudioSource[] sources = Camera.main.GetComponents<AudioSource>();
        foreach (AudioSource source in sources)
        {
            if (source.outputAudioMixerGroup.name == AudioType.SFX.ToString())
            {
                audioSFXSource = source;
            }
            else if (source.outputAudioMixerGroup.name == AudioType.BGM.ToString())
            {
                audioBGMSource = source;
            }
        }
        audioMixer = audioBGMSource.outputAudioMixerGroup.audioMixer;
        var bgmVol = Mathf.Lerp(-80f, 0f, LocalSaveLoadManager.Data.BackGroundVolume);
        var sfxVol = Mathf.Lerp(-80f, 0f, LocalSaveLoadManager.Data.SFXVolume);
        SetVolume(AudioType.BGM, bgmVol);
        SetVolume(AudioType.BGM, sfxVol);
        int theme = PlayerPrefs.GetInt("Theme", 0);
        if(theme > 0)
            Instance.PlayBGM(string.Format(themeBGM, theme));
    }

    private void LoadAudioSO()
    {
        var handle = Addressables.LoadAssetAsync<AudioSO>(audioSOAssetName);
        handle.WaitForCompletion();
        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("AudioSO Load Error!!");
        }
        audioSO = handle.Result;
    }
    public void SetVolume(AudioType audioType, float volume)
    {
        audioMixer.SetFloat(audioType.ToString(), volume);
    }

    public void PlayBGM(string key)
    {
        audioBGMSource.Stop();
        audioBGMSource.clip = audioSO.AudioClips[key];
        audioBGMSource.Play();
    }

    public void PlaySFX(string key)
    {
        audioSFXSource.clip = audioSO.AudioClips[key];
        audioSFXSource.Play();
    }
}