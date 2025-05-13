using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BusinessModelUIPackageCard : MonoBehaviour
{
    private readonly string numberOfTimes = "WeeklyNumberOfTimes {0}/3";
    private int times = 3;
    public int cost;
    public int adTicketValue, moneyValue, goldValue;
    [SerializeField] private Button mainButton;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI numberOfTimesText;
    private BusinessModelUIPackagePopup popup;
    void Start()
    {
        popup = ServiceLocator.Instance.GetSceneService<GameManager>().uiManager.uiBusinessModel.GetComponent<BusinessModelUI>().busunessModelUIPackagePopup.GetComponent<BusinessModelUIPackagePopup>();
        costText.text = cost.ToString("N0");
        SetNumberOfTimesText();
        mainButton.onClick.RemoveAllListeners();
        mainButton.onClick.AddListener(OnPopup);
    }
    private void OnPopup()
    {
        popup.gameObject.SetActive(true);
        popup.SetInfo(this);
    }
    public void Buy()
    {
        SetNumberOfTimesText();
    }
    private void SetNumberOfTimesText()
    {
        numberOfTimesText.text = LZString.GetUIString(String.Format(numberOfTimes, times));
    }
}
