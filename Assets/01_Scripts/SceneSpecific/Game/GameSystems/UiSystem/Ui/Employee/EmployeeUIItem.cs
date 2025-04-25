using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening.Core.Easing;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem.Composites;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using static SoonsoonData;

public class EmployeeUIItem : MonoBehaviour
{
    static readonly string employeeUpgradeFix = "EmployeeUpgradeComplete";
    static readonly string hallStaff = "HallStaff";
    static readonly string kitchenStaff = "KitchenStaff";
    static readonly string cashierStaff = "CashierStaff";
    static readonly string employment = "Employment";
    static readonly string education = "Education";
    private static readonly float buyInterval = 3f;   

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
        GameObject.FindWithTag("UIManager").GetComponent<UiManager>().EmployeeHpRenewal(employeeData);
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
                uiNameText.text = $"{LZString.GetUIString(cashierStaff)} :\n{LZString.GetUIString(string.Format(Strings.employeeNameKeyFormat, employeeData.StaffID))}";
                break;
            case WorkType.Hall:
                uiNameText.text = $"{LZString.GetUIString(hallStaff)} :\n{LZString.GetUIString(string.Format(Strings.employeeNameKeyFormat, employeeData.StaffID))}";
                break;
            case WorkType.Kitchen:
                uiNameText.text = $"{LZString.GetUIString(kitchenStaff)} :\n{LZString.GetUIString(string.Format(Strings.employeeNameKeyFormat, employeeData.StaffID))}";
                break;
        }
        button = GetComponentInChildren<Button>();
        workSpeedValue.text = employeeData.WorkSpeed.ToString();
        moveSpeedValue.text = employeeData.MoveSpeed.ToString();
        HealthValue.text = employeeData.Health.ToString();
        
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
        if (employeeSaveData[employeeId].level >= 5 || userData.Money < employeeData.Cost ||
            userData.Money < employeeData.Cost * employeeSaveData[employeeId].level)
        {
            return;
        }

        button.interactable = false;
        if (employeeSaveData[employeeId].level < 1 && userData.Money > employeeData.Cost)
        {
            ServiceLocator.Instance.GetSceneService<GameManager>().EmployeeManager.InstantiateAndRegisterWorker(employeeData);
            ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager.OnEventInvoked(MissionMainCategory.HireStaff, 1, (int)employeeData.StaffID);
            OnUpgradeEmployee();
        }
        else if (userData.Money > employeeData.Cost * employeeSaveData[employeeId].level)
        {
            ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager.OnEventInvoked(MissionMainCategory.UpgradeStaff, 1, (int)employeeData.StaffID);
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
                uiNameText.text = $"{LZString.GetUIString(cashierStaff)}:\n{LZString.GetUIString(string.Format(Strings.employeeNameKeyFormat, employeeData.StaffID))}:{employeeSaveData[employeeId].level}";
                break;
            case WorkType.Hall:
                uiNameText.text = $"{LZString.GetUIString(hallStaff)}:\n{LZString.GetUIString(string.Format(Strings.employeeNameKeyFormat, employeeData.StaffID))}:{employeeSaveData[employeeId].level}";
                break;
            case WorkType.Kitchen:
                uiNameText.text = $"{LZString.GetUIString(kitchenStaff)}:\n{LZString.GetUIString(string.Format(Strings.employeeNameKeyFormat, employeeData.StaffID))}:{employeeSaveData[employeeId].level}";
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
        
        int cost = employeeData.Cost * (employeeSaveData[employeeId].level + 1);
        
        float targetTime = Time.time + buyInterval;
        var alertPopup = ServiceLocator.Instance.GetGlobalService<AlertPopup>();
        
        if(employeeSaveData[employeeId].level == 0)
            alertPopup.PopUp("직원 고용중" ,"고용 고용!", SpumCharacter.HireEmployee, false);
        else
            alertPopup.PopUp("직원 교육중" ,"교육 교육!", SpumCharacter.HireEmployee, false);
        
        await UserDataManager.Instance.AdjustMoneyWithSave(-cost);
        await UserDataManager.Instance.UpgradeEmployee(employeeId);
        if (targetTime > Time.time)
        {
            await UniTask.WaitForSeconds(targetTime - Time.time);
        }

        UpdateUIAfterBuy();


        alertPopup.ChangeCharacter(SpumCharacter.HireEmployeeComplete);
        alertPopup.ChangeText("교육 완료!","만세!");
        alertPopup.EnableTouch();

    }

    private void UpdateUIAfterBuy()
    {
        ingameGoodsUi.SetCostUi();

        employeeManager.UpgradeEmployee(employeeId);

        employeeData.Health = employeeData.Health + (10 * employeeSaveData[employeeId].level);
        GameObject.FindWithTag("UIManager").GetComponent<UiManager>().EmployeeHpRenewal(employeeData);
        UserDataManager.Instance.AddRankPointWithSave(employeeData.RankPoint).Forget();
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