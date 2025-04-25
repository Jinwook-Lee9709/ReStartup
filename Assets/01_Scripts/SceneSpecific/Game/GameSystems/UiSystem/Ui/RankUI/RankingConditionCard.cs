using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class RankingConditionCard : MonoBehaviour
{
    public RankConditionData rankConditionData;

    private Button button;
    private Slider slider;
    public GameObject lockImage;
    public TextMeshProUGUI prevConditionText;
    public TextMeshProUGUI conditionText;
    public TextMeshProUGUI currentRankPointText;
    public TextMeshProUGUI explanationText;
    public RectTransform sliderGauge;
    private RankingConditionListUI rankConditionListUI;


    public int index;

    private void Start()
    {
        rankConditionListUI = gameObject.GetComponentInParent<RankingConditionListUI>();
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
            conditionText.text = $"{rankConditionData.GoalRanking}/{rankConditionData.GoalRanking}";
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
        }
        else
        {
            if (rankConditionData.Rank != 1)
            {
                var prevRankData = table.Data.First(x => x.Value.Rank == rankConditionData.Rank - 1);
                if (UserDataManager.Instance.CurrentUserData.CurrentRank == rankConditionData.Rank)
                {
                    currentRankPointText.gameObject.SetActive(true);
                    currentRankPointText.text = currentRankPoint.ToString();
                    currentRankPointText.ForceMeshUpdate();
                    slider.value = (float)(currentRankPoint - prevRankData.Value.GoalRanking) /
                                   (rankConditionData.GoalRanking - prevRankData.Value.GoalRanking);
                    Canvas.ForceUpdateCanvases();
                    UpdateTextPosition().Forget();
                }
                prevConditionText.text = prevRankData.Value.GoalRanking.ToString();
                conditionText.text = rankConditionData.GoalRanking.ToString();
            }
            else
            {
                currentRankPointText.gameObject.SetActive(true);
                slider.value = (float)currentRankPoint / rankConditionData.GoalRanking;
                prevConditionText.text = "0";
            }
            
            
            button.interactable = currentRankPoint >= rankConditionData.GoalRanking;
        }

        if (UserDataManager.Instance.CurrentUserData.CurrentRank > rankConditionData.Rank)
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
        UserDataManager.Instance.SetRankWithSave(rankConditionData.Rank + 1).Forget();
        ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager
            .OnEventInvoked(MissionMainCategory.GainRanking, 1);
        CheckComplete((int)UserDataManager.Instance.CurrentUserData.CurrentRankPoint);
        rankConditionListUI.CheckUnlock(index);
    }

    public void Unlock()
    {
        lockImage.SetActive(false);
    }
}