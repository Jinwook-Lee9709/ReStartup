using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BusinessModelUIPackageCard : MonoBehaviour
{
    public int cost;
    public int adTicketValue, moneyValue, goldValue;
    [SerializeField] private Button mainButton;
    [SerializeField] private TextMeshProUGUI costText;
    public TextMeshProUGUI nameText;
    private BusinessModelUIPackagePopup popup;
    private BusinessModelUI businessModelUI;
    void Start()
    {
        businessModelUI = ServiceLocator.Instance.GetSceneService<GameManager>().uiManager.uiBusinessModel.GetComponent<BusinessModelUI>();
        popup = businessModelUI.busunessModelUIPackagePopup.GetComponent<BusinessModelUIPackagePopup>();
        costText.text = cost.ToString("N0");
        mainButton.onClick.RemoveAllListeners();
        mainButton.onClick.AddListener(OnPopup);
    }
    private void OnPopup()
    {
        //businessModelUI.OnLimitationPackagePopup();
        //return;

        popup.gameObject.SetActive(true);
        popup.SetInfo(this);
    }
}
