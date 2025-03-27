using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class UiItem : MonoBehaviour
{
    //Fortest
    public EmployeeTableGetData employeeData;
    public GameObject employee;
    [SerializeField]
    private Image image;
    [SerializeField]
    private TextMeshProUGUI employeeNameText;
    [SerializeField]
    private TextMeshProUGUI EmployeeUpgradeCostText;
    private void Start()
    {
        StartCoroutine(LoadSpriteCoroutine(employeeData.Icon));
    }

    public void Init(EmployeeTableGetData data)
    {
        employeeData = data;
        employeeNameText.text = $"{employeeData.StaffID}";
        EmployeeUpgradeCostText.text = $"{employeeData.Cost}";
        var button = GetComponentInChildren<Button>();
        button.onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
        {
            //if (userData.Gold < employeeData.Cost * employeeData.upgradeCount)
            //{
            //  return;
            //}
            if (employeeData.upgradeCount < 1)
            {
                var newEmployee = Instantiate(employee).GetComponent<EmployeeFSM>();
                newEmployee.Init(ServiceLocator.Instance.GetSceneService<GameManager>().WorkerManager);
                newEmployee.EmployeeData = employeeData;
            }
            employeeData.upgradeCount++;
            employeeNameText.text = $"{this.employeeData.StaffID} : {employeeData.upgradeCount}";
            EmployeeUpgradeCostText.text = $"{employeeData.Cost * employeeData.upgradeCount}";

            employeeData.OnUpgrade();
        }));
    }
    private IEnumerator LoadSpriteCoroutine(string iconAddress)
    {
        var handle = Addressables.LoadAssetAsync<Sprite>(iconAddress);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            image.sprite = handle.Result;
        }
        else
        {
            Debug.LogError($"Failed to load sprite: {iconAddress}");
        }
    }
}