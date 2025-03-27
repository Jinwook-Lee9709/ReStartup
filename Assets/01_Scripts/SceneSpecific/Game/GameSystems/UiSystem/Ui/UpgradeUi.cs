using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeUi : MonoBehaviour
{
    public GameObject employeeScrollView;
    public GameObject foodScrollView;

    public void EmployeeScrollViewOpenButton()
    {
        employeeScrollView.SetActive(true);
        foodScrollView.SetActive(false);
    }
    public void FoodScrollViewOpenButton()
    {
        employeeScrollView.SetActive(false);
        foodScrollView.SetActive(true);
    }

}
