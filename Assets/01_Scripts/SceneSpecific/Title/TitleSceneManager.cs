using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
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
        if (!isLoginSucceed)
        {
            bool isSucceed = await UserAuthController.RefreshToken();
            if (!isSucceed)
                alarmText.text = "Refresh Token Expired, Please Login Again";
        }

        var sceneManager = ServiceLocator.Instance.GetGlobalService<SceneController>();
        sceneManager.LoadSceneWithLoading(SceneIds.Lobby, BeforeLobbySceneLoad);
    }

    private void OnGuestLoginButtonClick()
    {
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
        TokenManager.SaveToken(token.token, token.refreshToken);
    }

    private async UniTask BeforeLobbySceneLoad()
    {
        var response = await CurrencyDataDAC.GetCurrencyData();
        UserDataManager.Instance.CurrentUserData.Money = response.Data.First(x=>x.currencyType == CurrencyType.Money).amount;
        await InitializeStageStatus();
    }

    private async UniTask InitializeStageStatus()
    {
        var stageStatus = await StageStatusDataDAC.GetStageStatusData();
        if (stageStatus.Success && stageStatus.Data.Length == 0)
        {
            await SaveInitialStatusData();
            return;
        }
        UserDataManager.Instance.CurrentUserData.ThemeStatus = stageStatus.Data.ToList();
    }

    private async UniTask SaveInitialStatusData()
    {
        StageStatusData data = CreateStageInitialStatusData();
        var isSucceed = await StageStatusDataDAC.UpdateStageStatusData(data);
        if (!isSucceed)
        {
            //TODO: ReturnToTitleScene
        }
        UserDataManager.Instance.CurrentUserData.ThemeStatus.Add(data);
    }

    private StageStatusData CreateStageInitialStatusData()
    {
        var data = new StageStatusData
        {
            theme = ThemeIds.Theme1,
            isCleared = false,
            lastPlayed = DateTime.Now
        };
        return data;
    }
}