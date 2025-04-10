using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class UserDataManager : Singleton<UserDataManager>
{
    private UserData currentUserData = new();

    private UserDataManager()
    {
        if (currentUserData.Gold == 0) currentUserData.Gold = 1000000000;
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

    public event Action<int?> ChangeGoldAction;
    public event Action<int?> ChangeRankPointAction;
    public event Action<int, int> OnInteriorUpgradeEvent;
    public event Action<int> SetRankingPointAction;
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

    public IEnumerator OnGoldUp(Consumer consumer)
    {
        ModifyGold(consumer.needFood.SellingCost);
        OnRankPointUp(1000);

        yield return new WaitForSeconds(0.5f);

        if (consumer.needFood.FoodID == consumer.FSM.consumerData.LoveFoodId && 0.25f < Random.Range(0f, 1f))
            //TODO : Play Tip PopUp
            CurrentUserData.Gold +=
                Mathf.CeilToInt(consumer.needFood.SellingCost * (consumer.FSM.consumerData.SellTipPercent / 100));
    }

    public void OnRankPointUp(int getRankPoint)
    {
        SetRankingPointAction?.Invoke(getRankPoint);
    }

    public void ModifyGold(int gold)
    {
        CurrentUserData.Gold += gold;
        ChangeRankPointAction?.Invoke(currentUserData.CurrentRankPoint);
        ChangeGoldAction?.Invoke(CurrentUserData.Gold);
    }

    public void UpgradeInterior(int interiorId)
    {
        CurrentUserData.InteriorSaveData[interiorId]++;
        var upgradeCount = CurrentUserData.InteriorSaveData[interiorId];
        OnInteriorUpgradeEvent?.Invoke(interiorId, upgradeCount);
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