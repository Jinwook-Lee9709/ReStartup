using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public GameObject uiHUD;
    public GameObject uiApps;
    public GameObject uiReview;
    public void OnClickButtonSetAppsUi()
    {
        var uiSetChack = uiHUD.gameObject.GetComponent<HeadsUpDisplayUi>().includedUiSet;
        if (uiSetChack)
        {
            uiHUD.SetActive(false);
            uiApps.SetActive(true);
        }
    }
    public void UnCilckButtonSetUiReview()
    {
        uiApps.SetActive(false);
        uiReview.SetActive(true);
    }
}
