using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class EmployeeHpUIItem : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;
    public Slider hpbar;
    public EmployeeFSM employee;
    public EmployeeTableGetData employeeData;
    private void Start()
    {
        hpbar.interactable = false;
        if (employeeData != null)
        {
            StartCoroutine(LoadSpriteCoroutine(employeeData.Icon));
            var button = GetComponentInChildren<Button>();
            button.onClick.AddListener(() =>
            {
                employee.IncreaseHp(100);
                if (employeeData.currentHealth == employeeData.Health)
                {
                    //currentHp max
                    return;
                }
                if (employeeData.currentHealth > employeeData.Health)
                {
                    employeeData.currentHealth = employeeData.Health;
                }
                HpSet();
            });
            GameObject.FindWithTag("UIManager").GetComponent<UiManager>().uiEmployeeHp.GetComponent<EmployeeHpUi>().buttons.Add(button);
        }
    }
    public void SetEmployeeHpUiItem(EmployeeFSM employee)
    {
        this.employee = employee;
        employeeData = employee.EmployeeData;
        nameText.text = employeeData.StaffID.ToString();
        HpSet();
    }
    public void EmployeeHpSet()
    {
        if (employeeData.currentHealth < 0)
        {
            hpText.text = $"0/{employeeData.Health}";
            return;
        }
        HpSet();
    }
    private IEnumerator LoadSpriteCoroutine(string iconAddress)
    {
        var handle = Addressables.LoadAssetAsync<Sprite>(iconAddress);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
            image.sprite = handle.Result;
        else
            Debug.LogError($"Failed to load sprite: {iconAddress}");
    }
    private void HpSet()
    {
        hpText.text = $"{employeeData.currentHealth.ToString()}/{employeeData.Health}";
        hpbar.value = Mathf.Clamp01((float)employeeData.currentHealth / employeeData.Health);
    }
}
