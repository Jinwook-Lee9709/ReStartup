using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class RankingConditionListUI : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    public AssetReference rankingConditionCard;
    public Transform contents;
    private List<RankingConditionCard> cards = new();
    private int count = 0;

    private void Start()
    {
        UserDataManager.Instance.ChangeRankPointAction += RankPointCheck;
        UserDataManager.Instance.OnRankChangedEvent += RankCheck;
    }

    private void OnDestroy()
    {
        if (UserDataManager.Instance != null)
        {
            UserDataManager.Instance.ChangeRankPointAction -= RankPointCheck;
            UserDataManager.Instance.OnRankChangedEvent -= RankCheck;
        }
    }

    private void OnEnable()
    {
        MovePanelToCenterTask(UserDataManager.Instance.CurrentUserData.CurrentRank - 1).Forget();
    }

    private async UniTask MovePanelToCenterTask(int index)
    {
        await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);
        MovePanelToCenter(index);
    }

    [VInspector.Button]
    private void MovePanelToCenter(int index)
    {
        var contentRect = contents.GetComponent<RectTransform>();
        Vector3 contentPosition = contentRect.anchoredPosition;
        Vector3 itemPosition = cards[index].transform.localPosition;
        float contentWidth = contentRect.rect.size.x;
        float viewportWidth = scrollRect.viewport.rect.size.x;
        float targetX = itemPosition.x - (viewportWidth / 2);
        float minX = 0;
        float maxX = contentWidth - viewportWidth / 2;
        targetX = Mathf.Clamp(targetX, minX, maxX);
        Vector2 targetPosition = new Vector2(-targetX, contentPosition.y);
        scrollRect.velocity = Vector2.zero;
        DOTween.To(() => contentRect.anchoredPosition, x => contentRect.anchoredPosition = x, targetPosition, 0.5f)
            .SetEase(Ease.OutCubic).onComplete += () => { scrollRect.velocity = Vector2.zero; };
    }

    public void RankConditionCardAdd(RankConditionData data)
    {
        var newCard = Addressables.InstantiateAsync(rankingConditionCard, contents).WaitForCompletion();
        var newrankingConditionCard = newCard.GetComponent<RankingConditionCard>();
        newrankingConditionCard.index = count++;
        newrankingConditionCard.Init(data);
        cards.Add(newrankingConditionCard);
    }

    public void RankPointCheck(int val)
    {
        foreach (var card in cards)
        {
            card.CheckComplete((int)val);
        }
    }

    public void RankCheck(int Rank)
    {
        var point = (int)UserDataManager.Instance.CurrentUserData.CurrentRankPoint;
        foreach (var card in cards)
        {
            card.CheckComplete(point);
        }
    }

    public void CheckUnlock(int index)
    {
        int nextindex = index + 1;
        if (nextindex > cards.Count)
            return;
        var table = DataTableManager.Get<RankConditionDataTable>(DataTableIds.RankCondition.ToString());
        var count = table.Count(x =>
            x.Type == (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme);
        if (index != count)
            cards[index + 1].Unlock();
    }
}