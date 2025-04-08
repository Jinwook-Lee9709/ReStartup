using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class InteriorCardGroup : MonoBehaviour
{
    [SerializeField] private Transform cardParent;
    [SerializeField] private AssetReference cardPrefab;

    private List<InteriorCard> cards = new();

    public void InitializeGroup(List<InteriorData> interiorDataList, Action<InteriorData> onBuy)
    {
        ClearGroup();
        foreach (var data in interiorDataList)
        {
            GameObject cardObject = Addressables.InstantiateAsync(cardPrefab, cardParent).WaitForCompletion();
            var card = cardObject.GetComponent<InteriorCard>();
            card.Init(data, onBuy);
            cards.Add(card);
        }
    }
    
    
    
    
    public void ClearGroup()
    {
        foreach (Transform child in cardParent)
        {
            Destroy(child.gameObject);
        }
        cards.Clear();
    }

}
