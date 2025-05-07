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
    private AudioMixer audioMixer;
    private AudioSource audioBGMSource, audioSFXSource;
    private AudioSO audioSO;
    private Coroutine saveCoroutine;
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
        PlayBGM("Theme1BGM");
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
        switch (audioType)
        {
            case AudioType.Master:
                break;
            case AudioType.SFX:
                LocalSaveLoadManager.Data.SFXVolume = volume;
                break;
            case AudioType.BGM:
                LocalSaveLoadManager.Data.BackGroundVolume = volume;
                break;
        }
        if (saveCoroutine == null)
        {
            saveCoroutine = StartCoroutine(LocalSave());
        }
    }

    public void PlayBGM(string key)
    {
        audioBGMSource.clip = audioSO.AudioClips[key];
        audioBGMSource.Play();
    }

    public void PlaySFX(string key)
    {
        audioSFXSource.clip = audioSO.AudioClips[key]; 
        audioSFXSource.Play();
    }
    private IEnumerator LocalSave()
    {
        yield return new WaitForSeconds(10f);
        LocalSaveLoadManager.Save();
        saveCoroutine = null;
    }
}