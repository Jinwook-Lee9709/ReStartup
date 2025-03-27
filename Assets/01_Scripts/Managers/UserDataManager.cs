using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class UserDataManager : Singleton<UserDataManager>
{
    private UserData currentUserData = new();

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

        yield return new WaitForSeconds(0.5f);

        if (consumer.FSM.consumerData.TempTipProb < Random.Range(0f, 1f))
            //TODO : Play Tip PopUp
            CurrentUserData.Gold +=
                Mathf.CeilToInt(consumer.needFood.SellingCost * (consumer.FSM.consumerData.SellTipPercent / 100));
    }
}