using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class FoodUpgradeUIItem : MonoBehaviour
{
    public Image image;
    [SerializeField] private GameObject lockImage;
    [SerializeField] private GameObject newImage;
    [SerializeField] private GameObject payableArrowImage;
    [SerializeField] private TextMeshProUGUI levelText;


    //Fortest
    public EmployeeTableGetData employeeData;

    public FoodData foodData;

    private FoodUpgradeListUI foodUpgradeListUi;

    private Button button;

    private ConsumerManager consumerManager;
    private FoodUpgradePopup foodUpgradePopup;

    private IngameGoodsUi ingameGoodsUi;
    [SerializeField] private GameObject notEnoughCostPopUp;

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
        }
    }

    public void Init(FoodData data, FoodUpgradePopup popup)
    {
        foodData = data;
        foodUpgradePopup = popup;
        button = GetComponentInChildren<Button>();

        foodData.upgradeCount = UserDataManager.Instance.CurrentUserData.FoodSaveData[foodData.FoodID].level;

        if (foodData.upgradeCount != 0)
        {
            newImage.SetActive(true);
            lockImage.SetActive(false);
        }

        if (foodData.upgradeCount >= 5)
        {
            OnFoodLevelMax();
        }

        levelText.text = foodData.upgradeCount.ToString();
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        foodUpgradePopup.gameObject.SetActive(true);
        foodUpgradePopup.SetInfo(this);
    }

    public void OnBuy()
    {
        if (foodData.upgradeCount >= 5)
        {
            return;
        }

        var userData = UserDataManager.Instance.CurrentUserData;
        if (userData.Money > foodData.BasicCost * foodData.upgradeCount)
        {
            ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager
                .OnEventInvoked(MissionMainCategory.UpgradeFood, 1, (int)foodData.FoodID);
            foodData.upgradeCount++;
            foodUpgradePopup.SetInfo(this);
            levelText.text = foodData.upgradeCount.ToString();
            userData.Money -= foodData.BasicCost * foodData.upgradeCount;
            ingameGoodsUi.SetCostUi();
            HandleUpgradeFood().Forget();
        }
        else
        {
            Instantiate(notEnoughCostPopUp,
                GameObject.FindWithTag("UIManager").GetComponentInChildren<Canvas>().transform);
        }

        if (foodData.upgradeCount >= 5)
        {
            OnFoodLevelMax();
        }
    }

    private void OnFoodLevelMax()
    {
        button.interactable = false;
        newImage.SetActive(false);
        payableArrowImage.SetActive(false);
    }

    public void UnlockFoodUpgrade()
    {
        lockImage.SetActive(false);
        newImage.SetActive(true);
        foodData.upgradeCount = 1;
        levelText.text = foodData.upgradeCount.ToString();
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

    public void UpdatePayableArrow(int currentMoney)
    {
        if (foodData.upgradeCount >= 5)
        {
            payableArrowImage.SetActive(false);
        }
        else
        {
            payableArrowImage.SetActive(currentMoney > foodData.BasicCost * foodData.upgradeCount);
        }
    }

    private async UniTask HandleUpgradeFood()
    {
        await UserDataManager.Instance.UpgradeFood(foodData.FoodID);
    }
}