using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
                if (employeeData.currentHealth == employeeData.Health)
                {
                    return;
                }
                ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager.OnEventInvoked(MissionMainCategory.Recover, 1);
                employee.IncreaseHp(100);
                if (employeeData.currentHealth > employeeData.Health)
                {
                    employeeData.currentHealth = employeeData.Health;
                }
                HpSet();
            });
            GameObject.FindWithTag("UIManager").GetComponent<UiManager>().uiEmployeeHp.GetComponent<EmployeeHpUi>().buttons.Add(button);
        }
    }
    public void EmployeeAllRecovery(int val, CostType type, int costVal)
    {
        var userDataManager = UserDataManager.Instance;
        switch (type)
        {
            case CostType.Free:
                AdvertisementManager.Instance.ShowRewardedAd(() =>
                {
                    if (employeeData.currentHealth == employeeData.Health)
                    {
                        return;
                    }
                    ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager.OnEventInvoked(MissionMainCategory.Recover, 1);
                    employee.IncreaseHp(val);
                    if (employeeData.currentHealth > employeeData.Health)
                    {
                        employeeData.currentHealth = employeeData.Health;
                    }
                    HpSet();
                });
                return;
            case CostType.Money:
                if (costVal > userDataManager.CurrentUserData.Money)
                {
                    return;
                }
                userDataManager.AdjustMoneyWithSave(costVal).Forget();
                break;
            case CostType.Gold:
                if (costVal > userDataManager.CurrentUserData.Gold)
                {
                    return;
                }
                userDataManager.AdjustGoldWithSave(costVal).Forget();
                break;
        }
        if (employeeData.currentHealth == employeeData.Health)
        {
            return;
        }
        ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager.OnEventInvoked(MissionMainCategory.Recover, 1);
        employee.IncreaseHp(val);
        if (employeeData.currentHealth > employeeData.Health)
        {
            employeeData.currentHealth = employeeData.Health;
        }
        HpSet();
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
    public void HpSet()
    {
        hpText.text = $"{employeeData.currentHealth.ToString()}/{employeeData.Health}";
        hpbar.value = Mathf.Clamp01((float)employeeData.currentHealth / employeeData.Health);
    }
}
