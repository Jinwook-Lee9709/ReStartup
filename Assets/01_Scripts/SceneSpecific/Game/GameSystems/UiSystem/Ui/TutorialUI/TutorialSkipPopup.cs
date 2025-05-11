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

        tutorialSkipText.text = "사장님 스킵하시면\r\n식당 운영에 대한\r\n설명을 못 듣습니다.\r\n정말 스킵하시겠어요?\r\n";
    }
}
