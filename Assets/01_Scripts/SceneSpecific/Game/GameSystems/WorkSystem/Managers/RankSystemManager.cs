using UnityEngine;
using UnityEngine.UI;

public class RankSystemManager : MonoBehaviour
{
    public RankingSystemListUi rankingListUi;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private RectTransform canvas;
    [SerializeField] private GameObject playerClone;
    private RankingSystemUiItem playerUiItem;
    private void Start()
    {
        rankingListUi.rankSystemManager = this;
        rankingListUi.playerClone = playerClone.GetComponent<PlayerClone>();
        var data = DataTableManager.Get<RankingDataTable>("Ranking").Data;
        foreach (var item in data.Values)
        {
            if (item.Type == (int)gameManager.CurrentTheme)
                rankingListUi.AddRankingSystemItem(item);
        }
        var currentUserData = UserDataManager.Instance.CurrentUserData;
        // 플레이어 데이터 추가 (예제)
        RankingData playerData = new RankingData
        {
            RestaurantName = currentUserData.Name,
            Ranking = 0,
            RankingPoint = 0,
            Type = (int)gameManager.CurrentTheme
        };
        rankingListUi.AddRankingSystemItem(playerData);
        UserDataManager.Instance.SetRankingPointAction += rankingListUi.AddPlayerPoints;
        playerUiItem = rankingListUi.GetPlayerUiItem();
        //playerClone.GetComponent<PlayerClone>().OnActive(playerUiItem.rankingData);
    }
    private bool CheckOverlap(RectTransform rect)
    {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);

        foreach (var corner in corners)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, corner);
            if(!RectTransformUtility.RectangleContainsScreenPoint(canvas, screenPoint,Camera.main))
            {
                return false;
            }
        }
        return true;
    }
    private void Update()
    {
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