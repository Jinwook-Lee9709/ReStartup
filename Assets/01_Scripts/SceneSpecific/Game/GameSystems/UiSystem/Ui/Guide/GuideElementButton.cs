using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GuideElementButton : MonoBehaviour
{
    private static readonly string titleStringFormat = "Entry{0}";
    
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private Button button;

    public void Init(GuideElementData data, UnityAction<int> onClick)
    {
        var titleStringKey = String.Format(titleStringFormat, data.EntryID);
        var titleString = LZString.GetUIString(titleStringKey);
        
        title.text = titleString;
        
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick(data.EntryID));
    }
}
