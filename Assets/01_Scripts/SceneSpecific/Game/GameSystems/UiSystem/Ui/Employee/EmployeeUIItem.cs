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
    static readonly string employeeName = "EmployeeName{0}";
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
    [SerializeField] private GameObject notEnoughCostPopUp;

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
        }
        UserDataManager.Instance.ChangeMoneyAction += OnConditionChanged;
        GameObject.FindWithTag("UIManager").GetComponent<UiManager>().EmployeeHpRenewal(employeeData);
    }

    private void OnDestroy()
    {
        if (UserDataManager.Instance != null)
            UserDataManager.Instance.ChangeMoneyAction -= OnConditionChanged;
    }

    public void Init(EmployeeTableGetData data, EmployeeUpgradePopup employeeUpgradePopup)
    {
        employeeData = data;
        employeeData.StartValueSet();
        employeeId = employeeData.StaffID;

        this.employeeUpgradePopup = employeeUpgradePopup;
        uiNameText.text = LZString.GetUIString(string.Format(employeeName, employeeData.StaffID));

        button = GetComponentInChildren<Button>();
        employeeData.UpdateUpgradeStats(employeeSaveData[employeeId].level);
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
        if (employeeSaveData[employeeId].level >= 5)
        {
            return;
        }
        if (userData.Money < employeeData.Cost)
        {
            Instantiate(notEnoughCostPopUp, ServiceLocator.Instance.GetSceneService<GameManager>().uiManager.canvas.transform);
            return;
        }

        button.interactable = false;
        if (employeeSaveData[employeeId].level < 1 && userData.Money >= employeeData.Cost)
        {
            ServiceLocator.Instance.GetSceneService<GameManager>().EmployeeManager.InstantiateAndRegisterWorker(employeeData);
            ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager.OnEventInvoked(MissionMainCategory.HireStaff, 1, (int)employeeData.StaffID);
            OnUpgradeEmployee();
        }
        else if (userData.Money >= employeeData.Cost * employeeSaveData[employeeId].level)
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
        int payLevel = UserDataManager.Instance.CurrentUserData.EmployeeSaveData[employeeData.StaffID].level;
        if (payLevel >= 5)
        {
            uiUpgradeCostText.text = LZString.GetUIString(employeeUpgradeFix);
        }
        else
        {
            uiUpgradeCostText.text = $"{employeeData.Cost}";
        }
        uiNameText.text = LZString.GetUIString(string.Format(employeeName, employeeData.StaffID));

        workSpeedValue.text = employeeData.WorkSpeed.ToString();
        moveSpeedValue.text = employeeData.MoveSpeed.ToString();
        HealthValue.text = employeeData.Health.ToString();
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

        float targetTime = Time.time + Constants.POP_UP_DURATION;
        var alertPopup = ServiceLocator.Instance.GetGlobalService<AlertPopup>();

        if (employeeSaveData[employeeId].level == 0)
        {
            var title = LZString.GetUIString("HireEmployee");
            var message = LZString.GetUIString("HireEmployeeMessage");
            alertPopup.PopUp(title, message, SpumCharacter.HireEmployee, false);
        }
        else
        {
            var title = LZString.GetUIString("EducateEmployee");
            var message = LZString.GetUIString("EducateEmployeeMessage");
            alertPopup.PopUp(title, message, SpumCharacter.HireEmployee, false);
        }


        await UserDataManager.Instance.AdjustMoneyWithSave(-employeeData.Cost);
        await UserDataManager.Instance.UpgradeEmployee(employeeId);
        employeeData.UpdateUpgradeStats(employeeSaveData[employeeId].level);
        if (targetTime > Time.time)
        {
            await UniTask.WaitForSeconds(targetTime - Time.time);
        }

        UpdateUIAfterBuy();


        alertPopup.ChangeCharacter(SpumCharacter.HireEmployeeComplete);
        if (employeeSaveData[employeeId].level == 1)
        {
            var title = LZString.GetUIString("HiringComplete");
            var message = LZString.GetUIString("Hurray");
            alertPopup.ChangeText(title, message);
        }
        else
        {
            var title = LZString.GetUIString("EducationComplete");
            var message = LZString.GetUIString("Hurray");
            alertPopup.ChangeText(title, message);
        }
        alertPopup.EnableTouch();

    }

    private void UpdateUIAfterBuy()
    {
        ingameGoodsUi.SetCostUi();

        employeeManager.UpgradeEmployee(employeeId);

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
        return data.Cost <= UserDataManager.Instance.CurrentUserData.Money;
    }
}