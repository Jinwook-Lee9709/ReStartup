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
    [SerializeField] private PreferencesUpdatePopup updatePopup;
    private Coroutine saveCoroutine;
    void Start()
    {
        backgroundSound.onValueChanged.AddListener(BGMVolumSet);
        effectSound.onValueChanged.AddListener(SFXVolumSet);
        updateButton.onClick.AddListener(UpdatePopupSet);
        changeLangaugeButton.onClick.AddListener(ChangeLanguageButtonClick);
        contactUsButton.onClick.AddListener(OpenGmailApp);
    }

    private void Update()
    {
        if(Application.platform == RuntimePlatform.Android)
        {
            if(Input.GetKey(KeyCode.Escape))
            {
                ServiceLocator.Instance.GetSceneService<GameManager>().uiManager.OnClickButtonExitPreferencesUI();
            }
        }
    }
    public void ChangeLanguageButtonClick()
    {
        var sceneManager = ServiceLocator.Instance.GetGlobalService<SceneController>();
        sceneManager.LoadSceneWithLoading(SceneIds.Title);
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
    public void OpenGmailApp()
    {
        //#if UNITY_ANDROID && !UNITY_EDITOR
        if (IsGmailInstalled())
        {
            try
            {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                    using (AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", "android.intent.action.SEND"))
                    {
                        intent.Call<AndroidJavaObject>("setType", "message/rfc822");
                        string[] recipients = new string[] { "psmin5812@email.com" };
                        intent.Call<AndroidJavaObject>("putExtra", "android.intent.extra.EMAIL", recipients);
                        intent.Call<AndroidJavaObject>("putExtra", "android.intent.extra.SUBJECT", "문의드립니다");
                        intent.Call<AndroidJavaObject>("putExtra", "android.intent.extra.TEXT", "안녕하세요.");
                        intent.Call<AndroidJavaObject>("setType", "message/rfc822");

                        // 올바른 chooser 생성
                        using (AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent"))
                        {
                            AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>(
                                "createChooser", intent, "Gmail로 보내기");
                            currentActivity.Call("startActivity", chooser);
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("Gmail 열기 실패: " + e.Message);
            }
            //#endif
        }
        else
        {

        }
    }
    bool IsGmailInstalled()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager");

            try
            {
                AndroidJavaObject packageInfo = packageManager.Call<AndroidJavaObject>(
                    "getPackageInfo", "com.google.android.gm", 0);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #else
        return false;
        #endif
    }
}
