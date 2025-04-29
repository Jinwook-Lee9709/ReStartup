using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeUi : MonoBehaviour
{
    public GameObject employeeScrollView;
    public GameObject foodScrollView;
    public GameObject RestaurantSupervisePanel;

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
