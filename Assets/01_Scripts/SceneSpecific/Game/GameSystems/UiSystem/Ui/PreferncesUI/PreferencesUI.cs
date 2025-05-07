using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PreferencesUI : MonoBehaviour
{
    [SerializeField] private Button changeLangaugeButton;
    [SerializeField] private Button updateButton;
    [SerializeField] private Button contactUsButton;
    [SerializeField] private Slider backgroundSound;
    [SerializeField] private Slider effectSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private PreferencesUpdatePopup updatePopup;
    void Start()
    {
        backgroundSound.onValueChanged.AddListener(BGMVolumSet);
        effectSound.onValueChanged.AddListener(SFXVolumSet);
        updateButton.onClick.AddListener(UpdatePopupSet);
    }
    public void UpdatePopupSet()
    {
        updatePopup.gameObject.SetActive(true);
    }
    public void BGMVolumSet(float vol)
    {

        float volumeDB = Mathf.Lerp(-80f, 0f, vol);
        AudioManager.Instance.SetVolume(AudioManager.AudioType.BGM, volumeDB);
    }
    public void SFXVolumSet(float vol)
    {
        float volumeDB = Mathf.Lerp(-80f, 0f, vol);
        AudioManager.Instance.SetVolume(AudioManager.AudioType.SFX, volumeDB);
    }
}
