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

    public FoodData foodData;

    private EmployeeUpgradeListUi employeeUpgradeData;

    private Button button;

    private readonly string employeePrefab = "Agent.prefab";

    private ConsumerManager consumerManager;

    private IngameGoodsUi ingameGoodsUi;

    private void Start()
    {
        ingameGoodsUi = GameObject.FindWithTag("UIManager").GetComponent<UiManager>().inGameUi;
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
        var userData = UserDataManager.Instance.CurrentUserData;
        employeeData = data;
        int buyCost;
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
                buyCost = employeeData.Cost;
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
                workerManager.RegisterWorker(newEmployee, (WorkType)newEmployee.EmployeeData.StaffType, newEmployee.EmployeeData.StaffID);
                upgradeButtonText.text = "업그레이드";
                employeeData.upgradeCount++;
                uiUpgradeCostText.text = $"{employeeData.Cost * employeeData.upgradeCount}";
                employeeData.OnUpgrade();
                userData.Gold -= buyCost;
                ingameGoodsUi.SetGoldUi();
            }
            if (userData.Gold > employeeData.Cost * employeeData.upgradeCount)
            {
                buyCost = employeeData.Cost * employeeData.upgradeCount;
                employeeData.upgradeCount++;
                employeeData.OnUpgrade();
                uiUpgradeCostText.text = $"{employeeData.Cost * employeeData.upgradeCount}";
                userData.Gold -= buyCost;
                ingameGoodsUi.SetGoldUi();
            }
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
            if (employeeData.upgradeCount >= 5)
            {
                upgradeButtonText.text = "완료됨";
                button.interactable = false;
            }
        }));

    }
    public void Init(FoodData data)
    {
        var userData = UserDataManager.Instance.CurrentUserData;
        foodData = data;
        uiNameText.text = $"{foodData.FoodID}";
        uiUpgradeCostText.text = $"{foodData.BasicCost}";
        var button = GetComponentInChildren<Button>();
        upgradeButtonText.text = "연구하기";
        consumerManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>().consumerManager;
        int buyCost;
        if (foodData.FoodID == 301001)
        {
            consumerManager.foodIds.Add(foodData.FoodID);
            foodData.upgradeCount = 1;
            upgradeButtonText.text = "업그레이드";
        }
        button.onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
        {
            if (foodData.upgradeCount < 1 && userData.Gold > foodData.BasicCost)
            {
                consumerManager.foodIds.Add(foodData.FoodID);
                foodData.isUnlock = true;
                upgradeButtonText.text = "업그레이드";
                buyCost = foodData.BasicCost;
                foodData.upgradeCount++;
                uiNameText.text = $"{foodData.FoodID}";
                uiUpgradeCostText.text = $"{foodData.BasicCost * foodData.upgradeCount}";
                userData.Gold -= buyCost;
                ingameGoodsUi.SetGoldUi();
            }
            if (userData.Gold > foodData.BasicCost * foodData.upgradeCount)
            {
                buyCost = foodData.BasicCost * foodData.upgradeCount;
                foodData.upgradeCount++;
                uiNameText.text = $"{foodData.FoodID}";
                uiUpgradeCostText.text = $"{foodData.BasicCost * foodData.upgradeCount}";
                userData.Gold -= buyCost;
                ingameGoodsUi.SetGoldUi();
            }

            if (foodData.upgradeCount >= 5)
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

}