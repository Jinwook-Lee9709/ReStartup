using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class InteriorUIManager : MonoBehaviour
{
    [SerializeField] private Button hallButton;
    [SerializeField] private Button kitchenButton;
    [SerializeField] private Button closeButton;
    
    [SerializeField] private Transform cardGroupParent;
    [SerializeField] private AssetReference interiorCardGroupPrefab;

    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI goldText;
    
    [SerializeField] private List<InteriorCardGroup> hallCardGroups;
    [SerializeField] private List<InteriorCardGroup> kitchenCardGroups;

    [SerializeField] private InteriorUpgradePopup popup;
    [SerializeField] private InteriorUpgradeAuthorityNotifyPopup authorityPopup;
    
    private void Start()
    {
        Dictionary<InteriorCategory, List<InteriorData>> hallList;
        Dictionary<InteriorCategory, List<InteriorData>> kitchenList;

        var table = DataTableManager.Get<InteriorDataTable>(DataTableIds.Interior.ToString());
        var dataList = table
            .Where(x => x.RestaurantType == (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme)
            .ToList();

        InitCardGroups(dataList, ObjectArea.Hall);
        InitCardGroups(dataList, ObjectArea.Kitchen);
        InitButtonEvent();

        UpdateTexts();

        UserDataManager.Instance.ChangeMoneyAction -= OnMoneyChanged;
        UserDataManager.Instance.ChangeMoneyAction += OnMoneyChanged;
        UserDataManager.Instance.ChangeRankPointAction -= OnRankpointChanged;
        UserDataManager.Instance.ChangeRankPointAction += OnRankpointChanged;
    }
    

    private void OnEnable()
    {
        UpdateCards();
    }

    private void Update()
    {
        if(Application.platform == RuntimePlatform.Android)
        {
            if(Input.GetKey(KeyCode.Escape))
            {
                ServiceLocator.Instance.GetSceneService<GameManager>().uiManager.OnClickButtonExitInteriorUI();
            }
        }
    }
    
    

    private void InitCardGroups(List<InteriorData> dataList, ObjectArea area)
    {
        var targetList = area == ObjectArea.Hall ? hallCardGroups : kitchenCardGroups;
        var dict = dataList
            .Where(x => x.CookwareType == area)
            .GroupBy(x => x.UICategory)
            .ToDictionary(group => group.Key, group => group.ToList());
        foreach (var pair in dict)
        {
            var result = interiorCardGroupPrefab.InstantiateAsync(cardGroupParent).WaitForCompletion();
            var interiorGroup = result.GetComponent<InteriorCardGroup>();
            interiorGroup.InitializeGroup(pair.Value, pair.Key, popup, authorityPopup);
            targetList.Add(interiorGroup);
        }
    }

    private void InitButtonEvent()
    {
        hallButton.onClick.RemoveAllListeners();
        kitchenButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();
        hallButton.onClick.AddListener(() => ToggleCardGroups(hallCardGroups, kitchenCardGroups));
        kitchenButton.onClick.AddListener(() => ToggleCardGroups(kitchenCardGroups, hallCardGroups));
        closeButton.onClick.AddListener(OnClose);
        ToggleCardGroups(hallCardGroups, kitchenCardGroups);
    }

    private void OnMoneyChanged(int? gold)
    {
        UpdateCards();
        UpdateTexts();
    }

    private void UpdateTexts()
    {
        moneyText.text = UserDataManager.Instance.CurrentUserData.Money.ToString();
        goldText.text = UserDataManager.Instance.CurrentUserData.Gold.ToString();
    }

    private void OnRankpointChanged(int rankpoint)
    {
        UpdateCards();
    }

    private void UpdateCards()
    {
        foreach (var group in hallCardGroups)
        {
            group.UpdateCards();
        }
        foreach (var group in kitchenCardGroups)
        {
            group.UpdateCards();
        }
    }

    private void ToggleCardGroups(List<InteriorCardGroup> activeGroups, List<InteriorCardGroup> inactiveGroups)
    {
        foreach (var group in activeGroups)
        {
            group.gameObject.SetActive(true);
        }

        foreach (var group in inactiveGroups)
        {
            group.gameObject.SetActive(false);
        }
    }

    private void OnClose()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (UserDataManager.Instance != null)
        {
            UserDataManager.Instance.ChangeMoneyAction -= OnMoneyChanged;
            UserDataManager.Instance.ChangeRankPointAction -= OnRankpointChanged;
        }
    }
}