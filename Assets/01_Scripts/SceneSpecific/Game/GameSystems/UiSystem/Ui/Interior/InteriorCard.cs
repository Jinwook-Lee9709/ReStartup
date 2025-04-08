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
    [SerializeField] private Button authorizationCheckButton;
    [SerializeField] private GameObject authorityCheckPanel;
    
    
    private bool isGoldEnough;
    private bool isSatisfyRequirements;
    private InteriorUpgradePopup upgradePopup;
    private InteriorUpgradeAuthorityNotifyPopup upgradeAuthorityNotifyPopup;

    
    public bool IsStateChanged
    {
        get
        {
            var userData = UserDataManager.Instance.CurrentUserData;
            bool isGoldConditionChanged = isGoldEnough != (userData.Gold >= data.GetSellingCost());
            bool isRequirementsChanged = isSatisfyRequirements != 
                                         (userData.CurrentRankPoint >= data.Requirements1 && 
                                          (data.Requirements2 == 0 || userData.InteriorSaveData[data.Requirements2] != 0));
            return isGoldConditionChanged || isRequirementsChanged;
        }
    }
    
    
    private InteriorData data;
    public InteriorData Data => data;
    public bool IsInteractable => buyButton.interactable;

    public void Init(InteriorData data,InteriorUpgradePopup popup, InteriorUpgradeAuthorityNotifyPopup authorityNotifyPopup)
    {
        this.data = data;
        this.upgradePopup = popup;
        this.upgradeAuthorityNotifyPopup = authorityNotifyPopup;
        nameText.text = data.Name;
        
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnButtonClick);
        authorizationCheckButton.onClick.RemoveAllListeners();
        authorizationCheckButton.onClick.AddListener(OnAuthorizationCheckButtonTouched);
        UpdateInteractable();
    }

    public void UpdateInteractable()
    {
        var userData = UserDataManager.Instance.CurrentUserData;
        
        int upgradeLevel = userData.InteriorSaveData[data.InteriorID];
        int cost = data.GetSellingCost();
        
        costText.text = cost.ToString();
        isGoldEnough = userData.Gold >= cost;
        
        isSatisfyRequirements = CheckRequirements(userData);
        
        UpdatePanels();
        UpdateButtonAndText(upgradeLevel);
    }

    private void OnButtonClick()
    {
        if (UserDataManager.Instance.CurrentUserData.InteriorSaveData[data.InteriorID] != 0)
        {
            upgradePopup.gameObject.SetActive(true);
            upgradePopup.SetInfo(this);
        }
        else
        {
            OnBuy();
        }
    }

    public void OnBuy()
    {
        UserDataManager.Instance.UpgradeInterior(data.InteriorID);
        UserDataManager.Instance.ModifyGold(-data.SellingCost);
        UpdateInteractable();
    }
    
    private bool CheckRequirements(UserData userData)
    {
        return userData.CurrentRankPoint >= data.Requirements1 &&
               (data.Requirements2 == 0 || userData.InteriorSaveData[data.Requirements2] != 0);
    }

    private void UpdatePanels()
    {
        authorityCheckPanel.SetActive(!isSatisfyRequirements);
        costPanel.SetActive(isSatisfyRequirements);
    }

    private void UpdateButtonAndText(int upgradeLevel)
    {
        if (isSatisfyRequirements)
        {
            //FIXME:스트링테이블로 분리
            buyButtonText.text = upgradeLevel == 0 ? "구매" : "신상품";
            //
            
            buyButton.interactable = isGoldEnough;
            costText.color = isGoldEnough ? Color.white : Color.red;
        }
    }

    public void OnAuthorizationCheckButtonTouched()
    {
        upgradeAuthorityNotifyPopup.SetRequirementText(data);
        upgradeAuthorityNotifyPopup.gameObject.SetActive(true);
    }
}
