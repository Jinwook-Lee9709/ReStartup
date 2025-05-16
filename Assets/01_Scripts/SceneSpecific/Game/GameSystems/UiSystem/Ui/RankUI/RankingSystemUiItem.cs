using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class  RankingSystemUiItem : MonoBehaviour
{
    [SerializeField] Image frame;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI rankingText;
    public TextMeshProUGUI rankingPointText;
    public RankingData rankingData;
    private List<int> hatConditions = new List<int>();
    [SerializeField] HatListController hats;

    public void Init(RankingData data, List<int> hatConditions)
    {
        this.hatConditions = hatConditions;
        rankingData = data;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (rankingData.RestaurantName == UserDataManager.Instance.CurrentUserData.Name)
        {
            frame.color = Colors.rankPanelPlayerColor;
            rankingData.rankingPoint = (int)UserDataManager.Instance.CurrentUserData.CurrentRankPoint;
            nameText.text = UserDataManager.Instance.CurrentUserData.Name;
        }
        else
        {
            rankingData.rankingPoint = Random.Range(rankingData.RankingPointminimum, rankingData.RankingPointmaximum);
            nameText.text = LZString.GetUIString(rankingData.RestaurantName);
        }
        rankingPointText.text = rankingData.rankingPoint.ToString();
    }
    public void PlayerUiSet()
    {
        rankingData.RestaurantName = UserDataManager.Instance.CurrentUserData.Name;
        rankingData.rankingPoint = (int)UserDataManager.Instance.CurrentUserData.CurrentRankPoint;
        nameText.text = UserDataManager.Instance.CurrentUserData.Name;
    }
    public void SetHat()
    {
        int hat = 0;
        for (int i = 1; i <= hatConditions.Count; i++)
        {
            if (hatConditions[i - 1] < rankingData.rankingPoint)
            {
                hat = i;
            }
        }
        hats.SetHat(hat);
    }

    //private void Start()
    //{
    //    if (rankingData != null)
    //    {
    //        StartCoroutine(LoadSpriteCoroutine(rankingData.ImageFile));
    //    }
    //}

    //private IEnumerator LoadSpriteCoroutine(string iconAddress)
    //{
    //    var handle = Addressables.LoadAssetAsync<Sprite>(iconAddress);
    //    yield return handle;

    //    if (handle.Status == AsyncOperationStatus.Succeeded)
    //        image.sprite = handle.Result;
    //    else
    //        Debug.LogError($"Failed to load sprite: {iconAddress}");
    //}
}
