using Excellcube.EasyTutorial.Utils;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    // 버튼을 눌렀을 경우 아래 메서드 실행.
    public void PressTutorialButton()
    {
        TutorialEvent.Instance.Broadcast("TEMP_KEY");
    }
}
