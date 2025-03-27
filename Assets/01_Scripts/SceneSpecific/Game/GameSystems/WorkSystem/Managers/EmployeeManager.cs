using UnityEngine;

public class EmployeeManager : MonoBehaviour
{
    public EmployeeListUi employeeListUi;
    [SerializeField] private GameManager gameManager;
    
    public void Awake()
    {
    }

    public void Start()
    {
        var data = DataTableManager.Get<EmployeeDataTable>("Employee").Data;
        foreach (var item in data.Values)
            if (item.Theme == (int)gameManager.CurrentTheme)
                employeeListUi.AddUpgrade(item);
        foreach (var item in data.Values)
            if (item.upgradeCount > 0)
            {
            }
    }
}