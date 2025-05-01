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
        cancleButton.onClick.AddListener(OnCancle);
        acceptButton.onClick.AddListener(() =>
        {
            acceptAction?.Invoke();
            OnCancle();
        });

        tutorialSkipText.text = "튜토리얼 스킵 임시 텍스트";
    }
}
