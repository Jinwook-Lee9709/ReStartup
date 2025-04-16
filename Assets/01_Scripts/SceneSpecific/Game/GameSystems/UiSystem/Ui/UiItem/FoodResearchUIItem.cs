using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class FoodResearchUIItem : MonoBehaviour
{
    [SerializeField] private GameObject newImage;
    [SerializeField] private Image image;
    [SerializeField] private GameObject lockImage;
    [SerializeField] private TextMeshProUGUI NameText;
    [SerializeField] private TextMeshProUGUI CostText;
    [SerializeField] private TextMeshProUGUI RankPointText;
    [SerializeField] private Button lockButton;

    //Fortest
    public EmployeeTableGetData employeeData;
    public FoodData foodData;
    private FoodResearchListUI foodUpgradeListUi;
    private Button button;
    private ConsumerManager consumerManager;
    private IngameGoodsUi ingameGoodsUi;
    private UserData userData;
    private FoodResearchNotifyPopup upgradeAuthorityNotifyPopup;
    private GameManager gameManager;
    private bool chackCookWareUnlock;
    
    private Dictionary<int, FoodSaveData> foodSaveData;

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

            foodUpgradeListUi.FoodAllBuy += Unlock;
        }
    }

    public void Init(FoodData data, FoodResearchNotifyPopup notifyPopup)
    {
        userData = UserDataManager.Instance.CurrentUserData;
        foodData = data;
        upgradeAuthorityNotifyPopup = notifyPopup;
        RankPointText.text = data.GetRankPoints.ToString();
        CostText.text = data.BasicCost.ToString();
        NameText.text = $"{foodData.FoodID.ToString()}";
        newImage.SetActive(false);
        button = GetComponentInChildren<Button>();
        gameManager = ServiceLocator.Instance.GetSceneService<GameManager>();
        consumerManager = gameManager.consumerManager;
        var cookwareType = foodData.CookwareType;
        var currentTheme = gameManager.CurrentTheme;
        int currentCookwareAmount = userData.CookWareUnlock[currentTheme][cookwareType];
        chackCookWareUnlock = currentCookwareAmount >= foodData.CookwareNB;
#if UNITY_EDITOR
        if (foodData.FoodID == 301001)
        {
            consumerManager.foodIds.Add(foodData.FoodID);
            foodData.upgradeCount = 1;
            lockImage.SetActive(false);
            button.interactable = false;
            button.GetComponentInChildren<TextMeshProUGUI>().text = "연구됨";
        }
#endif
        if (UserDataManager.Instance.CurrentUserData.FoodSaveData[foodData.FoodID].level != 0)
        {
            lockImage.SetActive(false);
            button.interactable = false;
            consumerManager.foodIds.Add(foodData.FoodID);
        }
            
        if (foodData.Requirements < userData.CurrentRankPoint && chackCookWareUnlock)
        {
            lockImage.SetActive(false);
        }

        button.onClick.AddListener(OnBuy);
        lockButton.onClick.AddListener(OnAuthorizationCheckButtonTouched);
    }

    public void UnlockFood()
    {
        lockImage.SetActive(false);
    }

    public void OnAuthorizationCheckButtonTouched()
    {
        upgradeAuthorityNotifyPopup.SetRequirementText(foodData, chackCookWareUnlock);
        upgradeAuthorityNotifyPopup.gameObject.SetActive(true);
    }

    public void OnBuy()
    {
        var cookwareType = foodData.CookwareType;
        var currentTheme = gameManager.CurrentTheme;
        int currentCookwareAmount = userData.CookWareUnlock[currentTheme][cookwareType];
        chackCookWareUnlock = currentCookwareAmount >= foodData.CookwareNB;
        if (!chackCookWareUnlock)
            return;
        if (userData.Money > foodData.BasicCost)
        {
            Unlock();
        }
    }

    public void Unlock()
    {
        lockImage.SetActive(false);
        consumerManager.foodIds.Add(foodData.FoodID);
        
        userData.CurrentRankPoint += foodData.GetRankPoints;
        userData.Money -= foodData.BasicCost;
        ingameGoodsUi.SetGoldUi();
        gameManager.foodManager.UnlockFoodUpgrade(foodData);
        button.interactable = false;
        
        HandleUpgradeEmployee().Forget();
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

    private async UniTask HandleUpgradeEmployee()
    {
        await UserDataManager.Instance.UpgradeFood(foodData.FoodID);
    }
}