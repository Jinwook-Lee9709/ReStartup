using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class RankingGlobalController : MonoBehaviour
{
    private static readonly string PrefabName = "RankingGlobalItem";
    private static readonly int MaxRanking = 50;
    private static readonly float LOAD_RANKING_DELAY = 3f;
    
    [SerializeField] private Transform contents;
    
    private List<RankingGlobalUiItem> rankingGlobalUiItems = new List<RankingGlobalUiItem>();
    
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
        LoadRanking().Forget();
    }

    private async UniTask LoadRanking()
    {
        float targetTime = Time.time + LOAD_RANKING_DELAY;
        var alert = ServiceLocator.Instance.GetGlobalService<AlertPopup>();
        alert.PopUp("랭킹 로딩중..", "우리 가게는 몇등일까?", SpumCharacter.HireEmployee, false);
        var response = await RankerDataDAC.GetRankerData();
        if (response.ResponseCode != ResponseType.Success)
            return;
        if (Time.time < targetTime)
        {
            UniTask.WaitForSeconds(targetTime - Time.time);
        }

        alert.ClosePopup();
        SetInfo(response.Data);
    }

    private void SetInfo(RankerData[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            var data = arr[i];
            rankingGlobalUiItems[i].SetInfo(i, String.IsNullOrEmpty(data.name) ? "알수없음": data.name, "식당이름",
                data.rankPoint, data.uuid);
            rankingGlobalUiItems[i].gameObject.SetActive(true);
        }
    }
}