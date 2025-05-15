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
    [SerializeField] private HatListController hats;
    [SerializeField] private RankingSystemListUi listManager;
    [SerializeField] private RectTransform contentRect;
    
    private bool isActive = true;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClickClone);
    }

    public void UpdatePlayerData(RankingData data)
    {
        playerName.text = UserDataManager.Instance.CurrentUserData.Name;
        playerRank.text = data.rank.ToString();
        playerRankPoint.text = data.rankingPoint.ToString();
    }
    public void UpdatePlayerData(RankingData data, int hatCount)
    {
        hats.SetHat(hatCount);
        playerName.text = UserDataManager.Instance.CurrentUserData.Name;
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
        MovePanelToCenter(playerData.rank);
    }
    
    
    [VInspector.Button]
    private void MovePanelToCenter(int index)
    {
        Vector3 contentPosition = contentRect.anchoredPosition;
        Vector3 itemPosition = listManager.Items[index].transform.localPosition;
        float contentHeight = contentRect.rect.size.y;
        float viewportHeight = rect.viewport.rect.size.y;
        float targetY = itemPosition.y + (viewportHeight / 2);
        float maxY = 0;
        float minY = -(contentHeight - viewportHeight);
        targetY = Mathf.Clamp(targetY, minY, maxY);

        Vector2 targetPosition = new Vector2(contentPosition.x, -targetY);
        rect.velocity = Vector2.zero;
        DOTween.To(() => contentRect.anchoredPosition, x => contentRect.anchoredPosition = x, targetPosition, 0.5f)
            .SetEase(Ease.OutCubic).onComplete += () => { rect.velocity = Vector2.zero; };
    }

}
