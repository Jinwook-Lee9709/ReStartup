using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class RankingGlobalController : MonoBehaviour
{
    private static readonly string PrefabName = "RankingGlobalItem";
    private static readonly int MaxRanking = 50;
    
    [SerializeField] private Transform contents;
    [SerializeField] private GlobalPlayerClone globalPlayerClone;
    [SerializeField] private RectTransform canvas;
    
    private List<RankingGlobalUiItem> rankingGlobalUiItems = new List<RankingGlobalUiItem>();
    private bool isFirstLoad = true;

    private int userRank = int.MaxValue;
    
    public void Awake()
    {
        List<AsyncOperationHandle<GameObject>> handleList = new();
        for (int i = 0; i < 50; i++)
        {
            var handle = Addressables.InstantiateAsync(PrefabName, contents);
            handleList.Add(handle);
        }

        foreach (var handle in handleList)
        {
            handle.WaitForCompletion();
            var item = handle.Result.GetComponent<RankingGlobalUiItem>();
            rankingGlobalUiItems.Add(item);
            item.gameObject.SetActive(false);
        }
    }

    public void OnEnable()
    {
        if(!isFirstLoad)
            globalPlayerClone.OnActive();
        LoadRanking().Forget();
    }

    private async UniTask LoadRanking()
    {
        if (isFirstLoad)
        {
            isFirstLoad = false;
            await LoadRankingWithPopup();
            return;
        }

        await LoadDataAndUpdateUI();
    }

    private async UniTask LoadRankingWithPopup()
    {
        float targetTime = Time.time + Constants.POP_UP_DURATION;
        ShowLoadingPopup();
        var response = await LoadDataFromServer();
        var userResponse = await LoadUserDataFromServer();
        if (userResponse.ResponseCode != ResponseType.Success || response.ResponseCode != ResponseType.Success
                                                              || userResponse.Data == null || response.Data == null)
            return;
        if (Time.time < targetTime)
        {
            await UniTask.WaitForSeconds(targetTime - Time.time);
        }

        userRank = userResponse.Data.rank;
        globalPlayerClone.UpdatePlayerData(userResponse.Data);
        globalPlayerClone.SetRankerCount(response.Data.Length);
        SetInfo(response.Data);
        CloseLoadingPopup();
    }

    private async UniTask LoadDataAndUpdateUI()
    {
        var response = await LoadDataFromServer();
        var userResponse = await LoadUserDataFromServer();
        if (userResponse.ResponseCode != ResponseType.Success || response.ResponseCode != ResponseType.Success
            || userResponse.Data == null || response.Data == null)
            return;
        userRank = userResponse.Data.rank;
        globalPlayerClone.UpdatePlayerData(userResponse.Data);
        globalPlayerClone.SetRankerCount(response.Data.Length);
        SetInfo(response.Data);
    }
    
    private async UniTask<ApiResponse<RankerData[]>> LoadDataFromServer()
    {
        var response = await RankerDataDAC.GetRankerData();
        return response;
    }

    private async UniTask<ApiResponse<UserRankData>> LoadUserDataFromServer()
    {
        var response = await RankerDataDAC.GetUserRank();
        return response;
    }

    private void SetInfo(RankerData[] arr)
    {
        int rank = 1;
        for (int i = 0; i < arr.Length; i++)
        {
            var data = arr[i];
            rankingGlobalUiItems[i].SetInfo(rank, String.IsNullOrEmpty(data.name) ? "알수없음": data.name, "식당이름",
                data.rankPoint, data.uuid);
            rankingGlobalUiItems[i].gameObject.SetActive(true);
            if (i + 1 < arr.Length && arr[i].rankPoint == arr[i + 1].rankPoint)
            {
                continue;   
            }
            rank++;
        }
    }
    
    private void ShowLoadingPopup()
    {
        var alert = ServiceLocator.Instance.GetGlobalService<AlertPopup>();
        alert?.PopUp("랭킹 로딩중..", "우리 가게는 몇등일까?", SpumCharacter.HireEmployee, false);
    }

    // 팝업 닫기
    private void CloseLoadingPopup()
    {
        var alert = ServiceLocator.Instance.GetGlobalService<AlertPopup>();
        alert?.ClosePopup();
    }

    private void OnDisable()
    {
        globalPlayerClone.OnUnActive();
    }
    
    private void Update()
    {
        if (userRank > 50 || rankingGlobalUiItems.Count < userRank)
        {
            return;
        }
        bool isOverLap = rankingGlobalUiItems[userRank - 1].GetComponent<RectTransform>().CheckOverlap(canvas);
        if (isOverLap)
        {
            globalPlayerClone.OnUnActive();
        }
        else
        {
            globalPlayerClone.OnActive();
        }
    }
}