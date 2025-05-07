using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeUi : MonoBehaviour
{
    public GameObject employeeScrollView;
    public GameObject foodScrollView;
    public GameObject RestaurantSupervisePanel;

    private void Update()
    {
        if(Application.platform == RuntimePlatform.Android)
        {
            if(Input.GetKey(KeyCode.Escape))
            {
                ServiceLocator.Instance.GetSceneService<GameManager>().uiManager.OnClickButtonExitUiUpgrade();
            }
        }
    }
    public void EmployeeScrollViewOpenButton()
    {
        employeeScrollView.SetActive(true);
        foodScrollView.SetActive(false);
        RestaurantSupervisePanel.SetActive(false);
    }
    public void FoodScrollViewOpenButton()
    {
        employeeScrollView.SetActive(false);
        foodScrollView.SetActive(true);
        RestaurantSupervisePanel.SetActive(false);
    }

    public void RestaurantSupervisePanelOpenButton()
    {
        employeeScrollView.SetActive(false);
        foodScrollView.SetActive(false);
        RestaurantSupervisePanel.SetActive(true);
    }

}
