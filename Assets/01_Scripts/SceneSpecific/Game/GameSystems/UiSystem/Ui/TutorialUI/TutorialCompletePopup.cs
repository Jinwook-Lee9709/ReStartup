using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialCompletePopup : PopUp
{
    [SerializeField] private Button acceptButton;

    public void Init()
    {
        acceptButton.onClick.AddListener(OnCancle);

    }

}
