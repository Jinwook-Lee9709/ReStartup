using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScrollRectController : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<ScrollRect>().enabled = ServiceLocator.Instance.GetSceneService<GameManager>().tutorialManager == null;
    }
    public void OnTutorialStart()
    {
        GetComponent<ScrollRect>().enabled = false;
    }
    public void OnTutorialEnd()
    {
        GetComponent<ScrollRect>().enabled = true;
    }
}
