using System.Collections;
using System.Collections.Generic;
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
    public TextMeshProUGUI conditionText;
    public TextMeshProUGUI explanationText;
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
        CheakComplete((int)currentUserRankPoint);
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
        CheakComplete((int)currentUserRankPoint);
    }
    public void CheakComplete(int currentRankPoint)
    {
        if (currentRankPoint >= rankConditionData.GoalRanking)
        {
            conditionText.text = $"{rankConditionData.GoalRanking}/{rankConditionData.GoalRanking}";
            slider.value = 1f;
            button.interactable = true;
        }
        else
        {
            slider.value = rankConditionData.GoalRanking / rankConditionData.GoalRanking;
            conditionText.text = $"{currentRankPoint}/{rankConditionData.GoalRanking}";
            button.interactable = false;
        }

        if (UserDataManager.Instance.CurrentUserData.CurrentRank > rankConditionData.Rank)
        {
            button.interactable = false;
        }
    }
    public void OnButtonClick()
    {
        UserDataManager.Instance.SetRankWithSave(rankConditionData.Rank + 1).Forget();
        ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager.OnEventInvoked(MissionMainCategory.HireStaff, 1, (int)rankConditionData.RangkingID);
        CheakComplete((int)UserDataManager.Instance.CurrentUserData.CurrentRankPoint);
        rankConditionListUI.CheakUnlock(index);
    }
    public void Unlock()
    {
        lockImage.SetActive(false);
    }
}
