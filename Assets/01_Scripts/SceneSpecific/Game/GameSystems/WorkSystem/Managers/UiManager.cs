using UnityEngine;
using static UnityEditor.Progress;

public class UiManager : MonoBehaviour
{
    public GameObject uiHUD;
    public GameObject uiApps;
    public GameObject uiReview;
    public GameObject uiUpgrade;
    public GameObject uiEmployeeHp;
    public GameObject uiInterior;
    public GameObject uiPromotion;
    public GameObject uiQuest;
    public IngameGoodsUi inGameUi;

    public void Start()
    {
        EmployeeReewalAll();
    }
    public void OnClickButtonSetAppsUi()
    {
        var uiSetChack = uiHUD.gameObject.GetComponent<HeadsUpDisplayUi>().includedUiSet;
        if (uiSetChack)
        {
            uiHUD.SetActive(false);
            uiUpgrade.SetActive(false);
            uiApps.SetActive(true);
        }
    }

    public void OnCilckButtonOepnUiReview()
    {
        uiUpgrade.SetActive(false);
        uiPromotion.SetActive(false);
        uiReview.SetActive(true);
    }

    public void OnClickButtonOepnUiUpgrade()
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
    public void OnClickButtonOpenEmployeeHpUI()
    {
        uiEmployeeHp.SetActive(true);
    }
    public void OnClickButtonExitEmployeeHpUI()
    {
        uiEmployeeHp.SetActive(false);
    }
    public void EmployeeHpUIItemSet(EmployeeFSM employee)
    {
        uiEmployeeHp.GetComponent<EmployeeHpUi>().SetEmployeeUIItem(employee);
    }
    public void EmployeeHpSet(EmployeeFSM employee)
    {
        uiEmployeeHp.GetComponent<EmployeeHpUi>().EmployeeHpSet(employee);
    }
    public void EmployeeHpRenewal(EmployeeTableGetData employeeData)
    {
        uiEmployeeHp.GetComponent<EmployeeHpUi>().EmployeeHpRenewal(employeeData);
    }
    public void EmployeeReewalAll()
    {
        uiEmployeeHp.GetComponent<EmployeeHpUi>().EmployeeReewalAll();
    }
    public void OnClickButtonOpenInteriorUI()
    {
        uiInterior.SetActive(true);
    }
    public void OnClickButtonOpenPromotionUI()
    {
        uiPromotion.SetActive(true);
    }  
    public void OnClickButtonExitPromotionUI()
    {
        uiPromotion.SetActive(false);
    }
    public void OnClickButtonOpenQuestUI()
    {
        uiQuest.SetActive(true);
    }  
    public void OnClickButtonExitQuestUI()
    {
        uiQuest.SetActive(false);
    }
}