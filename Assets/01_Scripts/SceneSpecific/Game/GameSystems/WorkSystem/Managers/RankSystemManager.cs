using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankSystemManager : MonoBehaviour
{
    public RankingSystemListUi rankingListUi;
    public RankingConditionListUI rankingConditionListUi;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private RectTransform canvas;
    [SerializeField] private GameObject playerClone;
    [SerializeField] private GameObject localRankingPanel;
    private List<int> hatConditions = new();
    private RankingData playerData;
    private RankingSystemUiItem playerUiItem;
    private void Start()
    {
        rankingListUi.rankSystemManager = this;
        rankingListUi.playerClone = playerClone.GetComponent<PlayerClone>();

        var rankConditiondata = DataTableManager.Get<RankConditionDataTable>(DataTableIds.RankCondition.ToString()).Data;
        foreach (var item in rankConditiondata.Values)
        {
            if (item.Type == (int)gameManager.CurrentTheme)
            {
                rankingConditionListUi.RankConditionCardAdd(item);
                hatConditions.Add(item.GoalRanking);
            }
        }

        var data = DataTableManager.Get<RankingDataTable>("Ranking").Data;
        foreach (var item in data.Values)
        {
            if (item.Type == (int)gameManager.CurrentTheme)
                rankingListUi.AddRankingSystemItem(item, hatConditions);
        }
        var currentUserData = UserDataManager.Instance.CurrentUserData;
        // 플레이어 데이터 추가 (예제)
        playerData = new RankingData
        {
            RestaurantName = currentUserData.Name,
            rank = (int)currentUserData.CurrentRank,
            rankingPoint = (int)currentUserData.CurrentRankPoint,
            Type = (int)gameManager.CurrentTheme
        };
        rankingListUi.AddRankingSystemItem(playerData, hatConditions);
        UserDataManager.Instance.ChangeRankPointAction += rankingListUi.AddPlayerPoints;
        playerUiItem = rankingListUi.GetPlayerUiItem();
        //playerClone.GetComponent<PlayerClone>().OnActive(playerUiItem.rankingData);

        //RankCoinditionCardAdd

        playerClone.GetComponent<PlayerClone>().playerData = playerData;
    }

    private void OnDestroy()
    {
        if(UserDataManager.Instance != null)
            UserDataManager.Instance.ChangeRankPointAction -= rankingListUi.AddPlayerPoints;
    }
    private bool CheckOverlap(RectTransform rect)
    {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);

        foreach (var corner in corners)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, corner);
            if (RectTransformUtility.RectangleContainsScreenPoint(canvas, screenPoint, Camera.main))
            {
                return true;
            }
        }
        return false;
    }
    public void InitPlayerName()
    {
        rankingListUi.SetPlayerRankUIItem(playerData);
    }
    private void Update()
    {
        if (!localRankingPanel.activeSelf) return;
        bool isOverLap = CheckOverlap(playerUiItem.GetComponent<RectTransform>());
        if (isOverLap)
        {
            playerClone.GetComponent<PlayerClone>().OnUnActive();
        }
        else
        {
            playerClone.GetComponent<PlayerClone>().OnActive(playerUiItem.rankingData);
        }
    }
}