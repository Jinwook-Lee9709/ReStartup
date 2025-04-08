using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InteriorCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private GameObject costPanel;
    [SerializeField] private Image icon;
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI buyButtonText;
    
    [SerializeField] private GameObject authorizationCheckPanel;
    [SerializeField] private Button authorizationCheckButton;

    private bool isGoldEnough;
    private bool isSatisfyRequirements;
    
    public bool IsStateChanged
    {
        get
        {
            var userData = UserDataManager.Instance.CurrentUserData;
            bool isGoldConditionChanged = isGoldEnough != (userData.Gold >= data.SellingCost);
            bool isRequirementsChanged = isSatisfyRequirements != 
                                         (userData.CurrentRankPoint >= data.Requirements1 && 
                                          (data.Requirements2 == 0 || userData.InteriorSaveData[data.Requirements2] != 0));
            return isGoldConditionChanged || isRequirementsChanged;
        }
    }
    
    
    private InteriorData data;
    public InteriorData Data => data;
    public bool IsInteractable => buyButton.interactable;

    public void Init(InteriorData data, Action<InteriorData> onBuy)
    {
        this.data = data;
        nameText.text = data.Name;
        
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => onBuy(data));
        buyButton.onClick.AddListener(UpdateInteractable);
        authorizationCheckButton.onClick.RemoveAllListeners();
        authorizationCheckButton.onClick.AddListener(OnAuthorizationCheckButtonTouched);
        UpdateInteractable();
    }

    public void UpdateInteractable()
    {
        var userData = UserDataManager.Instance.CurrentUserData;
        
        int upgradeLevel = userData.InteriorSaveData[data.InteriorID];
        int cost = data.GetSellingCost(upgradeLevel);
        
        costText.text = cost.ToString();
        isGoldEnough = userData.Gold >= cost;
        
        isSatisfyRequirements = CheckRequirements(userData);
        
        UpdatePanels();
        UpdateButtonAndText(upgradeLevel);
    }

    private bool CheckRequirements(UserData userData)
    {
        return userData.CurrentRankPoint >= data.Requirements1 &&
               (data.Requirements2 == 0 || userData.InteriorSaveData[data.Requirements2] != 0);
    }

    private void UpdatePanels()
    {
        authorizationCheckPanel.SetActive(!isSatisfyRequirements);
        costPanel.SetActive(isSatisfyRequirements);
    }

    private void UpdateButtonAndText(int upgradeLevel)
    {
        if (isSatisfyRequirements)
        {
            //FIXME:스트링테이블로 분리
            buyButtonText.text = upgradeLevel == 0 ? "구매" : "업그레이드";
            //
            
            buyButton.interactable = isGoldEnough;
            costText.color = isGoldEnough ? Color.white : Color.red;
        }
    }

    public void OnAuthorizationCheckButtonTouched()
    {
        
    }
}
