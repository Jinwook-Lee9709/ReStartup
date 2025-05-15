using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RankingConditionCard : MonoBehaviour
{
    private readonly string goldIconSprite = "Gold";
    private readonly string inflowrateIconSprite = "BuffIcon0";
    private readonly string moneyIconSprite = "Cash";
    private readonly string adTicketIconSprite = "AdBlockTicket";
    private readonly string rankSt = "RankSt{0}";
    public RankConditionData rankConditionData;

    private Button button;
    private Slider slider;
    public GameObject lockImage;
    public GameObject clearImage;
    public TextMeshProUGUI prevConditionText;
    public TextMeshProUGUI conditionText;
    public TextMeshProUGUI currentRankPointText;
    public TextMeshProUGUI explanationText;
    public TextMeshProUGUI reward1ValueText;
    public TextMeshProUGUI reward2ValueText;
    public Image reward1Image;
    public Image reward2Image;
    public RectTransform sliderGauge;
    public Image emblemImage;
    public HatListController hatController;
    private RankingConditionListUI rankConditionListUI;


    public int index;

    private void Start()
    {
        rankConditionListUI = gameObject.GetComponentInParent<RankingConditionListUI>();
        switch (rankConditionData.RewardType1)
        {
            case RankRewardType.None:
                reward1Image.gameObject.SetActive(false);
                reward1ValueText.gameObject.SetActive(false);
                break;
            case RankRewardType.Money:
                StartCoroutine(LoadSpriteCoroutine(moneyIconSprite, reward1Image));
                break;
            case RankRewardType.Gold:
                StartCoroutine(LoadSpriteCoroutine(goldIconSprite, reward1Image));
                break;
            case RankRewardType.InflowRate:
                StartCoroutine(LoadSpriteCoroutine(inflowrateIconSprite, reward1Image));
                reward1Image.color = Color.blue;
                break;
            case RankRewardType.AdDelete:
                StartCoroutine(LoadSpriteCoroutine(adTicketIconSprite, reward1Image));
                break;
        }
        reward1ValueText.text = rankConditionData.RewardAmount1.ToString();
        switch (rankConditionData.RewardType2)
        {
            case RankRewardType.None:
                reward2Image.gameObject.SetActive(false);
                reward2ValueText.gameObject.SetActive(false);
                break;
            case RankRewardType.Money:
                StartCoroutine(LoadSpriteCoroutine(moneyIconSprite, reward2Image));
                break;
            case RankRewardType.Gold:
                StartCoroutine(LoadSpriteCoroutine(goldIconSprite, reward2Image));
                break;
            case RankRewardType.InflowRate:
                StartCoroutine(LoadSpriteCoroutine(inflowrateIconSprite, reward2Image)); 
                reward2Image.color = Color.blue;
                break;
            case RankRewardType.AdDelete:
                StartCoroutine(LoadSpriteCoroutine(adTicketIconSprite, reward2Image));
                break;
        }
        reward2ValueText.text = rankConditionData.RewardAmount2.ToString();
        explanationText.text = LZString.GetUIString(string.Format(rankSt, rankConditionData.Rank));
    }

    private void OnEnable()
    {
        var currentUserRankPoint = UserDataManager.Instance.CurrentUserData.CurrentRankPoint;
        var currentRank = UserDataManager.Instance.CurrentUserData.CurrentRank;
        if (index < currentRank)
        {
            Unlock();
        }

        CheckComplete((int)currentUserRankPoint);
    }

    public void Init(RankConditionData data)
    {
        rankConditionData = data;
        button = gameObject.GetComponentInChildren<Button>();
        slider = gameObject.GetComponentInChildren<Slider>();
        button.onClick.AddListener(OnButtonClick);
        var sprite = Addressables.LoadAssetAsync<Sprite>(String.Format(Strings.EmblemIdFormat, data.Rank)).WaitForCompletion();
        emblemImage.sprite = sprite;
        hatController.SetHat(rankConditionData.Rank);
        var currentUserRankPoint = UserDataManager.Instance.CurrentUserData.CurrentRankPoint;
        var currentRank = UserDataManager.Instance.CurrentUserData.CurrentRank;
        if (index < currentRank)
        {
            Unlock();
        }
        CheckComplete((int)currentUserRankPoint);
        
    }

    public void CheckComplete(int currentRankPoint)
    {
        currentRankPointText.gameObject.SetActive(false);
        var table = DataTableManager.Get<RankConditionDataTable>(DataTableIds.RankCondition.ToString());
        if (rankConditionData.Rank < UserDataManager.Instance.CurrentUserData.CurrentRank)
        {
            slider.value = 1f;
            button.interactable = true;
            if (rankConditionData.Rank == 1)
            {
                prevConditionText.text = "0";
            }
            else
            {
                var prevRankData = table.Data.First(x => x.Value.Rank == rankConditionData.Rank - 1);
                prevConditionText.text = prevRankData.Value.GoalRanking.ToString();
            }
            conditionText.text = rankConditionData.GoalRanking.ToString();
            clearImage.SetActive(true);
        }
        else
        {
            if (rankConditionData.Rank != 1)
            {
                var prevRankData = table.Data.First(x => x.Value.Rank == rankConditionData.Rank - 1);
                if (UserDataManager.Instance.CurrentUserData.CurrentRank == rankConditionData.Rank)
                {
                    currentRankPointText.gameObject.SetActive(true);

                    currentRankPointText.ForceMeshUpdate();
                    slider.value = (float)(currentRankPoint - prevRankData.Value.GoalRanking) /
                                   (rankConditionData.GoalRanking - prevRankData.Value.GoalRanking);
                    Canvas.ForceUpdateCanvases();
                    UpdateTextPosition().Forget();
                }
                prevConditionText.text = prevRankData.Value.GoalRanking.ToString();
            }
            else
            {
                currentRankPointText.gameObject.SetActive(true);
                slider.value = (float)currentRankPoint / rankConditionData.GoalRanking;
                prevConditionText.text = "0";
                
            }
            currentRankPointText.text = currentRankPoint.ToString();
            conditionText.text = rankConditionData.GoalRanking.ToString();
            
            button.interactable = currentRankPoint >= rankConditionData.GoalRanking;
        }

        if (UserDataManager.Instance.CurrentUserData.CurrentRank > rankConditionData.Rank
            || UserDataManager.Instance.CurrentUserData.IsRankCompensationClaimed)
        {
            button.interactable = false;
        }
    }

    private async UniTask UpdateTextPosition()
    {
        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        var bounds = currentRankPointText.textBounds.size;
        var sliderGaugeWidth = sliderGauge.rect.width;
        if (bounds.x < sliderGaugeWidth)
        {
            currentRankPointText.rectTransform.anchoredPosition = new Vector2(sliderGaugeWidth - bounds.x,
                currentRankPointText.rectTransform.anchoredPosition.y);
        }
        else
        {
            currentRankPointText.rectTransform.anchoredPosition = new Vector2(0,
                currentRankPointText.rectTransform.anchoredPosition.y);
        }
    }

    public void OnButtonClick()
    {
        var table = DataTableManager.Get<RankConditionDataTable>(DataTableIds.RankCondition.ToString());
        var count = table.Count(x=>x.Type == (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme);
        if (count != rankConditionData.Rank)
        {
            UserDataManager.Instance.SetRankWithSave(rankConditionData.Rank + 1).Forget();
            
            ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager
                .OnEventInvoked(MissionMainCategory.GainRanking, 1);
            CheckComplete((int)UserDataManager.Instance.CurrentUserData.CurrentRankPoint);
            rankConditionListUI.CheckUnlock(index);
        }
        else
        {
            UserDataManager.Instance.SaveIsRankCompensationClaimed(true).Forget();
            button.interactable = false;
        }
        var data = table.Data.First(x => x.Value.Rank == rankConditionData.Rank);
        GetRankReward(data.Value.RewardType1, data.Value.RewardAmount1);
        GetRankReward(data.Value.RewardType2, data.Value.RewardAmount2);
    }

    public void GetRankReward(RankRewardType type, int amount)
    {
        switch (type)
        {
            case RankRewardType.None:
                return;
            case RankRewardType.Money:
                UserDataManager.Instance.AdjustMoneyWithSave(amount).Forget();
                break;
            case RankRewardType.Gold:
                UserDataManager.Instance.AdjustGoldWithSave(amount).Forget();
                break;
            case RankRewardType.InflowRate:
                UserDataManager.Instance.CurrentUserData.InflowRate += amount;
                break;
            case RankRewardType.AdDelete:
                UserDataManager.Instance.AdjustAdTicketWithSave(amount).Forget();
                break;
        }
        
    }
    private IEnumerator LoadSpriteCoroutine(string iconAddress, Image image)
    {
        var handle = Addressables.LoadAssetAsync<Sprite>(iconAddress);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
            image.sprite = handle.Result;
        else
            Debug.LogError($"Failed to load sprite: {iconAddress}");
    }

    public void Unlock()
    {
        lockImage.SetActive(false);
    }
}