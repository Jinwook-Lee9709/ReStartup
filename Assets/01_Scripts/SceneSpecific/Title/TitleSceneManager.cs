using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    
    
    [FormerlySerializedAs("button")] [SerializeField]
    private Button startButton;

    [SerializeField] private Button guestLoginButton;
    [SerializeField] private Button testButton;
    [SerializeField] private TextMeshProUGUI alarmText;


    private void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClick);
        guestLoginButton.onClick.AddListener(OnGuestLoginButtonClick);
    }
    

    private void OnStartButtonClick()
    {
        startButton.interactable = false;
        GameStartTask().Forget();
    }

    private async UniTask GameStartTask()
    {
        if (!GuestLoginManager.ReadUUID())
        {
            alarmText.text = "Need To Register Fisrt";
            return;
        }

        if (!TokenManager.ReadToken())
        {
            alarmText.text = "Need To Login Fisrt";
            return;
        }
        bool isLoginSucceed = await UserAuthController.VerifyToken();
        alarmText.text = "Login Succeed";
        if (!isLoginSucceed)
        {
            bool isSucceed = await UserAuthController.RefreshToken();
            if (!isSucceed)
                alarmText.text = "Refresh Token Expired, Please Login Again";
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
    }

    private async UniTask LoginAsGuest()
    {
        var token = await UserAuthController.LoginAsGuestTask();
        if (token == null)
            return;
        TokenManager.SaveToken(token.token, token.refreshToken);
    }

    private async UniTask BeforeLobbySceneLoad()
    {
        var nameResponse = await UserDataDAC.GetUserName();
        var response = await CurrencyDataDAC.GetCurrencyData();
        UserDataManager.Instance.CurrentUserData.Gold = response.Data.First(x=>x.currencyType == CurrencyType.Gold).amount;
        UserDataManager.Instance.CurrentUserData.Money = response.Data.First(x=>x.currencyType == CurrencyType.Money).amount;
        UserDataManager.Instance.CurrentUserData.Name = nameResponse.Data;
        await InitializeStageStatus();
    }

    private async UniTask GameSceneLoadTask()
    {
        var nameResponse = await UserDataDAC.GetUserName();
        var response = await CurrencyDataDAC.GetCurrencyData();
        UserDataManager.Instance.CurrentUserData.Gold = response.Data.First(x=>x.currencyType == CurrencyType.Gold).amount;
        UserDataManager.Instance.CurrentUserData.Money = response.Data.First(x=>x.currencyType == CurrencyType.Money).amount;
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
}