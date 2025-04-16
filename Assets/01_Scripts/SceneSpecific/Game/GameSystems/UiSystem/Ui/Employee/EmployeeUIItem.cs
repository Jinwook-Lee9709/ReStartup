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
                uiNameText.text = $"계산원 :\n{employeeData.StaffID}";
                break;
            case WorkType.Hall:
                uiNameText.text = $"홀직원 :\n{employeeData.StaffID}";
                break;
            case WorkType.Kitchen:
                uiNameText.text = $"주방직원 :\n{employeeData.StaffID}";
                break;
        }
        uiUpgradeCostText.text = $"{employeeData.Cost}";
        button = GetComponentInChildren<Button>();
        upgradeButtonText.text = "고용";
        workSpeedValue.text = employeeData.WorkSpeed.ToString();
        moveSpeedValue.text = employeeData.MoveSpeed.ToString();
        HealthValue.text = employeeData.Health.ToString();
        
        SetUpgradeButtonText(employeeSaveData[employeeId].level);
        button.onClick.AddListener(OnButtonClick);

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
        if (employeeSaveData[employeeId].level >= 5)
        {
            return;
        }
        button.interactable = false;
        if (employeeSaveData[employeeId].level < 1 && userData.Money > employeeData.Cost)
        {
            ServiceLocator.Instance.GetSceneService<GameManager>().EmployeeManager.InstantiateAndRegisterWorker(employeeData);

            userData.Money -= employeeData.Cost;
            OnUpgradeEmployee();
        }
        if (userData.Money > employeeData.Cost * employeeData.upgradeCount)
        {
            OnUpgradeEmployee();
            userData.Money -= employeeData.Cost * employeeData.upgradeCount;
        }
        SetUpgradeButtonText(employeeSaveData[employeeId].level);
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
        ingameGoodsUi.SetGoldUi();
        uiUpgradeCostText.text = $"{employeeData.Cost * employeeData.upgradeCount}";
        switch ((WorkType)employeeData.StaffType)
        {
            case WorkType.All:
                break;
            case WorkType.Payment:
                uiNameText.text = $"계산원 :\n{employeeData.StaffID}:{employeeData.upgradeCount}";
                break;
            case WorkType.Hall:
                uiNameText.text = $"홀직원 :\n{employeeData.StaffID}:{employeeData.upgradeCount}";
                break;
            case WorkType.Kitchen:
                uiNameText.text = $"주방직원 :\n{employeeData.StaffID}:{employeeData.upgradeCount}";
                break;
        }
        HandleUpgradeEmployee().Forget();
    }

    private void SetUpgradeButtonText(int level)
    {
        switch (level)
        {
            case 0:
                upgradeButtonText.text = "고용";   
                break;
            case > 0 and < 5:
                upgradeButtonText.text = "교육";
                break;
            default:
                upgradeButtonText.text = "완료됨";
                break;

        }
    }

    private async UniTask HandleUpgradeEmployee()
    {
        await UserDataManager.Instance.UpgradeEmployee(employeeId);
        if (employeeSaveData[employeeId].level < 5)
        {
            button.interactable = true;
        }
    }
    
}