using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialGuidePopup : PopUp
{
    public async UniTask Init()
    {
        await UniTask.NextFrame();
        backGround.onClick.RemoveAllListeners();
        popupUi.GetComponent<Button>().onClick.AddListener(OnCancle);
    }
}
