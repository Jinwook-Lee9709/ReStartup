using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InteriorCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private GameObject costPanel;
    [SerializeField] private Image icon;
    [SerializeField] private Button button;
    
    private InteriorData data;
    public InteriorData Data => data;
    public bool IsInteractable => button.interactable;

    public void Init(InteriorData data, Action<InteriorData> onBuy)
    {
        this.data = data;
        nameText.text = data.Name;
        costText.text = data.SellingCost.ToString();
        
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onBuy(data));
    }

    public void UpdateInteractable()
    {
        bool isAffordable = UserDataManager.Instance.CurrentUserData.Gold >= data.SellingCost;
        if (isAffordable)
        {
            button.interactable = true;
            costText.color = Color.white;
        }
        else
        {
            button.interactable = false;
            costText.color = Color.red;
        }
    }
}
