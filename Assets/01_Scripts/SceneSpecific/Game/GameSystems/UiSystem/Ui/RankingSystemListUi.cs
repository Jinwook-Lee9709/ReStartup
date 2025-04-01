using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RankingSystemListUi : MonoBehaviour
{
    public GameObject upgradeItemObject;
    public GameManager gameManager;
    public Transform parent;
    public List<RankingSystemUiItem> items = new();
    private void Awake()
    {
    }

    private void Start()
    { 
    }

    public void AddRankingSystemItem(RankingData data)
    {
        var ui = Instantiate(upgradeItemObject, parent).GetComponent<RankingSystemUiItem>();
        ui.Init(data);
        items.Add(ui);
        RankUpdate();
    }

    public void RankUpdate()
    {
        items = items.OrderByDescending(item => item.rankingData.RankingPoint).ToList();

        for (int i = 0; i < items.Count; i++)
        {
            items[i].rankingData.Ranking = i + 1;
            items[i].UpdateUI();
            items[i].transform.SetSiblingIndex(i);
        }
    }
    public void AddPlayerPoints(int points)
    {
        var player = items.FirstOrDefault(i => i.rankingData.RestaurantName == "Player");
        if (player != null)
        {
            player.rankingData.RankingPoint += points;
            RankUpdate();
        }
    }
}