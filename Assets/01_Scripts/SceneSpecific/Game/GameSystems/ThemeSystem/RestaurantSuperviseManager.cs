using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class RestaurantSuperviseManager : MonoBehaviour
{
    private readonly string themeBGM = "Theme{0}BGM";
    [SerializeField] RestaurantSuperviseUIManager restaurantSuperviseUIManager;
    private SupervisorCompensationSO compensation;
    private void Start()
    {
        restaurantSuperviseUIManager.InitRestaurantListPanel(OnThemeChanged);
        restaurantSuperviseUIManager.InitSupervisorListPanel(OnHireEmployee);
        compensation = Addressables.LoadAssetAsync<SupervisorCompensationSO>(Strings.CompensationSoKey).WaitForCompletion();
        
    }

    private void OnThemeChanged()
    {
        ThemeChangeTask().Forget();
    }

    private async UniTask ThemeChangeTask()
    {
        float endTime = Time.time + Constants.POP_UP_DURATION;
        
        var currentTheme = ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme;
        var conditionData= DataTableManager.Get<ThemeConditionDataTable>(DataTableIds.ThemeCondition.ToString()).GetConditionData((int)currentTheme + 1);
        
        var popup = ServiceLocator.Instance.GetGlobalService<AlertPopup>();

        var title = LZString.GetUIString("ConnectToServer");
        var message = LZString.GetUIString("MoveToNextTheme");
        popup.PopUp(title, message, SpumCharacter.HireEmployee, false);

        await UserDataManager.Instance.AdjustMoneyWithSave(-conditionData.Requirements1);

        var data = CreateInitialThemeData(currentTheme);
        UserDataManager.Instance.CurrentUserData.ThemeStatus.Add(data.theme, data);
        var isSucceed = await UserDataManager.Instance.UpdateStageStatus(data.theme);
        if (!isSucceed)
            return;
        if (Time.time < endTime)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(endTime - Time.time));
        }

        popup.ClosePopup();

        PlayerPrefs.SetInt("Theme", (int)currentTheme + 1);
        ServiceLocator.Instance.GetGlobalService<SceneController>().LoadSceneWithLoading(SceneIds.Dev3, GameSceneLoader.BeforeGameSceneLoad);

    }

    private static StageStatusData CreateInitialThemeData(ThemeIds currentTheme)
    {
        StageStatusData data = new StageStatusData();
        data.theme = currentTheme + 1;
        data.isCleared = false;
        data.lastClaim = DateTime.Now;
        data.managerCount = 0;
        return data;
    }

    private void OnHireEmployee(int theme, int number)
    {
        var themeStatus = UserDataManager.Instance.CurrentUserData.ThemeStatus[(ThemeIds)theme];
        themeStatus.managerCount = number;
        if (number != 1)
        {
            var ratio = compensation.multipliers[number - 1] / compensation.multipliers[number];
            var currentHour = Math.Clamp(DateTime.Now.Hour - themeStatus.lastClaim.Hour, 0, 24);
            themeStatus.lastClaim = themeStatus.lastClaim.AddHours(currentHour * ratio);
        }
        else
        {
            themeStatus.lastClaim = DateTime.Now;
        }

        UserDataManager.Instance.UpdateStageStatus((ThemeIds)theme).Forget();
        restaurantSuperviseUIManager.UpdateRestaurantButton();
        restaurantSuperviseUIManager.OnSupervisorHire();
    }
}
