using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmployeeManager : MonoBehaviour
{
    public EmployeeUpgradeListUi upgradeListUi;
    public void Awake()
    {

    }
    public void Start()
    {
        var data = DataTableManager.Get<EmployeeDataTable>("Employee").Data;
        foreach (var item in data.Values)
        {
            if(item.Theme == (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme)
                upgradeListUi.AddEmployeeUpgradeItem(item);
        }
    }
}
