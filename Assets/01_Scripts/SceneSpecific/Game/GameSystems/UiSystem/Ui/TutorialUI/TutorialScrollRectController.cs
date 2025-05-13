using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScrollRectController : MonoBehaviour
{
    private void OnEnable()
    {
        ScrollRectLock();
    }
    public void ScrollRectLock()
    {
        GetComponent<ScrollRect>().enabled = ServiceLocator.Instance.GetSceneService<GameManager>().tutorialManager == null;
    }
}
