using UnityEngine;
public class UiManager : MonoBehaviour
{
    public GameObject canvas;
    public GameObject uiHUD;
    public GameObject uiApps;
    public GameObject uiReview;
    public GameObject uiUpgrade;
    public GameObject uiEmployeeHp;
    public GameObject uiInterior;
    public GameObject uiPromotion;
    public GameObject uiQuest;
    public GameObject uiPreferences;
    public IngameGoodsUi inGameUi;

    public void Start()
    {
        EmployeeReewalAll();
    }

    private void Update()
    {
        if(Application.platform == RuntimePlatform.Android)
        {
            if(Input.GetKey(KeyCode.Escape))
            {
                OnClickButtonExitUiApps();
            }
        }
    }
    
    public void OnMissionClear()
    {
        uiApps.GetComponent<AppsUi>().MissionNewImageON();
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

    public void OnCilckButtonOpenUiReview()
    {
        uiUpgrade.SetActive(false);
        uiPromotion.SetActive(false);
        uiReview.SetActive(true);
    }

    public void OnClickButtonOpenUiUpgrade()
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
    public  void OnClickButtonExitInteriorUI()
    {
        uiInterior.SetActive(false);
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
        uiApps.GetComponent<AppsUi>().MissionNewImageOFF();
        uiQuest.SetActive(true);
    }  
    public void OnClickButtonExitQuestUI()
    {
        uiQuest.SetActive(false);
    }
    public void OnClickButtonOpenPreferencesUI()
    {
        uiPreferences.SetActive(true);
    }
    public void OnClickButtonExitPreferencesUI()
    {
        uiPreferences.SetActive(false);
        LocalSaveLoadManager.Save();
    }
}