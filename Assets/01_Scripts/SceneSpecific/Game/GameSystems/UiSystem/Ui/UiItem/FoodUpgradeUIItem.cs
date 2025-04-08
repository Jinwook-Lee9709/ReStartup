using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class FoodUpgradeUIItem : MonoBehaviour
{

    [SerializeField] private GameObject levelUpImage;

    [SerializeField] private Image image;

    [SerializeField] private TextMeshProUGUI levelText;

    //Fortest
    public EmployeeTableGetData employeeData;

    public FoodData foodData;

    private FoodUpgradeListUI foodUpgradeListUi;

    private Button button;

    private ConsumerManager consumerManager;

    private IngameGoodsUi ingameGoodsUi;
    private void Start()
    {
        ingameGoodsUi = GameObject.FindWithTag("UIManager").GetComponent<UiManager>().inGameUi;

        if (foodData != null)
        {
            StartCoroutine(LoadSpriteCoroutine(foodData.IconID));
            foodUpgradeListUi = GetComponentInParent<FoodUpgradeListUI>();
            if (foodUpgradeListUi == null)
            {
                Debug.LogError($"{gameObject.name}의 부모 중 foodUpgradeListUi를 찾을 수 없습니다.");
                return;
            }
            foodUpgradeListUi.AddButtonList(button);
        }
    }
    public void Init(FoodData data)
    {

        var userData = UserDataManager.Instance.CurrentUserData;
        foodData = data;
        levelUpImage.SetActive(false);
        levelText.text = $"{foodData.upgradeCount}";
        button = GetComponentInChildren<Button>();
        var gameManager = ServiceLocator.Instance.GetSceneService<GameManager>();
        consumerManager = gameManager.consumerManager;
        if (foodData.FoodID == 301001)
        {
            consumerManager.foodIds.Add(foodData.FoodID);
            foodData.upgradeCount = 1;
        }
        button.onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
        {
            if (foodData.upgradeCount >= 5)
            {
                return;
            }
            // add food unlock Requirements
            if (userData.Gold > foodData.BasicCost * foodData.upgradeCount)
            {
                foodData.upgradeCount++;

                userData.Gold -= foodData.BasicCost * foodData.upgradeCount;
                ingameGoodsUi.SetGoldUi();
            }

            if (foodData.upgradeCount >= 5)
            {
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
