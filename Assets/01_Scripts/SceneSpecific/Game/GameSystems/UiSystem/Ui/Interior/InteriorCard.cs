using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class InteriorCard : MonoBehaviour
{
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


    public bool IsStateChanged
    {
        get
        {
            var userData = UserDataManager.Instance.CurrentUserData;
            var isGoldConditionChanged = isGoldEnough != userData.Gold >= Data.GetSellingCost();
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
    }

    public void UpdateInteractable()
    {
        var userData = UserDataManager.Instance.CurrentUserData;

        var upgradeLevel = userData.InteriorSaveData[Data.InteriorID];
        var cost = Data.GetSellingCost();

        costText.text = cost.ToString();
        isGoldEnough = userData.Gold >= cost;

        isSatisfyRequirements = CheckRequirements(userData);

        UpdatePanels();
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
            OnBuy();
        }
    }

    public void OnBuy()
    {
        UserDataManager.Instance.UpgradeInterior(Data.InteriorID);
        UserDataManager.Instance.ModifyGold(-Data.SellingCost);
        UpdateInteractable();
    }

    private bool CheckRequirements(UserData userData)
    {
        return userData.CurrentRankPoint >= Data.Requirements1 &&
               (Data.Requirements2 == 0 || userData.InteriorSaveData[Data.Requirements2] != 0);
    }

    private void UpdatePanels()
    {
        authorityCheckPanel.SetActive(!isSatisfyRequirements);
        costPanel.SetActive(isSatisfyRequirements);
    }

    private void UpdateButtonAndText(int upgradeLevel)
    {
        if (isSatisfyRequirements)
        {
            //FIXME:스트링테이블로 분리
            buyButtonText.text = upgradeLevel == 0 ? "구매" : "신상품";
            //

            buyButton.interactable = isGoldEnough;
            costText.color = isGoldEnough ? Color.white : Color.red;
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
}