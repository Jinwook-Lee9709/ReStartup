using UnityEngine;

public class RankSystemManager : MonoBehaviour
{
    public RankingSystemListUi rankingListUi;
    [SerializeField] private GameManager gameManager;

    private void Start()
    {
        var data = DataTableManager.Get<RankingDataTable>("Ranking").Data;
        foreach (var item in data.Values)
        {
            if (item.Type == (int)gameManager.CurrentTheme)
                rankingListUi.AddRankingSystemItem(item);
        }

        // 플레이어 데이터 추가 (예제)
        RankingData playerData = new RankingData
        {
            RestaurantName = "Player",
            Ranking = 0,
            RankingPoint = 1000,
            Type = (int)gameManager.CurrentTheme
        };
        rankingListUi.AddRankingSystemItem(playerData);
        UserDataManager.Instance.setRankingPointAction += rankingListUi.AddPlayerPoints;
    }
}