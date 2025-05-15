using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalPlayerClone : MonoBehaviour
{
    public ScrollRect rect;
    public UserRankData playerData;
    [SerializeField] private Image playerImage;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI playerRank;
    [SerializeField] private TextMeshProUGUI playerRankPoint;
    private int rankerCount = 0;
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private RankingGlobalController listManager;
    private bool isActive = true;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClickClone);
    }

    public void SetRankerCount(int count)
    {
        rankerCount = count;
    }

    public void UpdatePlayerData(UserRankData data)
    {
        playerData = data;
        playerName.text = UserDataManager.Instance.CurrentUserData.Name;
        playerRank.text = data.rank.ToString();
        playerRankPoint.text = data.rankPoint.ToString();
    }
    public void OnActive()
    {
        if (!gameObject.activeSelf)
        {
            isActive = true;
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
        if (playerData.rank > 50)
            return;
        MovePanelToCenter(playerData.rank);
    }
    
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
