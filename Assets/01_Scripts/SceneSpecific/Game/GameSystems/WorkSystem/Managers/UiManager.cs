using UnityEngine;

public class UiManager : MonoBehaviour
{
    public GameObject uiHUD;
    public GameObject uiApps;
    public GameObject uiReview;
    public GameObject uiUpgrade;

    public void OnClickButtonSetAppsUi()
    {
        var uiSetChack = uiHUD.gameObject.GetComponent<HeadsUpDisplayUi>().includedUiSet;
        Debug.Log(uiSetChack);
        if (uiSetChack)
        {
            uiHUD.SetActive(false);
            uiUpgrade.SetActive(false);
            uiApps.SetActive(true);
        }
    }

    public void OnCilckButtonSetUiReview()
    {
        uiUpgrade.SetActive(false);
        uiApps.SetActive(false);
        uiReview.SetActive(true);
    }

    public void OnClickButtonSetUiUpgrade()
    {
        uiUpgrade.SetActive(true);
    }

    public void OnClickButtonExitUiUpgrade()
    {
        uiUpgrade.SetActive(false);
    }

    public void OnClickButtonExitUiApps()
    {
        uiApps.SetActive(false);
        uiHUD.SetActive(true);
    }

    public void OnClickButtonExitReviewUi()
    {
        uiReview.SetActive(false);
        uiHUD.SetActive(true);
    }
}