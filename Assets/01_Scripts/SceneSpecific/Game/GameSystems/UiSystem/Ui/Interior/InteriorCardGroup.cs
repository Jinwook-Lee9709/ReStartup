using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class InteriorCardGroup : MonoBehaviour
{
    [SerializeField] private Transform cardParent;
    [SerializeField] private AssetReference cardPrefab;
    [SerializeField] private TextMeshProUGUI categoryText;

    private List<InteriorCard> cards = new();
    private InteriorCategory category;

    public void InitializeGroup(List<InteriorData> interiorDataList, InteriorCategory category, Action<InteriorData> onBuy)
    {
        ClearGroup();
        this.category = category;
        categoryText.text = category.ToString();
        foreach (var data in interiorDataList)
        {
            GameObject cardObject = Addressables.InstantiateAsync(cardPrefab, cardParent).WaitForCompletion();
            var card = cardObject.GetComponent<InteriorCard>();
            card.Init(data, onBuy);
            cards.Add(card);
        }
    }

    public void UpdateCards()
    {
        foreach (var card in cards)
        {
            if (card.IsStateChanged)
            {
                card.UpdateInteractable();
            }
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
