using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class RankingConditionListUI : MonoBehaviour
{
    public AssetReference rankingConditionCard;
    public Transform contents;
    private List<RankingConditionCard> cards = new();
    private int count = 0;

    private void Start()
    {
        UserDataManager.Instance.ChangeRankPointAction += RankPointCheak;
    }
    public void RankCoinditionCardAdd(RankConditionData data)
    {
        var newCard = Addressables.InstantiateAsync(rankingConditionCard, contents).WaitForCompletion();
        var newrankingConditionCard = newCard.GetComponent<RankingConditionCard>();
        newrankingConditionCard.index = count++;
        newrankingConditionCard.Init(data);
        cards.Add(newrankingConditionCard);
    }
    public void RankPointCheak(int val)
    {
        foreach (var card in cards)
        {
            card.CheakComplete((int)val);
        }
    }
    public void CheakUnlock(int index)
    {
        int nextindex = index + 1;
        if(nextindex > cards.Count)
            return;
        cards[index + 1].Unlock();
    }
}
