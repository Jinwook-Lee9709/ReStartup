using Excellcube.EasyTutorial.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class TutorialTriggerButton : MonoBehaviour
{

    public void OnTutorialTriggerButtonTouch()
    {
        TutorialEvent.Instance.Broadcast(Strings.tutorialCompeleteKey);
    }
}
