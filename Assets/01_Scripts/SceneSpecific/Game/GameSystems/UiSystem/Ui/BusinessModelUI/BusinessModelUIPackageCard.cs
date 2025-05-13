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
    [SerializeField] private Button mainButton;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI numberOfTimesText;
    void Start()
    {
        costText.text = cost.ToString("N0");
        SetNumberOfTimesText();
        mainButton.onClick.RemoveAllListeners();
        mainButton.onClick.AddListener(Buy);
    }
    private void Buy()
    {
        if (times <= 0)
        {
            return;
        }
        --times;
        //팝업띄우기
        SetNumberOfTimesText();
    }
    private void SetNumberOfTimesText()
    {
        numberOfTimesText.text = LZString.GetUIString(String.Format(numberOfTimes, times));
    }
}
