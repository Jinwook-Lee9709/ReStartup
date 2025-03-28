using UnityEngine;

public class EmployeeListUi : MonoBehaviour
{
    public EmployeeManager employeeManager;
    public GameObject upgradeObject;

    private void Start()
    {
    }

    public void AddUpgrade(EmployeeTableGetData data)
    {
        var ui = Instantiate(upgradeObject, transform).GetComponent<UiItem>();
        ui.Init(data);
    }
}