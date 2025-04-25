using System.Collections;
using System.Collections.Generic;
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

    private void OnEnable()
    {
        scrollRect.GetComponent<ScrollRect>().DOHorizontalNormalizedPos(Mathf.InverseLerp(0,cards.Count, UserDataManager.Instance.CurrentUserData.CurrentRank - 1),1f);
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
        if(nextindex > cards.Count)
            return;
        cards[index + 1].Unlock();
    }
}
