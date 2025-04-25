using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class InteriorCard : MonoBehaviour
{
    private static readonly string BuyStringID = "Buy";
    private static readonly string NewProductStringID = "GodProduct";
    private static readonly string PurchaseCompleteStringID = "PurchaseComplete";

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private GameObject costPanel;
    [SerializeField] private Image icon;
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI buyButtonText;
    [SerializeField] private Button authorizationCheckButton;
    [SerializeField] private GameObject authorityCheckPanel;


    private bool isGoldEnough;
    private bool isSatisfyRequirements;
    private InteriorUpgradeAuthorityNotifyPopup upgradeAuthorityNotifyPopup;
    private InteriorUpgradePopup upgradePopup;

    private string currentButtonStringId;

    public bool IsStateChanged
    {
        get
        {
            var userData = UserDataManager.Instance.CurrentUserData;
            var isGoldConditionChanged = isGoldEnough != userData.Money >= Data.GetSellingCost();
            var isRequirementsChanged = isSatisfyRequirements !=
                                        (userData.CurrentRankPoint >= Data.Requirements1 &&
                                         (Data.Requirements2 == 0 ||
                                          userData.InteriorSaveData[Data.Requirements2] != 0));
            return isGoldConditionChanged || isRequirementsChanged;
        }
    }

    public InteriorData Data { get; private set; }

    public bool IsInteractable => buyButton.interactable;

    public void Init(InteriorData data, InteriorUpgradePopup popup,
        InteriorUpgradeAuthorityNotifyPopup authorityNotifyPopup)
    {
        this.Data = data;
        upgradePopup = popup;
        upgradeAuthorityNotifyPopup = authorityNotifyPopup;
        nameText.text = data.Name;

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnButtonClick);
        authorizationCheckButton.onClick.RemoveAllListeners();
        authorizationCheckButton.onClick.AddListener(OnAuthorizationCheckButtonTouched);
        UpdateInteractable();

        LocalizationSettings.SelectedLocaleChanged += OnLanguageChanged;
    }

    public void UpdateInteractable()
    {
        var userData = UserDataManager.Instance.CurrentUserData;

        var upgradeLevel = userData.InteriorSaveData[Data.InteriorID];
        var cost = Data.GetSellingCost();

        costText.text = cost.ToString();
        isGoldEnough = userData.Money >= cost;

        isSatisfyRequirements = CheckRequirements(userData);

        UpdatePanels(upgradeLevel);
        UpdateButtonAndText(upgradeLevel);
        UpdateImage();
    }

    private void OnButtonClick()
    {
        if (UserDataManager.Instance.CurrentUserData.InteriorSaveData[Data.InteriorID] != 0)
        {
            upgradePopup.gameObject.SetActive(true);
            upgradePopup.SetInfo(this);
        }
        else
        {
            buyButton.interactable = false;
            OnBuy().Forget();
        }
    }

    public async UniTask OnBuy()
    {
        float targetTime = Time.time + Constants.POP_UP_DURATION;
        var alertPopup = ServiceLocator.Instance.GetGlobalService<AlertPopup>();
        alertPopup.PopUp("인테리어 구매중!", "영차 영차!", SpumCharacter.Construct, false);
        await UserDataManager.Instance.AdjustMoneyWithSave(-Data.GetSellingCost());
        await UserDataManager.Instance.UpgradeInterior(Data.InteriorID);
        if (Time.time < targetTime)
        {
            await UniTask.WaitForSeconds(targetTime - Time.time);
        }
        UpdateInteractable();
        if (UserDataManager.Instance.CurrentUserData.InteriorSaveData[Data.InteriorID] == 1)
            ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager.OnEventInvoked(MissionMainCategory.BuyInterior, 1, (int)Data.InteriorID);
        else
            ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager.OnEventInvoked(MissionMainCategory.UpgradeInterior, 1, (int)Data.InteriorID);
        alertPopup.ChangeCharacter(SpumCharacter.ConstructComplete);
        alertPopup.ChangeText("구매 완료!", "만세!");
        alertPopup.EnableTouch();
    }

    private bool CheckRequirements(UserData userData)
    {
        return userData.CurrentRankPoint >= Data.Requirements1 &&
               (Data.Requirements2 == 0 || userData.InteriorSaveData[Data.Requirements2] != 0);
    }

    private void UpdatePanels(int upgradeLevel)
    {
        authorityCheckPanel.SetActive(!isSatisfyRequirements);
        costPanel.SetActive(isSatisfyRequirements && upgradeLevel != Data.MaxUpgradeCount);
    }

    private void UpdateButtonAndText(int upgradeLevel)
    {
        if (isSatisfyRequirements)
        {
            //FIXME:스트링테이블로 분리
            if (upgradeLevel == 0)
            {
                currentButtonStringId = BuyStringID;

            }
            else if (upgradeLevel == Data.MaxUpgradeCount)
            {
                currentButtonStringId = PurchaseCompleteStringID;
            }
            else
            {
                currentButtonStringId = NewProductStringID;
            }
            buyButtonText.text = LZString.GetUIString(currentButtonStringId);
            buyButton.interactable = isGoldEnough && upgradeLevel != Data.MaxUpgradeCount;
            costText.color = isGoldEnough || upgradeLevel == Data.MaxUpgradeCount ? Color.white : Color.red;
        }
    }

    private void UpdateImage()
    {
        var level = UserDataManager.Instance.CurrentUserData.InteriorSaveData[Data.InteriorID];

        Sprite sprite = null;
        if (Data.IconID == "Dummy")
            sprite = Addressables.LoadAssetAsync<Sprite>(Data.IconID).WaitForCompletion();
        else if (level != 0)
            sprite = Addressables.LoadAssetAsync<Sprite>(Data.IconID + level).WaitForCompletion();
        else
            sprite = Addressables.LoadAssetAsync<Sprite>(Data.IconID + 1).WaitForCompletion();

        icon.sprite = sprite;
    }

    public void OnAuthorizationCheckButtonTouched()
    {
        upgradeAuthorityNotifyPopup.SetRequirementText(Data);
        upgradeAuthorityNotifyPopup.gameObject.SetActive(true);
    }

    private void OnLanguageChanged(Locale locale)
    {
        buyButtonText.text = LZString.GetUIString(currentButtonStringId);
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLanguageChanged;
    }
}