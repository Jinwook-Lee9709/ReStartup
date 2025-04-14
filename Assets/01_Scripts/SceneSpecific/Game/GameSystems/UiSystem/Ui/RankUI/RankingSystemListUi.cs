using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RankingSystemListUi : MonoBehaviour
{
    public RankSystemManager rankSystemManager;
    public GameObject upgradeItemObject;
    public GameManager gameManager;
    public Transform parent;
    public PlayerClone playerClone;
    public List<RankingSystemUiItem> items = new();

    public void AddRankingSystemItem(RankingData data)
    {
        var ui = Instantiate(upgradeItemObject, parent).GetComponent<RankingSystemUiItem>();
        ui.Init(data);
        items.Add(ui);
        RankUpdate();
    }

    public void RankUpdate()
    {
        items = items.OrderByDescending(item => item.rankingData.rankingPoint).ToList();

        for (int i = 0; i < items.Count; i++)
        {
            items[i].rankingData.rank = i + 1;
            items[i].rankingText.text = items[i].rankingData.rank.ToString();
            items[i].UpdateUI();
            items[i].transform.SetSiblingIndex(i);
        }
    }
    public void AddPlayerPoints(int points)
    {
        var player = items.FirstOrDefault(i => i.rankingData.RestaurantName == "Player");
        if (player != null)
        {
            UserDataManager.Instance.CurrentUserData.CurrentRankPoint += points;
            player.rankingData.rankingPoint += points;
            RankUpdate();
            playerClone.UpdatePlayerData(player.rankingData);
        }
    }

    public RankingSystemUiItem GetPlayerUiItem()
    {
        var player = items.FirstOrDefault(i => i.rankingData.RestaurantName == UserDataManager.Instance.CurrentUserData.Name);
        if (player != null)
        {
            return player;
        }
        return null;
    }
}