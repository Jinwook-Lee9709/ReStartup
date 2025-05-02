using System;

using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Random = UnityEngine.Random;

public class UserDataManager : Singleton<UserDataManager>
{
    private UserData currentUserData = new();
    private UserDataManager()
    {
    }

    public UserData CurrentUserData
    {
        get
        {
            if (currentUserData == null)
                currentUserData = new UserData();
            return currentUserData;
        }
        set
        {
            if (value == null) throw new ArgumentNullException("currentUserInfo Set Value");
            currentUserData = value;
        }
    }

    public event Action<int?> ChangeMoneyAction;
    public event Action<int?> ChangeGoldAction;
    public event Action<int> ChangeRankPointAction;
    public event Action<int, int> OnInteriorUpgradeEvent;
    public event Action<bool> OnReviewCntFullEvent;

    public event Action<int> OnRankChangedEvent;

    public void OnRankPointUp(int getRankPoint)
    {
        currentUserData.CurrentRankPoint += getRankPoint;
        ChangeRankPointAction?.Invoke((int)currentUserData.CurrentRankPoint);
    }

    public async UniTask<bool> AddRankPointWithSave(int rankPoint)
    {
        var currentTheme = ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme;
        OnRankPointUp(rankPoint);
        var response = await ThemeRecordDAC.UpdateThemeRankpoint((int)currentTheme, (int)currentUserData.CurrentRankPoint);
        return response;
    }

    public void GetTotalRankPoint()
    {
        
    }

    public async UniTask SetRankWithSave(int rank)
    {
        var currentTheme = ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme;
        CurrentUserData.CurrentRank = rank;
        await ThemeRecordDAC.UpdateThemeRank((int)currentTheme, rank);
        OnRankChangedEvent?.Invoke(rank);
    }

    public void AdjustMoney(int money)
    {
        CurrentUserData.Money += money;
        ChangeMoneyAction?.Invoke(CurrentUserData.Money);
    }

    public async UniTask AdjustMoneyWithSave(int money)
    {
        AdjustMoney(money);
        await SaveMoneyData();
    }

    public async UniTask SaveMoneyData()
    {
        List<CurrencyData> list = new();
        if (CurrentUserData.Money == null)
            return;
        CurrencyData data = new CurrencyData(CurrencyType.Money, (int)CurrentUserData.Money);
        list.Add(data);
        await CurrencyDataDAC.UpdateCurrencyData(list);
    }

    public void AdjustGold(int gold)
    {
        CurrentUserData.Gold += gold;
        ChangeGoldAction?.Invoke(CurrentUserData.Gold);
    }

    public async UniTask AdjustGoldWithSave(int gold)
    {
        AdjustGold(gold);
        await SaveGoldData();
    }

    public async UniTask SaveGoldData()
    {
        List<CurrencyData> list = new();
        CurrencyData data = new CurrencyData(CurrencyType.Gold, CurrentUserData.Gold);
        list.Add(data);
        await CurrencyDataDAC.UpdateCurrencyData(list);
    }



    public async UniTask UpgradeInterior(int interiorId)
    {
        CurrentUserData.InteriorSaveData[interiorId]++;
        var upgradeCount = CurrentUserData.InteriorSaveData[interiorId];
        await SaveInteriorUpgrade(interiorId);

        OnInteriorUpgradeEvent?.Invoke(interiorId, upgradeCount);
    }

    public async UniTask<bool> UpdateStageStatus(ThemeIds themeId)
    {
        var dataToUpload = CurrentUserData.ThemeStatus[themeId];
        var result = await StageStatusDataDAC.UpdateStageStatusData(dataToUpload);
        return result;
    }

    public async UniTask SaveInteriorUpgrade(int interiorId)
    {
        var table = DataTableManager.Get<InteriorDataTable>(DataTableIds.Interior.ToString());
        var theme = table.GetData(interiorId).RestaurantType;
        InteriorSaveData data = new InteriorSaveData();
        data.id = interiorId;
        data.theme = (ThemeIds)theme;
        data.level = CurrentUserData.InteriorSaveData[interiorId];
        await InteriorSaveDataDAC.UpdateInteriorData(data);
    }

    public async UniTask UpgradeEmployee(int staffId)
    {
        currentUserData.EmployeeSaveData[staffId].level++;
        var payload = currentUserData.EmployeeSaveData[staffId];
        await EmployeeSaveDataDAC.UpdateEmployeeData(payload);
    }

    public async UniTask UpgradeFood(int foodId)
    {
        currentUserData.FoodSaveData[foodId].level++;
        var payload = currentUserData.FoodSaveData[foodId];
        await FoodSaveDataDAC.UpdateFoodData(payload);
    }

    public async UniTask OnSellingFood(int foodId)
    {
        currentUserData.FoodSaveData[foodId].sellCount++;
        var payload = currentUserData.FoodSaveData[foodId];
        await FoodSaveDataDAC.UpdateFoodData(payload);
    }

    public async UniTask<bool> OnUsePromotion(PromotionBase promotion, bool isAd)
    {
        if (isAd)
            CurrentUserData.PromotionSaveData[promotion.PromotionID].adUseCount++;
        else
            CurrentUserData.PromotionSaveData[promotion.PromotionID].buyUseCount++;
        List<PromotionData> payload = new() { CurrentUserData.PromotionSaveData[promotion.PromotionID] };
        var result = await PromotionDataDAC.UpdatePromotionData(payload);
        return result;
    }

    public async UniTask<bool> OnUseBuff(Buff buff)
    {
        BuffSaveData buf = new BuffSaveData();
        buf.id = buff.BuffID;
        buf.type = buff.BuffType;
        buf.remainTime = buff.remainBuffTime;
        var result = await BuffSaveDataDAC.UpdateBuffSaveData(buf);
        return result;
    }

    public async UniTask<bool> SaveRemainBuffTime(Buff buff, float buffTime)
    {
        BuffSaveData buf = new BuffSaveData();
        buf.id = buff.BuffID;
        buf.type = buff.BuffType;
        buf.remainTime = buffTime;
        var result = await BuffSaveDataDAC.UpdateBuffSaveData(buf);
        return result;
    }

    public async UniTask<bool> OnBuffExpired(Buff buff)
    {
        var result = await BuffSaveDataDAC.DeleteBuffSaveData(buff.BuffType);
        return result;
    }

    public void AddConsumerCnt(bool isPositive)
    {
        if (isPositive)
        {
            currentUserData.PositiveCnt++;
            if (currentUserData.PositiveCnt >= 15)
            {
                OnReviewCntFullEvent?.Invoke(isPositive);
                currentUserData.PositiveCnt = 0;
                ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager.OnEventInvoked(MissionMainCategory.GoodReview, 1);
                ReviewCountUp(isPositive);
            }
        }
        else
        {
            currentUserData.NegativeCnt++;
            if (currentUserData.NegativeCnt >= 4)
            {
                if (Random.Range(0f, 1f) < 0.6f) OnReviewCntFullEvent?.Invoke(isPositive);
                ReviewCountUp(isPositive);
                currentUserData.NegativeCnt = 0;
            }
        }
    }

    public void ReviewCountUp(bool isPositive)
    {
        var currentTheme = ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme;
        if (isPositive)
        {
            if (currentUserData.reviewCountForTheme.TryGetValue(currentTheme, out var counts))
            {
                counts.positive++;
                currentUserData.reviewCountForTheme[currentTheme] = (counts.positive, counts.negative);
            }
        }
        else
        {
            if (currentUserData.reviewCountForTheme.TryGetValue(currentTheme, out var counts))
            {
                counts.negative++;
                currentUserData.reviewCountForTheme[currentTheme] = (counts.positive, counts.negative);
            }
        }
    }

    public void OnNegativeReviewRemove()
    {
        var currentTheme = ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme;
        if (currentUserData.reviewCountForTheme.TryGetValue(currentTheme, out var counts))
        {
            counts.negative--;
            currentUserData.reviewCountForTheme[currentTheme] = (counts.positive, counts.negative);
        }
    }

    public void ResetThemeSave()
    {
        CurrentUserData.InteriorSaveData.Clear();
        CurrentUserData.FoodSaveData.Clear();
        CurrentUserData.EmployeeSaveData.Clear();
        CurrentUserData.PromotionSaveData.Clear();
        CurrentUserData.BuffSaveData.Clear();
        CurrentUserData.ReviewSaveData.Clear();
        CurrentUserData.Cumulative = 0;
        CurrentUserData.CurrentRank = 0;
        CurrentUserData.CurrentRankPoint = 0;
    }
}