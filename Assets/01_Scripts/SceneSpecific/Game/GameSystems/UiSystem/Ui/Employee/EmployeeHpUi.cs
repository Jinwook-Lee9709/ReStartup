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
    [SerializeField] private GameObject notEnoughCostPopUp;
    [SerializeField] private GameObject employeeHpFullPopUp;
    [SerializeField] private GameObject donHaveEmployeePopUp;

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

    public void EmployeeAllRecovery(int val, CostType type, int costVal)
    {
        if (items.Count == 0)
        {
            Instantiate(donHaveEmployeePopUp, GameObject.FindWithTag("UIManager").GetComponentInChildren<Canvas>().transform);
            return;
        }


        int hpFullCount = 0;
        switch (type)
        {
            case CostType.Free:
                break;
            case CostType.Money:
                foreach (var item in items)
                {
                    if (costVal > (int)UserDataManager.Instance.CurrentUserData.Money)
                    {
                        Instantiate(notEnoughCostPopUp, GameObject.FindWithTag("UIManager").GetComponentInChildren<Canvas>().transform);
                        return;
                    }
                    if (!item.CheakRecoverHelthHelth(item.employeeData.currentHealth, item.employeeData.Health))
                    {
                        hpFullCount++;
                    }
                }
                break;
            case CostType.Gold:
                foreach (var item in items)
                {
                    if (costVal > (int)UserDataManager.Instance.CurrentUserData.Gold)
                    {
                        Instantiate(notEnoughCostPopUp, GameObject.FindWithTag("UIManager").GetComponentInChildren<Canvas>().transform);
                        return;
                    }
                    if (!item.CheakRecoverHelthHelth(item.employeeData.currentHealth, item.employeeData.Health))
                    {
                        hpFullCount++;
                    }
                }
                break;
        }
        if (hpFullCount == items.Count)
        {
            Instantiate(employeeHpFullPopUp, GameObject.FindWithTag("UIManager").GetComponentInChildren<Canvas>().transform);
            return;
        }
        switch (type)
        {
            case CostType.Free:
                break;
            case CostType.Money:
                UserDataManager.Instance.AdjustMoney(-costVal);
                break;
            case CostType.Gold:
                UserDataManager.Instance.AdjustGold(-costVal);
                break;
        }
        foreach (var item in items)
        {
            item.EmployeeAllRecovery(val, type);
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
