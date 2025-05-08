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
    [SerializeField] private GameObject notEnoughCostPopUp;
    //Fortest
    public FoodData foodData;
    private FoodResearchListUI foodUpgradeListUi;
    private Button button;
    private ConsumerManager consumerManager;
    private IngameGoodsUi ingameGoodsUi;
    private UserData userData;
    private FoodResearchNotifyPopup upgradeAuthorityNotifyPopup;
    private GameManager gameManager;
    private bool chackCookWareUnlock;

    private float imageSizeFilter = 0.12f;
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
            // var newWidth = foodUpgradeListUi.gameObject.GetComponent<RectTransform>().rect.width * imageSizeFilter;
            // image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newWidth);
            // image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
        }
    }

    public void Init(FoodData data, FoodResearchNotifyPopup notifyPopup)
    {
        userData = UserDataManager.Instance.CurrentUserData;
        foodData = data;
        upgradeAuthorityNotifyPopup = notifyPopup;
        RankPointText.text = data.GetRankPoints.ToString();
        CostText.text = data.BasicCost.ToString();
        NameText.text = LZString.GetUIString(string.Format(Strings.foodNameKeyFormat, foodData.StringID));
        newImage.SetActive(false);
        button = GetComponentInChildren<Button>();
        gameManager = ServiceLocator.Instance.GetSceneService<GameManager>();
        consumerManager = gameManager.consumerManager;

        if (UserDataManager.Instance.CurrentUserData.FoodSaveData[foodData.FoodID].level != 0)
        {
            lockImage.SetActive(false);
            button.interactable = false;
            newImage.SetActive(false);
            consumerManager.foodIds.Add(foodData.FoodID);
            button.GetComponentInChildren<TextMeshProUGUI>().text = LZString.GetUIString(Strings.complete);
        }
        if (foodData.Requirements < userData.CurrentRankPoint && chackCookWareUnlock)
        {
            newImage.SetActive(true);
            lockImage.SetActive(false);
        }
        UnlockCookwareAmount();
        UnlockFood();
        button.onClick.AddListener(OnBuy);
        lockButton.onClick.AddListener(OnAuthorizationCheckButtonTouched);
    }

    public void UnlockFood()
    {
        if (chackCookWareUnlock && userData.CurrentRankPoint >= foodData.Requirements)
        {
            lockImage.SetActive(false);
            newImage.SetActive(UserDataManager.Instance.CurrentUserData.FoodSaveData[foodData.FoodID].level == 0);
        }

    }
    public void UnlockCookwareAmount()
    {
        var cookwareType = foodData.CookwareType;
        var currentTheme = gameManager.CurrentTheme;
        int currentCookwareAmount = userData.CookWareUnlock[currentTheme][cookwareType];
        chackCookWareUnlock = currentCookwareAmount >= foodData.CookwareNB;
    }

    public void OnAuthorizationCheckButtonTouched()
    {
        upgradeAuthorityNotifyPopup.SetRequirementText(foodData, chackCookWareUnlock);
        upgradeAuthorityNotifyPopup.gameObject.SetActive(true);
    }

    public void OnBuy()
    {
        if (!chackCookWareUnlock)
            return;
        if (userData.Money >= foodData.BasicCost)
        {
            Unlock().Forget();
            newImage.SetActive(false);
        }
        else
        {
            Instantiate(notEnoughCostPopUp, GameObject.FindWithTag("UIManager").GetComponentInChildren<Canvas>().transform);
        }
    }

    public async UniTask Unlock()
    {
        if (consumerManager.foodIds.Contains(foodData.FoodID))
        {
            return;
        }
        
        lockImage.SetActive(false);
        consumerManager.foodIds.Add(foodData.FoodID);
        float targetTime = Time.time + Constants.POP_UP_DURATION;
        var alertPopup = ServiceLocator.Instance.GetGlobalService<AlertPopup>();
        alertPopup.PopUp("음식 연구중!", "어떤 음식이 나오려나?", SpumCharacter.FoodResearch, false);
        bool isComplete = await UserDataManager.Instance.AddRankPointWithSave(foodData.GetRankPoints);
        if (!isComplete)
        {
            return;
        }
        if (targetTime > Time.time)
        {
            await UniTask.WaitForSeconds(targetTime - Time.time);
        }
        ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager.OnEventInvoked(MissionMainCategory.UnlockFood, 1, (int)foodData.FoodID);
        foodData.upgradeCount = 1;
        userData.Money -= foodData.BasicCost;
        ingameGoodsUi.SetCostUi();
        gameManager.foodManager.UnlockFoodUpgrade(foodData);
        button.interactable = false;
        button.GetComponentInChildren<TextMeshProUGUI>().text = LZString.GetUIString(Strings.complete);
        HandleUpgradeFood().Forget();
        
        alertPopup.ChangeCharacter(SpumCharacter.FoodResearchComplete);
        alertPopup.ChangeText("연구 완료!","만세!");
        alertPopup.EnableTouch();
        
        
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

    private async UniTask HandleUpgradeFood()
    {
        await UserDataManager.Instance.UpgradeFood(foodData.FoodID);
    }
}