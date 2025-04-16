using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbySceneManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private List<Button> themeSelectButtons;
    
    
    void Start()
    {
        var goldText = UserDataManager.Instance.CurrentUserData.Money.ToString();
        text.text = $"Money = {goldText}";
        for (int i = 0; i < themeSelectButtons.Count; i++)
        {
            var button = themeSelectButtons[i];
            int id = i+1;
            button.onClick.AddListener(() => OnThemeSelectButtonTouched(id));
        }
        
    }

    private void OnThemeSelectButtonTouched(int id)
    {
        PlayerPrefs.SetInt("Theme", id);
        var sceneManager = ServiceLocator.Instance.GetGlobalService<SceneController>();
        sceneManager.LoadSceneWithLoading(SceneIds.Dev2, BeforeGameSceneLoad);
    }

    private async UniTask BeforeGameSceneLoad()
    {
        int theme = PlayerPrefs.GetInt("Theme", 1);
        var interiorQueryTask =  InteriorSaveDataDAC.GetInteriorData(theme);
        var foodQueryTask =  FoodSaveDataDAC.GetFoodData(theme);
        var employeeQueryTask =  EmployeeSaveDataDAC.GetEmployeeData(theme);
        var themeRecordQueryTask = ThemeRecordDAC.GetThemeRecordData(theme);
        
        var getInteriorResponse = await interiorQueryTask;
        var getFoodResponse = await foodQueryTask;
        var getEmployeeResponse = await employeeQueryTask;
        var themeRecordResponse = await themeRecordQueryTask;
        
        if (getInteriorResponse.Data.Length == 0)
        {
            await SaveInitialInteriorData(theme);
        }
        else
        {
            foreach (var item in getInteriorResponse.Data)
            {
                UserDataManager.Instance.CurrentUserData.InteriorSaveData[item.id] = item.level;
            }
        }

        if (getFoodResponse.Data.Length == 0)
        {
            await SaveInitialFoodData(theme);
        }
        else
        {
            foreach (var item in getFoodResponse.Data)
            {
                UserDataManager.Instance.CurrentUserData.FoodSaveData[item.id] = item;
            }
        }

        foreach (var item in getEmployeeResponse.Data)
        {
            UserDataManager.Instance.CurrentUserData.EmployeeSaveData[item.id] = item;
        }

        if (themeRecordResponse.Data.Length == 0)
        {
            await SaveInitialRecordData(theme);
        }
        else
        {
            UserDataManager.Instance.CurrentUserData.CurrentRank = themeRecordResponse.Data[0].ranking;
            UserDataManager.Instance.CurrentUserData.CurrentRankPoint = themeRecordResponse.Data[0].rank_point;
            UserDataManager.Instance.CurrentUserData.Cumulative = themeRecordResponse.Data[0].cumulative;
        }
    }

    private async UniTask SaveInitialInteriorData(int theme)
    {
        var payload = new List<InteriorSaveData>();
        var table = DataTableManager.Get<InteriorDataTable>(DataTableIds.Interior.ToString());
        var query = table.Where(x => x.RestaurantType == theme && x.SellingCost == 0);
        foreach (var item in query)
        {
            InteriorSaveData data = new InteriorSaveData();
            data.id = item.InteriorID;
            data.theme = (ThemeIds)theme;
            data.level = 1;
            payload.Add(data);
        }
        var result = await InteriorSaveDataDAC.UpdateInteriorData(payload);
        if (!result)
        {
            //TODO: ReturnToTitle
        }

        foreach (var item in query)
        {
            UserDataManager.Instance.CurrentUserData.InteriorSaveData[item.InteriorID] = 1;
        }
    }
    
    private async UniTask SaveInitialFoodData(int theme)
    {
        var table = DataTableManager.Get<FoodDataTable>(DataTableIds.Food.ToString());
        var data = table.First(x => x.Type == theme && x.Requirements == 0);
        FoodSaveData payload = new FoodSaveData();
        payload.id = data.FoodID;
        payload.level = 1;
        payload.theme = (ThemeIds)theme;
        payload.sellCount = 0;
        var result = await FoodSaveDataDAC.UpdateFoodData(payload);

        if (!result)
        {
            
        }
        else
        {
            UserDataManager.Instance.CurrentUserData.FoodSaveData[payload.id] = payload;
        }
        
    }

    private async UniTask SaveInitialRecordData(int theme)
    {
        ThemeRecordData payload = new ThemeRecordData();
        payload.theme = theme;
        payload.cumulative = 0;
        payload.rank_point = 0;
        payload.ranking = 1;
        var result = await ThemeRecordDAC.UpdateThemeRecordData(payload);
        if (!result)
        {
            
        }
        UserDataManager.Instance.CurrentUserData.CurrentRank = 1;
        UserDataManager.Instance.CurrentUserData.CurrentRankPoint = 0;
        UserDataManager.Instance.CurrentUserData.Cumulative = 0;
    }
    
}
