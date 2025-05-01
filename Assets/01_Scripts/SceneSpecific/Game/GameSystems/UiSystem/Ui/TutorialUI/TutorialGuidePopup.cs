using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialGuidePopup : PopUp
{
    public void Init()
    {
        popupUi.GetComponent<Button>().onClick.AddListener(OnCancle);
    }
}
