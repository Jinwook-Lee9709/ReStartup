using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class UserDataManager : Singleton<UserDataManager>
{
    private UserData currentUserData = new();

    private UserDataManager()
    {
        if (currentUserData.Money == 0) currentUserData.Money = 1000000000;
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
    public event Action<int> ChangeRankPointAction;
    public event Action<int, int> OnInteriorUpgradeEvent;
    public event Action<bool> OnReviewCntFullEvent;

    public bool SaveDB()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> LoadDB(string uid = "1234")
    {
        var result = await UserDataDAC.GetUserData(uid);
        CurrentUserData = result;
        return true;
    }

    public void InitCurrentUserData()
    {
        StartCoroutine(LoadRoutine());
    }

    private IEnumerator LoadRoutine()
    {
        var task = LoadDB();
        while (!task.IsCompleted) yield return new WaitForEndOfFrame();
        if (task.Result)
            Debug.Log("Load DB Success");
        else
            Debug.Log("Load DB Fail");
    }

    public IEnumerator OnMoneyUp(Consumer consumer)
    {
        AdjustMoneyWithSave(consumer.needFood.SellingCost).Forget();
        AddRankPointWithSave(1000).Forget();

        yield return new WaitForSeconds(0.5f);

        if (consumer.needFood.FoodID == consumer.FSM.consumerData.LoveFoodId && 0.25f < Random.Range(0f, 1f))
            //TODO : Play Tip PopUp
            CurrentUserData.Money +=
                Mathf.CeilToInt(consumer.needFood.SellingCost * (consumer.FSM.consumerData.SellTipPercent / 100));
    }

    public void OnRankPointUp(int getRankPoint)
    {
        currentUserData.CurrentRankPoint += getRankPoint;
        ChangeRankPointAction?.Invoke(getRankPoint);
    }
    
    public async UniTask AddRankPointWithSave(int rankPoint)
    {
        var currentTheme = ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme;
        OnRankPointUp(rankPoint);
        await ThemeRecordDAC.UpdateThemeRankpoint((int)currentTheme, rankPoint);
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

    public async UniTask UpgradeInterior(int interiorId)
    {
        CurrentUserData.InteriorSaveData[interiorId]++;
        var upgradeCount = CurrentUserData.InteriorSaveData[interiorId];
        await SaveInteriorUpgrade(interiorId);
       
        OnInteriorUpgradeEvent?.Invoke(interiorId, upgradeCount);
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

    public async UniTask UpgradeEmployee (int staffId)
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

    public void AddConsumerCnt(bool isPositive)
    {
        if (isPositive)
        {
            currentUserData.PositiveCnt++;
            if (currentUserData.PositiveCnt >= 15)
            {
                OnReviewCntFullEvent?.Invoke(isPositive);
                currentUserData.PositiveCnt = 0;
            }
        }
        else
        {
            currentUserData.NegativeCnt++;
            if (currentUserData.NegativeCnt >= 4)
            {
                if (Random.Range(0f, 1f) < 0.6f) OnReviewCntFullEvent?.Invoke(isPositive);
                currentUserData.NegativeCnt = 0;
            }
        }
    }
}