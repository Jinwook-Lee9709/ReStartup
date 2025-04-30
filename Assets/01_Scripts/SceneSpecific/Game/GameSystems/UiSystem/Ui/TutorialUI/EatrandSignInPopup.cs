using Excellcube.EasyTutorial.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EatrandSignInPopup : PopUp
{
    public void Init()
    {
        popupUi.GetComponent<Button>().onClick.AddListener(OnCancle);
        popupUi.GetComponent<Button>().onClick.AddListener(()=>TutorialEvent.Instance.Broadcast(Strings.tutorialCompeleteKey));
        backGround.onClick.AddListener(() => TutorialEvent.Instance.Broadcast(Strings.tutorialCompeleteKey));
    }
}
