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
    private List<int> hatContisions = new();

    public List<RankingSystemUiItem> Items => items;

    private void Start()
    {
        AddPlayerPoints((int)UserDataManager.Instance.CurrentUserData.CurrentRankPoint);
    }

    public void AddRankingSystemItem(RankingData data, List<int> hatContisions)
    {
        this.hatContisions = hatContisions;
        var ui = Instantiate(upgradeItemObject, parent).GetComponent<RankingSystemUiItem>();
        ui.Init(data, hatContisions);
        items.Add(ui);
        RankUpdate();
    }
    public void SetPlayerRankUIItem(RankingData data)
    {
        foreach (var item in items)
        {
            if(item.rankingData == data)
            {
                item.PlayerUiSet();
            }
        }
    }

    public void RankUpdate()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].UpdateUI();
        }
        items = items.OrderByDescending(item => item.rankingData.rankingPoint).ToList();

        for (int i = 0; i < items.Count; i++)
        {
            items[i].rankingData.rank = i + 1;
            items[i].rankingText.text = items[i].rankingData.rank.ToString();
            items[i].transform.SetSiblingIndex(i);
            items[i].SetHat();
        }
    }
    public void AddPlayerPoints(int points)
    {
        var player = items.FirstOrDefault(i => i.rankingData.RestaurantName == UserDataManager.Instance.CurrentUserData.Name);
        if (player != null)
        {
            player.rankingData.rankingPoint += points;
            RankUpdate();
            int hat = 0;
            for (int i = 1; i <= hatContisions.Count; i++)
            {
                if (hatContisions[i - 1] < player.rankingData.rankingPoint)
                {
                    hat = i;
                }
            }
            playerClone.UpdatePlayerData(player.rankingData,hat);
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