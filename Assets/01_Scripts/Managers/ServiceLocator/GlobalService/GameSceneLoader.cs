using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class GameSceneLoader
{
    public static async UniTask BeforeGameSceneLoad()
    {
        try
        {
            UserDataManager.Instance.ResetThemeSave();

            int theme = PlayerPrefs.GetInt("Theme", 1);
            var interiorQueryTask = InteriorSaveDataDAC.GetInteriorData(theme);
            var foodQueryTask = FoodSaveDataDAC.GetFoodData(theme);
            var employeeQueryTask = EmployeeSaveDataDAC.GetEmployeeData(theme);
            var themeRecordQueryTask = ThemeRecordDAC.GetThemeRecordData(theme);
            var promotionQueryTask = PromotionDataDAC.GetPromotionData();
            var buffQueryTask = BuffSaveDataDAC.GetAllBuffSaveData();
            var reviewQueryTask = ReviewSaveDataDAC.GetReviewSaveData();
            var missionQueryTask = MissionSaveDataDAC.GetAllMissionSaveData();

            var getInteriorResponse = await interiorQueryTask;
            var getFoodResponse = await foodQueryTask;
            var getEmployeeResponse = await employeeQueryTask;
            var themeRecordResponse = await themeRecordQueryTask;
            var promotionResponse = await promotionQueryTask;
            var buffResponse = await buffQueryTask;
            var reviewResponse = await reviewQueryTask;
            var missionResponse = await missionQueryTask;

            if (getInteriorResponse == null || getFoodResponse == null || getEmployeeResponse == null ||
                themeRecordResponse == null || promotionResponse == null || buffResponse == null ||
                reviewResponse == null)
                throw new NullReferenceException();

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

            if (getFoodResponse?.Data.Length == 0)
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

            if (promotionResponse.Data.Length == 0)
            {
                await SaveInitialPromotionData();
            }
            else
            {
                foreach (var data in promotionResponse.Data)
                {
                    UserDataManager.Instance.CurrentUserData.PromotionSaveData.Add(data.id, data);
                }
            }

            if (buffResponse.Data.Length != 0)
            {
                foreach (var data in buffResponse?.Data)
                {
                    UserDataManager.Instance.CurrentUserData.BuffSaveData.Add(data.id, data);
                }
            }

            if (reviewResponse.Data.Length != 0)
            {
                UserDataManager.Instance.CurrentUserData.ReviewSaveData.Clear();
                foreach (var data in reviewResponse.Data.OrderBy(x => x.orderIndex))
                {
                    DateTime kstNow = data.createdTime.AddHours(9);
                    data.createdTime = kstNow;
                    UserDataManager.Instance.CurrentUserData.ReviewSaveData.Add(data);
                }
            }

            if (missionResponse.Data.Length != 0)
            {
                foreach (var data in missionResponse.Data)
                {
                    UserDataManager.Instance.CurrentUserData.MissionSaveData.Add(data.id, data);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private static async UniTask SaveInitialInteriorData(int theme)
    {
        var payload = new List<InteriorSaveData>();
        var table = DataTableManager.Get<InteriorDataTable>(DataTableIds.Interior.ToString());
        var query = table.Where(x => x.RestaurantType == theme && x.DefaultFurniture);
        foreach (var item in query)
        {
            InteriorSaveData data = new InteriorSaveData();
            data.id = item.InteriorID;
            data.theme = (ThemeIds)theme;
            data.level = 1;
            payload.Add(data);
        }

        if (payload.Count == 0)
            return;

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

    private static async UniTask SaveInitialFoodData(int theme)
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

    private static async UniTask SaveInitialRecordData(int theme)
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

    private static async UniTask SaveInitialPromotionData()
    {
        var table = DataTableManager.Get<PromotionDataTable>(DataTableIds.Promoiton.ToString());
        List<PromotionData> payload = new();
        foreach (PromotionBase data in table)
        {
            var promotionData = new PromotionData
            {
                id = data.PromotionID,
                buyUseCount = 0,
                adUseCount = 0,
            };
            payload.Add(promotionData);
        }

        var result = await PromotionDataDAC.UpdatePromotionData(payload);
        if (!result)
        {
        }
        else
        {
            foreach (var data in payload)
            {
                UserDataManager.Instance.CurrentUserData.PromotionSaveData.Add(data.id, data);
            }
        }
    }
}