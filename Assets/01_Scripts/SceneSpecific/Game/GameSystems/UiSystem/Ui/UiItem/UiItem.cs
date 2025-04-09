using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem.Composites;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class UiItem : MonoBehaviour
{
    [SerializeField] private Image image;

    [SerializeField] private TextMeshProUGUI uiNameText;

    [SerializeField] private TextMeshProUGUI uiUpgradeCostText;

    [SerializeField] private TextMeshProUGUI upgradeButtonText;

    //Fortest
    public EmployeeTableGetData employeeData;

    private EmployeeUpgradeListUi employeeUpgradeListUi;

    private Button button;

    private readonly string employeePrefab = "Agent.prefab";

    private IngameGoodsUi ingameGoodsUi;

    private void Start()
    {
        ingameGoodsUi = GameObject.FindWithTag("UIManager").GetComponent<UiManager>().inGameUi;
        if (employeeData != null)
        {
            StartCoroutine(LoadSpriteCoroutine(employeeData.Icon));
            employeeUpgradeListUi = GetComponentInParent<EmployeeUpgradeListUi>();
            if (employeeUpgradeListUi == null)
            {
                Debug.LogError($"{gameObject.name}의 부모 중 EmployeeUpgradeListUi를 찾을 수 없습니다.");
                return;
            }
            employeeUpgradeListUi.AddButtonList(button);
        }
        
    }

    public void Init(EmployeeTableGetData data)
    {
        var userData = UserDataManager.Instance.CurrentUserData;
        employeeData = data;
        var gameManager = ServiceLocator.Instance.GetSceneService<GameManager>();
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
        upgradeButtonText.text = "고용하기";
        button.onClick.AddListener((UnityEngine.Events.UnityAction)(async () =>
        {
            //if (userData.Gold < employeeData.Cost * employeeData.upgradeCount)
            //{
            //  return;
            //}
            if (employeeData.upgradeCount >= 5)
            {
                return;
            }
            if (employeeData.upgradeCount < 1 && userData.Gold > employeeData.Cost)
            {

                var handle = Addressables.LoadAssetAsync<GameObject>(employeePrefab);
                GameObject prefab = await handle.Task;
                var newEmployee = Instantiate(prefab).GetComponent<EmployeeFSM>();
                newEmployee.EmployeeData = employeeData;
                // var spriteRenderer = newEmployee.GetComponent<SpriteRenderer>();
                //
                // switch ((WorkType)employeeData.StaffType)
                // {
                //     case WorkType.Payment:
                //         spriteRenderer.color = Color.yellow;
                //         break;
                //     case WorkType.Hall:
                //         spriteRenderer.color = Color.blue;
                //         break;
                //     case WorkType.Kitchen:
                //         spriteRenderer.color = Color.red;
                //         break;
                // }
                newEmployee.GetComponentInChildren<TextMeshPro>().text = $"{((WorkType)employeeData.StaffType).ToString()}직원";
                var workerManager = gameManager.WorkerManager;
                workerManager.RegisterWorker(newEmployee, (WorkType)newEmployee.EmployeeData.StaffType, newEmployee.EmployeeData.StaffID);

                userData.Gold -= employeeData.Cost;
                OnUpgradeEmployee();
            }
            if (userData.Gold > employeeData.Cost * employeeData.upgradeCount)
            {
                OnUpgradeEmployee();
                userData.Gold -= employeeData.Cost * employeeData.upgradeCount;
            }
            if (employeeData.upgradeCount >= 5)
            {
                upgradeButtonText.text = "완료됨";
                button.interactable = false;
            }
        }));

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
        upgradeButtonText.text = "업그레이드";
        employeeData.upgradeCount++;
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
    }
}