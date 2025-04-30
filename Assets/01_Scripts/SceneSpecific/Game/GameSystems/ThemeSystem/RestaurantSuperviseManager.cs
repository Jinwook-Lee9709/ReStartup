using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class RestaurantSuperviseManager : MonoBehaviour
{
    [SerializeField] RestaurantSuperviseUIManager restaurantSuperviseUIManager;
    private void Start()
    {
        restaurantSuperviseUIManager.InitRestaurantListPanel(OnThemeChanged);
        restaurantSuperviseUIManager.InitSupervisorListPanel(OnHireEmployee);
    }

    private void OnThemeChanged()
    {
        ThemeChangeTask().Forget();
    }

    private async UniTask ThemeChangeTask()
    {
        float endTime = Time.time + Constants.POP_UP_DURATION;

        var popup = ServiceLocator.Instance.GetGlobalService<AlertPopup>();
        popup.PopUp("서버와 통신중", "다음 식당으로 이동!", SpumCharacter.HireEmployee, false);
        
        var currentTheme = ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme;
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
        ServiceLocator.Instance.GetGlobalService<SceneController>().LoadSceneWithLoading(SceneIds.Dev0, GameSceneLoader.BeforeGameSceneLoad);

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
        
    }
}
