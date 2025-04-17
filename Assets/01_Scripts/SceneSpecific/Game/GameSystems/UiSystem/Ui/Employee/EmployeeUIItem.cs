using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening.Core.Easing;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem.Composites;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class EmployeeUIItem : MonoBehaviour
{
    static readonly string employeeUpgradeFix = "EmployeeUpgradeComplete";
    static readonly string hallStaff = "HallStaff";
    static readonly string kitchenStaff = "KitchenStaff";
    static readonly string cashierStaff = "CashierStaff";
    static readonly string employment = "Employment";
    static readonly string education = "Education";

    [SerializeField] private Image image;

    [SerializeField] private TextMeshProUGUI uiNameText;
    [SerializeField] private TextMeshProUGUI uiUpgradeCostText;
    [SerializeField] private TextMeshProUGUI upgradeButtonText;

    [SerializeField] private TextMeshProUGUI workSpeedValue;
    [SerializeField] private TextMeshProUGUI moveSpeedValue;
    [SerializeField] private TextMeshProUGUI HealthValue;

    //Fortest
    public EmployeeTableGetData employeeData;

    private EmployeeUIManager employeeUIManager;
    private EmployeeManager employeeManager;

    private Button button;

    private readonly string employeePrefab = "Agent.prefab";

    private IngameGoodsUi ingameGoodsUi;
    private EmployeeUpgradePopup employeeUpgradePopup;

    private Dictionary<int, EmployeeSaveData> employeeSaveData;
    private int employeeId;

    private void Awake()
    {
        employeeSaveData = UserDataManager.Instance.CurrentUserData.EmployeeSaveData;
    }

    private void Start()
    {
        employeeManager = ServiceLocator.Instance.GetSceneService<GameManager>().EmployeeManager;
        ingameGoodsUi = GameObject.FindWithTag("UIManager").GetComponent<UiManager>().inGameUi;
        if (employeeData != null)
        {
            StartCoroutine(LoadSpriteCoroutine(employeeData.Icon));
            employeeUIManager = GetComponentInParent<EmployeeUIManager>();
            if (employeeUIManager == null)
            {
                Debug.LogError($"{gameObject.name}의 부모 중 employeeUIManagerr를 찾을 수 없습니다.");
                return;
            }
            employeeUIManager.EmployeeAllBuy += OnBuy;
        }

        UserDataManager.Instance.ChangeMoneyAction += OnConditionChanged;
    }

    public void Init(EmployeeTableGetData data, EmployeeUpgradePopup employeeUpgradePopup)
    {
        employeeData = data;
        employeeId = employeeData.StaffID;
        
        this.employeeUpgradePopup = employeeUpgradePopup;
        switch ((WorkType)employeeData.StaffType)
        {
            case WorkType.All:
                break;
            case WorkType.Payment:
                uiNameText.text = $"{LZString.GetUIString(hallStaff)} :\n{LZString.GetUIString(string.Format(Strings.employeeNameKeyFormat,employeeData.StaffID))}";
                break;
            case WorkType.Hall:
                uiNameText.text = $"{LZString.GetUIString(kitchenStaff)} :\n{LZString.GetUIString(string.Format(Strings.employeeNameKeyFormat, employeeData.StaffID))}";
                break;
            case WorkType.Kitchen:
                uiNameText.text = $"{LZString.GetUIString(cashierStaff)} :\n{LZString.GetUIString(string.Format(Strings.employeeNameKeyFormat, employeeData.StaffID))}";
                break;
        }
        button = GetComponentInChildren<Button>();
        workSpeedValue.text = employeeData.WorkSpeed.ToString();
        moveSpeedValue.text = employeeData.MoveSpeed.ToString();
        HealthValue.text = employeeData.Health.ToString();
        employeeData.Health = employeeData.Health + (10 * employeeSaveData[employeeId].level);

        SetButtonInteractable();
        SetInfoText();
        SetUpgradeButtonText(employeeSaveData[employeeId].level);
        button.onClick.AddListener(OnButtonClick);

    }

    private void OnConditionChanged(int? money)
    {
        SetButtonInteractable();
    }

    private void OnButtonClick()
    {
        if (employeeSaveData[employeeId].level != 0)
        {
            employeeUpgradePopup.gameObject.SetActive(true);
            employeeUpgradePopup.SetInfo(this, image.sprite);
        }
        else
        {
            OnBuy();
        }
    }
    public void OnBuy()
    {
        var userData = UserDataManager.Instance.CurrentUserData;
        if (employeeSaveData[employeeId].level >= 5 || userData.Money < employeeData.Cost)
        {
            return;
        }
        button.interactable = false;
        if (employeeSaveData[employeeId].level < 1 && userData.Money > employeeData.Cost)
        {
            ServiceLocator.Instance.GetSceneService<GameManager>().EmployeeManager.InstantiateAndRegisterWorker(employeeData);
            OnUpgradeEmployee();
        }
        else if (userData.Money > employeeData.Cost * employeeSaveData[employeeId].level)
        {
            OnUpgradeEmployee();
        }
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
    private void OnUpgradeEmployee()
    {
        HandleUpgradeEmployee().Forget();
    }

    private void SetInfoText()
    {
        int payLevel = UserDataManager.Instance.CurrentUserData.EmployeeSaveData[employeeData.StaffID].level + 1;
        if (payLevel > 5)
        {
            uiUpgradeCostText.text = LZString.GetUIString(employeeUpgradeFix);
            
        }
        else
        {
            uiUpgradeCostText.text = $"{employeeData.Cost * (employeeSaveData[employeeId].level + 1)}";

        }
        switch ((WorkType)employeeData.StaffType)
        {
            case WorkType.All:
                break;
            case WorkType.Payment:
                uiNameText.text = $"{LZString.GetUIString(hallStaff)}:\n{LZString.GetUIString(string.Format(Strings.employeeNameKeyFormat, employeeData.StaffID))}:{employeeSaveData[employeeId].level}";
                break;
            case WorkType.Hall:
                uiNameText.text = $"{LZString.GetUIString(kitchenStaff)}:\n{LZString.GetUIString(string.Format(Strings.employeeNameKeyFormat, employeeData.StaffID))}:{employeeSaveData[employeeId].level}";
                break;
            case WorkType.Kitchen:
                uiNameText.text = $"{LZString.GetUIString(cashierStaff)}:\n{LZString.GetUIString(string.Format(Strings.employeeNameKeyFormat, employeeData.StaffID))}:{employeeSaveData[employeeId].level}";
                break;
        }
    }

    private void SetUpgradeButtonText(int level)
    {
        switch (level)
        {
            case 0:
                upgradeButtonText.text = LZString.GetUIString(employment);
                break;
            case > 0 and < 5:
                upgradeButtonText.text = LZString.GetUIString(education);
                break;
            default:
                upgradeButtonText.text = LZString.GetUIString(Strings.complete);
                break;

        }
    }

    private async UniTask HandleUpgradeEmployee()
    {
        if (!IsPayable(employeeData))
            return;

        await UserDataManager.Instance.UpgradeEmployee(employeeId);
        employeeManager.UpgradeEmployee(employeeId);
        int cost = employeeData.Cost * employeeSaveData[employeeId].level;
        employeeData.Health = employeeData.Health + (10 * employeeSaveData[employeeId].level);
        await UserDataManager.Instance.AdjustMoneyWithSave(-cost);
        ingameGoodsUi.SetCostUi();
        SetInfoText();
        SetUpgradeButtonText(employeeSaveData[employeeId].level);
        SetButtonInteractable();
    }

    private void SetButtonInteractable()
    {
        button.interactable = IsPayable(employeeData);
    }

    public bool IsPayable(EmployeeTableGetData data)
    {
        int payLevel = UserDataManager.Instance.CurrentUserData.EmployeeSaveData[data.StaffID].level + 1;
        if (payLevel > 5)
        {
            uiUpgradeCostText.text = LZString.GetUIString(employeeUpgradeFix);
            return false;
        }
        return payLevel * data.Cost <= UserDataManager.Instance.CurrentUserData.Money;
    }
}