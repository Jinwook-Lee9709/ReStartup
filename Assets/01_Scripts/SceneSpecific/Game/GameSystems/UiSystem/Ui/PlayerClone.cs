using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerClone : MonoBehaviour
{
    public RankingData playerData;
    [SerializeField] private Image playerImage;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI playerRank;
    [SerializeField] private TextMeshProUGUI playerRankPoint;

    private bool isActive = true;

    private void UpdatePlayerData(RankingData data)
    {
        //playerImage.
        playerName.text = data.RestaurantName;
        playerRank.text =data.Ranking.ToString();
        playerRankPoint.text = data.RankingPoint.ToString();
    }
    public void OnActive(RankingData playerData)
    {
        if (!gameObject.activeSelf)
        {
            isActive = true;
            UpdatePlayerData(playerData);
            gameObject.SetActive(true);
            transform.DOScale(1f, 0.5f).SetEase(Ease.InOutElastic);
        }
    }
    public void OnUnActive()
    {
        if (isActive)
        {
            isActive = false;
            transform.DOKill();
            transform.DOScale(0f, 0.5f).SetEase(Ease.InOutElastic).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
    }
}
