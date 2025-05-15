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
    static readonly string employeeName = "EmployeeName{0}";
    public Image image;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI buyCostText;
    public TextMeshProUGUI recoveryValueText;
    public Slider hpbar;
    public EmployeeFSM employee;
    public EmployeeTableGetData employeeData;
    public int buyCost = 2000;
    public int recoveryValue = 20;
    [SerializeField] private GameObject notEnoughCostPopUp;
    [SerializeField] private GameObject employeeHpFullPopUp;

    private void Start()
    {
        hpbar.interactable = false;
        HpSet();
        if (employeeData != null)
        {
            StartCoroutine(LoadSpriteCoroutine(employeeData.Icon));
            buyCostText.text = $"{buyCost}";
            var button = GetComponentInChildren<Button>();
            button.onClick.AddListener(() =>
            {
                if(!CheakRecoverHelth())
                {
                    return;
                }
                ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager.OnEventInvoked(MissionMainCategory.Recover, 1);
                employee.IncreaseHp(recoveryValue);
                UserDataManager.Instance.AdjustMoney(-buyCost);
                if (employeeData.currentHealth > employeeData.Health)
                {
                    employeeData.currentHealth = employeeData.Health;
                }
                HpSet();
            });
        }
    }
    public void EmployeeAllRecovery(int val, CostType type)
    {
        var userDataManager = UserDataManager.Instance;
        if(CostType.Free == type)
        {
            AdvertisementManager.Instance.ShowRewardedAd(async () =>
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
        }
        ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager.OnEventInvoked(MissionMainCategory.Recover, 1);
        employee.IncreaseHp(val);
        if (employeeData.currentHealth > employeeData.Health)
        {
            employeeData.currentHealth = employeeData.Health;
        }
        HpSet();
    }
    public bool CheakRecoverHelthHelth(int currentHelth, int maxHelth)
    {
        if (currentHelth >= maxHelth)
        {
            Debug.Log("체력꽉참");
            return false;
        }
        return true;
    }
    public bool CheakRecoverHelthCost(int cost, int haveGoods)
    {
        if (cost > haveGoods)
        {
            Instantiate(notEnoughCostPopUp, GameObject.FindWithTag("UIManager").GetComponentInChildren<Canvas>().transform);
            Debug.Log("돈모자람");
            return false;
        }
        return true;
    }
    public bool CheakRecoverHelth()
    {
        if (buyCost > (int)UserDataManager.Instance.CurrentUserData.Money)
        {
            OnNotEnoughCostPopUp();
            Debug.Log("돈모자람");
            return false;
        }
        if (employeeData.currentHealth >= employeeData.Health)
        {
            OnEmployeeHpFullPopUp();
            Debug.Log("체력꽉참");

            return false;
        }
        return true;
    }
    public void OnNotEnoughCostPopUp()
    {
        Instantiate(notEnoughCostPopUp, GameObject.FindWithTag("UIManager").GetComponentInChildren<Canvas>().transform);
    }
    public void OnEmployeeHpFullPopUp()
    {
        Instantiate(employeeHpFullPopUp, GameObject.FindWithTag("UIManager").GetComponentInChildren<Canvas>().transform);
    }
    public void SetEmployeeHpUiItem(EmployeeFSM employee)
    {
        this.employee = employee;
        employeeData = employee.EmployeeData;
        nameText.text = LZString.GetUIString(string.Format(employeeName, employeeData.StaffID));
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
