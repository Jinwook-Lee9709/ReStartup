using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialSkipPopup : PopUp
{
    [SerializeField] private Button cancleButton, acceptButton;
    [SerializeField] private TextMeshProUGUI tutorialSkipText;

    public void Init(UnityAction acceptAction)
    {
        cancleButton.onClick.AddListener(() =>
        {
            cancleButton.onClick.RemoveAllListeners();
            acceptButton.onClick.RemoveAllListeners();
            OnCancle();
        });
        acceptButton.onClick.AddListener(() =>
        {
            acceptAction?.Invoke();
            OnCancle();
        });
    }
}
