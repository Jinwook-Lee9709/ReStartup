using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button guestLoginButton;
    [SerializeField] private Button resetUserButton;
    [SerializeField] private Button loginButton;

    [SerializeField] private Button closeButton;
    [SerializeField] private Button testButton;
    [SerializeField] private TextMeshProUGUI alarmText;
    [SerializeField] private Button koreanChangeButton;
    [SerializeField] private Button englishChangeButton;
    [SerializeField] private Button languageSettingButton;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject koreanCheck;
    [SerializeField] private GameObject englishCheck;
    [SerializeField] private GameObject languageSettingInObject;
    
    
    private void Awake()
    {
        SetInitialStatus();
    }

    private void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClick);
        guestLoginButton.onClick.AddListener(OnGuestLoginButtonClick);
        loginButton.onClick.AddListener(RetryLogin);
        koreanChangeButton.onClick.AddListener(SwitchToKorean);
        englishChangeButton.onClick.AddListener(SwitchToEnglish);
        languageSettingButton.onClick.AddListener(OpenOrExitSetting);
        closeButton.onClick.AddListener(OpenOrExitSetting);
        BgmPlayTask().Forget();
    }
    
    private async void SetInitialStatus()
    {
        if (!GuestLoginManager.ReadUUID())
        {
            startButton.gameObject.SetActive(false);
            guestLoginButton.gameObject.SetActive(true);
            return;
        }

        var isSucceed = await LoginAsGuest();
        if (!isSucceed)
        {
            PopupLoginFailAlert().Forget();
            startButton.gameObject.SetActive(true);
            startButton.interactable = false;
            guestLoginButton.gameObject.SetActive(false);
            loginButton.gameObject.SetActive(true);
            return;
        }
        
        if (!TokenManager.ReadToken())
        {
            startButton.gameObject.SetActive(false);
            guestLoginButton.gameObject.SetActive(true);
            guestLoginButton.interactable = true;
            return;
        }

        startButton.gameObject.SetActive(true);
        startButton.interactable = true;
        guestLoginButton.gameObject.SetActive(false);
        loginButton.gameObject.SetActive(false);
    }

    private async UniTask PopupLoginFailAlert()
    {
        await UniTask.Yield();
        var title = LZString.GetUIString("LoginFailTitle");
        var message = LZString.GetUIString("LoginFailDescription");
        var popup = ServiceLocator.Instance.GetGlobalService<AlertPopup>();
        Debug.Log(popup==null);
        popup?.PopUp(title, message, isError: true);
    }

    private async UniTask BgmPlayTask()
    {
        await UniTask.Yield();
        AudioManager.Instance.PlayBGM("TitleBGM");
    }


    private void OnStartButtonClick()
    {
        GameStartTask().Forget();
    }

    private void RetryLogin()
    {
        SetInitialStatus();
    }

    private async UniTask GameStartTask()
    {
        startButton.interactable = false;
        await LoginAsGuest();
        if (!TokenManager.ReadToken())
        {
            //팝업 필요
            startButton.interactable = true;
            return;
        }

        bool isLoginSucceed = await UserAuthController.VerifyToken();
        alarmText.text = "Login Succeed";
        if (!isLoginSucceed)
        {
            bool isSucceed = await UserAuthController.RefreshToken();
            if (!isSucceed)
                alarmText.text = "Refresh Token Expired, Please Login Again";
            startButton.interactable = true;
            return;
        }

        GameSceneLoadTask().Forget();

        // var sceneManager = ServiceLocator.Instance.GetGlobalService<SceneController>();
        // sceneManager.LoadSceneWithLoading(SceneIds.Lobby, BeforeLobbySceneLoad);
    }

    private void OnGuestLoginButtonClick()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            var title = LZString.GetUIString("NetworkFailureAlertTitle");
            var message = LZString.GetUIString("NetworkFailureAlertDescription");
            ServiceLocator.Instance.GetGlobalService<AlertPopup>().PopUp(title, message, isError: true);
            return;
        }

        if (!GuestLoginManager.ReadUUID())
        {
            RegisterAsGuest().Forget();
        }
        else
        {
            LoginAsGuest().Forget();
        }
    }

    private async UniTask RegisterAsGuest()
    {
        var isSucceed = await UserAuthController.RegisterAsGuestTask();
        if (isSucceed)
            resetUserButton.interactable = true;
        
        startButton.gameObject.SetActive(true);
        guestLoginButton.gameObject.SetActive(false);
    }

    private async UniTask<bool> LoginAsGuest()
    {
        var token = await UserAuthController.LoginAsGuestTask();
        if (token == null)
            return false;
        TokenManager.SaveToken(token.token, token.refreshToken);
        return true;
    }

    private async UniTask BeforeLobbySceneLoad()
    {
        var nameResponse = await UserDataDAC.GetUserName();
        var response = await CurrencyDataDAC.GetCurrencyData();
        UserDataManager.Instance.CurrentUserData.Gold =
            response.Data.First(x => x.currencyType == CurrencyType.Gold).amount;
        UserDataManager.Instance.CurrentUserData.Money =
            response.Data.First(x => x.currencyType == CurrencyType.Money).amount;
        UserDataManager.Instance.CurrentUserData.Name = nameResponse.Data;
        await InitializeStageStatus();
    }

    private async UniTask GameSceneLoadTask()
    {
        var nameResponse = await UserDataDAC.GetUserName();
        var response = await CurrencyDataDAC.GetCurrencyData();
        UserDataManager.Instance.CurrentUserData.Gold =
            response.Data.First(x => x.currencyType == CurrencyType.Gold).amount;
        UserDataManager.Instance.CurrentUserData.Money =
            response.Data.First(x => x.currencyType == CurrencyType.Money).amount;
        UserDataManager.Instance.CurrentUserData.AdTicket =
            response.Data.First(x => x.currencyType == CurrencyType.AdTicket).amount;
        UserDataManager.Instance.CurrentUserData.Name = nameResponse.Data;

        var stageStatus = await StageStatusDataDAC.GetStageStatusData();
        ThemeIds currentTheme = ThemeIds.Theme1;
        if (stageStatus.ResponseCode == ResponseType.Success && stageStatus.Data.Length == 0)
        {
            await SaveInitialStatusData();
            currentTheme = ThemeIds.Theme1;
        }
        else
        {
            foreach (var data in stageStatus.Data)
            {
                UserDataManager.Instance.CurrentUserData.ThemeStatus[data.theme] = data;
            }

            currentTheme = stageStatus.Data.Max(x => x.theme);
            PlayerPrefs.SetInt("Theme", (int)currentTheme);
        }

        PlayerPrefs.SetInt("Theme", (int)currentTheme);
        var sceneManager = ServiceLocator.Instance.GetGlobalService<SceneController>();
        sceneManager.LoadSceneWithLoading(SceneIds.Dev0, GameSceneLoader.BeforeGameSceneLoad);
        startButton.interactable = true;
    }

    private async UniTask InitializeStageStatus()
    {
        var stageStatus = await StageStatusDataDAC.GetStageStatusData();
        if (stageStatus.ResponseCode == ResponseType.Success && stageStatus.Data.Length == 0)
        {
            await SaveInitialStatusData();
            return;
        }

        if (stageStatus.ResponseCode != ResponseType.Success)
        {
            alarmText.text = "Stage Status Data Load Failed";
        }

        foreach (var data in stageStatus.Data)
        {
            UserDataManager.Instance.CurrentUserData.ThemeStatus[data.theme] = data;
        }
    }

    private async UniTask SaveInitialStatusData()
    {
        StageStatusData data = CreateStageInitialStatusData();
        var isSucceed = await StageStatusDataDAC.UpdateStageStatusData(data);
        if (!isSucceed)
        {
            //TODO: ReturnToTitleScene
        }

        UserDataManager.Instance.CurrentUserData.ThemeStatus.Add(data.theme, data);
    }

    private StageStatusData CreateStageInitialStatusData()
    {
        var data = new StageStatusData
        {
            theme = ThemeIds.Theme1,
            isCleared = false,
            lastClaim = DateTime.Now,
            managerCount = 0,
        };
        return data;
    }

    public void SwitchToKorean()
    {
        if (SetLocale("ko"))
        {
            LocalSaveLoadManager.Data.LanguageType = LanguageType.Korean;
            LocalSaveLoadManager.Save();

            koreanCheck.gameObject.SetActive(true);
            englishCheck.gameObject.SetActive(false);
        }
    }

    public void SwitchToEnglish()
    {
        if (SetLocale("en"))
        {
            LocalSaveLoadManager.Data.LanguageType = LanguageType.English;
            LocalSaveLoadManager.Save();

            koreanCheck.gameObject.SetActive(false);
            englishCheck.gameObject.SetActive(true);
        }
    }

    private bool SetLocale(string localeCode)
    {
        Locale newLocale = null;

        foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
        {
            if (locale.Identifier.Code == localeCode)
            {
                newLocale = locale;
                break;
            }
        }

        if (newLocale != null)
        {
            LocalizationSettings.SelectedLocale = newLocale;
            Debug.Log($"Locale switched to: {newLocale.LocaleName}");
            return true;
        }
        else
        {
            Debug.LogWarning($"Locale with code {localeCode} not found.");
            return false;
        }
    }

    private void OpenOrExitSetting()
    {
        if (languageSettingInObject.activeSelf)
        {
            closeButton.interactable = false;
            var image = closeButton.GetComponent<Image>();
            settingPanel.transform.PopdownAnimation();
            image.FadeOutAnimation(
                onComplete: () =>
                {
                    closeButton.interactable = true;
                    languageSettingInObject.SetActive(false);
                }
            );
        }
        else
        {
            languageSettingInObject.SetActive(true);
            closeButton.interactable = false;
            var image = closeButton.GetComponent<Image>();
            settingPanel.transform.PopupAnimation(
                onComplete: () =>
                {
                    closeButton.interactable = true;
                });
            image.FadeInAnimation();
        }
    }
}