using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmployeeHpUi : MonoBehaviour
{
    [SerializeField] private Image panel;
    [SerializeField] private Button background;
    public GameObject employeeHpItem;
    public Transform employeeHpItemParent;
    private List<EmployeeHpUIItem> items = new();
    public List<Button> buttons = new();

    private void OnEnable()
    {
        background.interactable = false;
        if (background != null)
        {
            var backgroundImage = background.GetComponent<Image>();
            backgroundImage.FadeInAnimation();
        }

        if (panel != null)
        {
            panel.transform.PopupAnimation(onComplete: () => background.interactable = true);
        }
    }

    public void Start()
    {
        background.onClick.RemoveAllListeners();
        background.onClick.AddListener(OnClose);
    }

    public void SetEmployeeUIItem(EmployeeFSM employee)
    {
        var item = Instantiate(employeeHpItem, employeeHpItemParent).GetComponent<EmployeeHpUIItem>();
        items.Add(item);
        item.SetEmployeeHpUiItem(employee);
    }

    public void EmployeeHpSet(EmployeeFSM employee)
    {
        foreach (var item in items)
        {
            if (item.employeeData == employee.EmployeeData)
            {
                item.EmployeeHpSet();
            }
        }
    }

    public void EmployeeHpRenewal(EmployeeTableGetData employeeData)
    {
        foreach (var item in items)
        {
            if (item.employeeData == employeeData)
            {
                item.HpSet();
            }
        }
    }
    public void OnButtonClickEmployeeAll()
    {
        foreach (var item in buttons)
        {
            item.onClick.Invoke();
        }
    }

    public void EmployeeAllRecovery(int val, CostType type, int costVal)
    {
        foreach (var item in items)
        {
            item.EmployeeAllRecovery(val, type, costVal);
        }
    }

    public void EmployeeReewalAll()
    {
        foreach (var item in items)
        {
            item.HpSet();
        }
    }

    public void OnClose()
    {
        background.interactable = false;
        var backgroundImage = background.GetComponent<Image>();
        backgroundImage.FadeOutAnimation();
        panel.transform.PopdownAnimation(onComplete: () => { gameObject.SetActive(false); });
    }
}
