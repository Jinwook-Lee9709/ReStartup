using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class RankingSystemUiItem : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI rankingText;
    public TextMeshProUGUI rankingPointText;
    public RankingData rankingData;

    public void Init(RankingData data)
    {
        rankingData = data;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (rankingData.RestaurantName == "Player")
        {
            rankingData.rankingPoint = (int)UserDataManager.Instance.CurrentUserData.CurrentRankPoint;
            nameText.text = LZString.GetUIString(Strings.PlayerTag);
        }
        else
        {
            rankingData.rankingPoint = Random.Range(rankingData.RankingPointminimum, rankingData.RankingPointmaximum);
            nameText.text = LZString.GetUIString(rankingData.RestaurantName);
        }
        rankingPointText.text = rankingData.rankingPoint.ToString();
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
