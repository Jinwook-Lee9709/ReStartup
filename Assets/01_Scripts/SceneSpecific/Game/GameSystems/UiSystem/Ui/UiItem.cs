using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class UiItem : MonoBehaviour
{
    public GameObject employee;

    [SerializeField] private Image image;

    [SerializeField] private TextMeshProUGUI uiNameText;

    [SerializeField] private TextMeshProUGUI uiUpgradeCostText;

    [SerializeField] private TextMeshProUGUI upgradeButtonText;

    //Fortest
    public EmployeeTableGetData employeeData;

    public FoodData foodData;

    private void Start()
    {
        if (employeeData != null)
        {
            StartCoroutine(LoadSpriteCoroutine(employeeData.Icon));
        }
        if (foodData != null)
        {
            StartCoroutine(LoadSpriteCoroutine(foodData.IconID));
        }
    }

    public void Init(EmployeeTableGetData data)
    {
        employeeData = data;
        uiNameText.text = $"{employeeData.StaffID}";
        uiUpgradeCostText.text = $"{employeeData.Cost}";
        var button = GetComponentInChildren<Button>();
        upgradeButtonText.text = "고용하기";
        button.onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
        {
            //if (userData.Gold < employeeData.Cost * employeeData.upgradeCount)
            //{
            //  return;
            //}
            if (employeeData.upgradeCount < 1)
            {
                var newEmployee = Instantiate(employee).GetComponent<EmployeeFSM>();
                newEmployee.EmployeeData = employeeData;
                var workerManager = ServiceLocator.Instance.GetSceneService<GameManager>().WorkerManager;
                workerManager.RegisterWorker(newEmployee, (WorkType)newEmployee.EmployeeData.StaffType);
            }
            upgradeButtonText.text = "업그레이드";
            employeeData.upgradeCount++;
            employeeData.OnUpgrade();
            uiNameText.text = $"{employeeData.StaffID} : {employeeData.upgradeCount}";
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
                upgradeButtonText.text = "업그레이드";
            }
            foodData.upgradeCount++;
            uiNameText.text = $"{foodData.FoodID}";
            uiUpgradeCostText.text = $"{foodData.BasicCost * foodData.upgradeCount}";
            button.GetComponent<TextMeshProUGUI>().text = "구매함";
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