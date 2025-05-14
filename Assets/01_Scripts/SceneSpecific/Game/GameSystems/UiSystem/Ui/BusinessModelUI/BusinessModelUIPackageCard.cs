using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BusinessModelUIPackageCard : MonoBehaviour
{
    private readonly string numberOfTimes = "WeeklyLimitation{0}";
    public int times = 3;
    public int cost;
    public int adTicketValue, moneyValue, goldValue;
    [SerializeField] private Button mainButton;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI numberOfTimesText;
    private BusinessModelUIPackagePopup popup;
    private BusinessModelUI businessModelUI;
    void Start()
    {
        businessModelUI = ServiceLocator.Instance.GetSceneService<GameManager>().uiManager.uiBusinessModel.GetComponent<BusinessModelUI>();
        popup = businessModelUI.busunessModelUIPackagePopup.GetComponent<BusinessModelUIPackagePopup>();
        costText.text = cost.ToString("N0");
        SetNumberOfTimesText();
        mainButton.onClick.RemoveAllListeners();
        mainButton.onClick.AddListener(OnPopup);
    }
    private void OnPopup()
    {
        if (times <= 0)
        {
            businessModelUI.OnLimitationPackagePopup();
            return;
        }
        popup.gameObject.SetActive(true);
        popup.SetInfo(this);
    }
    public void Buy()
    {
        --times;
        SetNumberOfTimesText();
    }
    private void SetNumberOfTimesText()
    {
        numberOfTimesText.text = LZString.GetUIString(String.Format(numberOfTimes, times));
    }
}
