using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmployeeUpgradeListUi : MonoBehaviour
{
    public GameObject upgradeItemObject;
    private EmployeeTableGetData employeeData;
    public List<Button> allBuyButton;
    public Transform contents;
    private WorkType workType;

    void Start()
    {

    }
    public void AddEmployeeUpgradeItem(EmployeeTableGetData data)
    {
       
        employeeData = data;
        var ui = Instantiate(upgradeItemObject, contents).GetComponent<EmployeeUIItem>();
        ui.Init(data);
    }
    public void SetWorkType(WorkType worktype)
    {
        switch (worktype)
        {
            case WorkType.All:
                break;
            case WorkType.Payment:
                GetComponentInChildren<TextMeshProUGUI>().text = "계산원";
                break;
            case WorkType.Hall:
                GetComponentInChildren<TextMeshProUGUI>().text = "홀직원";
                break;
            case WorkType.Kitchen:
                GetComponentInChildren<TextMeshProUGUI>().text = "주방직원";
                break;
        }
    }
    public void AddButtonList(Button button)
    {
        allBuyButton.Add(button);
    }
    public void EmployeeAllBuy()
    {
        foreach (var item in allBuyButton)
        {
            item.onClick.Invoke();
        }
    }
}