using Excellcube.EasyTutorial.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EatrandAlarmPopup : PopUp
{
    [SerializeField] private TextMeshProUGUI alarmText;
    [SerializeField] private TextMeshProUGUI alarmTitleText;

    public void Init(string titleText,string text)
    {
        popupUi.GetComponent<Button>().onClick.AddListener(OnCancle);
        popupUi.GetComponent<Button>().onClick.AddListener(()=>TutorialEvent.Instance.Broadcast(Strings.tutorialCompeleteKey));
        backGround.onClick.AddListener(() => TutorialEvent.Instance.Broadcast(Strings.tutorialCompeleteKey));
        alarmText.text = text;
        alarmTitleText.text = titleText;
    }
}
