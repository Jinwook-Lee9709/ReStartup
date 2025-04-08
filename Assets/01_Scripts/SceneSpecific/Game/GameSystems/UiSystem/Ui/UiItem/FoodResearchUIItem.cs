using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using static UnityEditor.Progress;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class FoodResearchUIItem : MonoBehaviour
{

    [SerializeField] private GameObject newImage;
    [SerializeField] private Image image;
    [SerializeField] private GameObject lockImage;
    [SerializeField] private TextMeshProUGUI NameText;
    [SerializeField] private TextMeshProUGUI CostText;
    [SerializeField] private TextMeshProUGUI RankPointText;

    //Fortest
    public EmployeeTableGetData employeeData;

    public FoodData foodData;

    private FoodResearchListUI foodUpgradeListUi;

    private Button button;

    private ConsumerManager consumerManager;

    private IngameGoodsUi ingameGoodsUi;

    private UserData userData;
    private void Start()
    {
        ingameGoodsUi = GameObject.FindWithTag("UIManager").GetComponent<UiManager>().inGameUi;

        if (foodData != null)
        {
            StartCoroutine(LoadSpriteCoroutine(foodData.IconID));
            foodUpgradeListUi = GetComponentInParent<FoodResearchListUI>();
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

        userData = UserDataManager.Instance.CurrentUserData;
        foodData = data;
        RankPointText.text = data.GetRankPoints.ToString();
        CostText.text = data.BasicCost.ToString();
        NameText.text = $"{foodData.FoodID.ToString()}";
        newImage.SetActive(false);
        button = GetComponentInChildren<Button>();
        var gameManager = ServiceLocator.Instance.GetSceneService<GameManager>();
        consumerManager = gameManager.consumerManager;
        if (foodData.FoodID == 301001)
        {
            consumerManager.foodIds.Add(foodData.FoodID);
            foodData.upgradeCount = 1;
            lockImage.SetActive(false);
            button.interactable = false;
        }
        else
        {
            if (foodData.Requirements < userData.CurrentRankPoint)
            {
                lockImage.SetActive(false);
            }
        }
        button.onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
        {
            // add food unlock Requirements
            if (userData.Gold > foodData.BasicCost)
            {
                var cookwareType = foodData.CookwareType;
                var currentTheme = gameManager.CurrentTheme;
                int currentCookwareAmount = userData.CookWareUnlock[currentTheme][cookwareType];
                if (currentCookwareAmount < foodData.CookwareNB)
                    return;

                consumerManager.foodIds.Add(foodData.FoodID);
                foodData.isUnlock = true;
                foodData.upgradeCount++;
                userData.CurrentRankPoint += foodData.GetRankPoints;
                userData.Gold -= foodData.BasicCost;
                ingameGoodsUi.SetGoldUi();
                gameManager.foodManager.UnlockFoodUpgrade(foodData);
                button.interactable = false;
            }
        }));
    }
    public void UnlockFood()
    {
        lockImage.SetActive(false);
        userData.CurrentRankPoint += foodData.GetRankPoints;
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
