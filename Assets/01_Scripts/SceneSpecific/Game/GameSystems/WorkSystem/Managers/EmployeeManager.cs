using UnityEngine;

public class EmployeeManager : MonoBehaviour
{
    public EmployeeUpgradeListUi employeeListUi;
    [SerializeField] private GameManager gameManager;

    public void Start()
    {
        var data = DataTableManager.Get<EmployeeDataTable>("Employee").Data;
        foreach (var item in data.Values)
            if (item.Theme == (int)gameManager.CurrentTheme)
                employeeListUi.AddEmployeeUpgradeItem(item);
        foreach (var item in data.Values)
            if (item.upgradeCount > 0)
            {
            }
    }
}