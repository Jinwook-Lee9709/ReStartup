using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEditor.Progress;
using Random = UnityEngine.Random;

public class UserDataManager : Singleton<UserDataManager>
{
    private UserData currentUserData = new();

    public event Action<int?> getGoldAction;
    public event Action<int> setRankingPointAction;
    public event Action<bool> OnReviewCntFullEvent;

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
        CurrentUserData.Gold += consumer.needFood.SellingCost;
        getGoldAction.Invoke(CurrentUserData.Gold);
        setRankingPointAction.Invoke(1000);

        yield return new WaitForSeconds(0.5f);

        if (consumer.needFood.FoodID == consumer.FSM.consumerData.LoveFoodId && 0.25f < Random.Range(0f, 1f))
        {    //TODO : Play Tip PopUp
            CurrentUserData.Gold +=
                Mathf.CeilToInt(consumer.needFood.SellingCost * (consumer.FSM.consumerData.SellTipPercent / 100));
        }
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
                if (Random.Range(0f, 1f) < 0.6f)
                {
                    OnReviewCntFullEvent?.Invoke(isPositive);
                }
                currentUserData.NegativeCnt = 0;
            }
        }
    }
}