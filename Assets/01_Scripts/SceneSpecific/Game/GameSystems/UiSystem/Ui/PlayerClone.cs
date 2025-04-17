using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerClone : MonoBehaviour
{
    public ScrollRect rect;
    public RankingData playerData;
    [SerializeField] private Image playerImage;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI playerRank;
    [SerializeField] private TextMeshProUGUI playerRankPoint;

    private bool isActive = true;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClickClone);
    }

    public void UpdatePlayerData(RankingData data)
    {
        playerName.text = data.RestaurantName;
        playerRank.text = data.rank.ToString();
        playerRankPoint.text = data.rankingPoint.ToString();
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

    public void OnClickClone()
    {
        rect.DOVerticalNormalizedPos(Mathf.InverseLerp(50, 0, playerData.rank),1f);
    }
}
