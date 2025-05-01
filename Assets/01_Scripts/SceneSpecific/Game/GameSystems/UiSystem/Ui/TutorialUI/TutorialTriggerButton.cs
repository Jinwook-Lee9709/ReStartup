using Excellcube.EasyTutorial.Utils;
using UnityEngine;

public class TutorialTriggerButton : MonoBehaviour
{

    public void OnTutorialTriggerButtonTouch()
    {
        TutorialEvent.Instance.Broadcast(Strings.tutorialCompeleteKey);
    }
}
