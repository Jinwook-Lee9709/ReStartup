using Cysharp.Threading.Tasks;
using Excellcube.EasyTutorial.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPhase1RewardPopup : PopUp
{
    [SerializeField] private int rewardMoney = 100000;
    public void Init()
    {
        popupUi.GetComponent<Button>().onClick.AddListener(OnCancle);
        UserDataManager.Instance.AdjustMoneyWithSave(rewardMoney).Forget();
        popupUi.GetComponent<Button>().onClick.AddListener(() => TutorialEvent.Instance.Broadcast(Strings.tutorialCompeleteKey));
        backGround.onClick.AddListener(() => TutorialEvent.Instance.Broadcast(Strings.tutorialCompeleteKey));
    }
}
