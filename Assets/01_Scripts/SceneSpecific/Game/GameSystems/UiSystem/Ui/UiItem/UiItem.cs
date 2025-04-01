using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
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

    public FoodData foodData;

    private EmployeeUpgradeListUi employeeUpgradeData;

    private Button button;

    private readonly string employeePrefab = "Agent.prefab";

    private void Start()
    {

        if (employeeData != null)
        {
            StartCoroutine(LoadSpriteCoroutine(employeeData.Icon));
            employeeUpgradeData = GetComponentInParent<EmployeeUpgradeListUi>();
            if (employeeUpgradeData == null)
            {
                Debug.LogError($"{gameObject.name}의 부모 중 EmployeeUpgradeListUi를 찾을 수 없습니다.");
                return;  // Null이면 실행 중단
            }
            employeeUpgradeData.AddButtonList(button);
        }
        if (foodData != null)
        {
            StartCoroutine(LoadSpriteCoroutine(foodData.IconID));
        }

    }

    public void Init(EmployeeTableGetData data)
    {

        employeeData = data;
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
            if (employeeData.upgradeCount < 1)
            {
                //Spwn Staff
                var handle = Addressables.LoadAssetAsync<GameObject>(employeePrefab);
                GameObject prefab = await handle.Task;
                var newEmployee = Instantiate(prefab).GetComponent<EmployeeFSM>();
                newEmployee.EmployeeData = employeeData;

                switch ((WorkType)employeeData.StaffType)
                {
                    case WorkType.All:
                        break;
                    case WorkType.Payment:
                        newEmployee.GetComponent<SpriteRenderer>().color = Color.yellow;
                        break;
                    case WorkType.Hall:
                        newEmployee.GetComponent<SpriteRenderer>().color = Color.blue;
                        break;
                    case WorkType.Kitchen:
                        newEmployee.GetComponent<SpriteRenderer>().color = Color.red;
                        break;
                }
                newEmployee.GetComponentInChildren<TextMeshPro>().text = $"{((WorkType)employeeData.StaffType).ToString()}직원";
                var workerManager = ServiceLocator.Instance.GetSceneService<GameManager>().WorkerManager;
                workerManager.RegisterWorker(newEmployee, (WorkType)newEmployee.EmployeeData.StaffType);
            }
            upgradeButtonText.text = "업그레이드";
            employeeData.upgradeCount++;
            employeeData.OnUpgrade();
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
            uiUpgradeCostText.text = $"{employeeData.Cost * employeeData.upgradeCount}";
            if (employeeData.upgradeCount >= 5)
            {
                button.interactable = false;
            }
        }));

    }
    public void Init(FoodData data)
    {
        foodData = data;
        uiNameText.text = $"{foodData.FoodID}";
        uiUpgradeCostText.text = $"{foodData.BasicCost}";
        var button = GetComponentInChildren<Button>();
        upgradeButtonText.text = "연구하기";
        button.onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
        {
            if (foodData.upgradeCount < 1)
            {
                foodData.isUnlock = true;
                upgradeButtonText.text = "업그레이드";
            }
            foodData.upgradeCount++;
            uiNameText.text = $"{foodData.FoodID}";
            uiUpgradeCostText.text = $"{foodData.BasicCost * foodData.upgradeCount}";
            button.GetComponentInChildren<TextMeshProUGUI>().text = "구매함";
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

}