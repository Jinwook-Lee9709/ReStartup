using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using VInspector;

public class SupervisorListPanel : MonoBehaviour
{
    private static readonly string ManagerStringKey = "ManagerFormat";
    private static readonly string SupervisorIconStringFormat = "ManagerIcon{0}";
    [SerializeField] private AssetReference supervisorInfoPanel;
    [SerializeField] private Transform supervisorInfoParent;
    [SerializeField] private SerializedDictionary<int, int> costMultiplier = new() { { 1, 1 }, { 2, 3 }, { 3, 4 } };
    private SupervisorCompensationSO compensation; 
    private List<SupervisorInfoCard> cards = new();

    [SerializeField] private Button claimButton;
    [SerializeField] private TextMeshProUGUI compensationText;

    private int cursorThemeID = 1;

    private void Awake()
    {
        compensation = Addressables.LoadAssetAsync<SupervisorCompensationSO>(Strings.CompensationSoKey).WaitForCompletion();
    }

    private void Start()
    {
        claimButton.onClick.RemoveAllListeners();
        claimButton.onClick.AddListener(OnClaim);
    }

    private void OnEnable()
    {
        UpdateClaimButton();
    }

    private void UpdateClaimButton(bool animate = false)
    {
        bool canClaim = CanClaim(out StageStatusData themeStatus);
        if (!canClaim)
        {
            claimButton.interactable = false;
            if (animate)
            {
                claimButton.transform.PopdownAnimation();
            }
            else
            {
                claimButton.gameObject.SetActive(false);    
            }
            return;
        }

        var calculatedCompensation = CalculateCompensation(themeStatus);
        compensationText.text = CalculateCompensation(themeStatus).ToString();
        claimButton.interactable = calculatedCompensation != 0;
        
        if (animate)
        {
            claimButton.transform.PopupAnimation();
        }
        else
        {
            claimButton.gameObject.SetActive(true);
        }
    }
    
    private bool CanClaim(out StageStatusData themeStatus)
    {
        if (cursorThemeID > (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme)
        {
            themeStatus = null;
            return false; 
        }
        UserDataManager.Instance.CurrentUserData.ThemeStatus.TryGetValue((ThemeIds)cursorThemeID, out var status);
        themeStatus = status;
        if (themeStatus == null || themeStatus.managerCount == 0)
        {
            return false;
        }

        return true;
    }

    private void OnClaim()
    {
        if (cursorThemeID > (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme)
            return;
        var themeStatus  = UserDataManager.Instance.CurrentUserData.ThemeStatus[(ThemeIds)cursorThemeID];
        if (themeStatus.managerCount == 0)
            return;
        var compensationAmount = CalculateCompensation(themeStatus);
        themeStatus.lastClaim = DateTime.Now;
        UserDataManager.Instance.AdjustMoneyWithSave(compensationAmount).Forget();
        StageStatusDataDAC.UpdateStageStatusData(themeStatus).Forget();
        UpdateClaimButton(true);
    }

    private int CalculateCompensation(StageStatusData themeStatus)
    {
        var currentTheme = (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme;
        DateTime lastClaimTime = themeStatus.lastClaim;
        DateTime now = DateTime.Now;
        var interval = now - lastClaimTime;
        var clampedInterval = Math.Clamp((int)interval.TotalHours, 0, 24);
        int multiplier = compensation.multipliers[themeStatus.managerCount];
        int finalCompensation = compensation.baseCompensation[currentTheme] * clampedInterval / 24 * multiplier;
        return finalCompensation;
    }

    public void Init(Action<int, int> action)
    {
        cursorThemeID = (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme;

        var themeStatus = UserDataManager.Instance.CurrentUserData.ThemeStatus[(ThemeIds)cursorThemeID];
        for (int i = 1; i <= Constants.MAX_SUPERVISOR_COUNT; i++)
        {
            SupervisorInfoCard card = Addressables.InstantiateAsync(supervisorInfoPanel, supervisorInfoParent).WaitForCompletion()
                .GetComponent<SupervisorInfoCard>();
            var info = CreateDefaultInfo(cursorThemeID, i);
            info.isHired = i <= themeStatus.managerCount;
            info.isHireable = i == themeStatus.managerCount + 1 &&
                              UserDataManager.Instance.CurrentUserData.CurrentRank >=
                              Constants.SUPERVISOR_HIRE_REQUIREMENTS;
            info.icon = Addressables.LoadAssetAsync<Sprite>(String.Format(SupervisorIconStringFormat, i)).WaitForCompletion();
            card.Init(info, (number) => { action(cursorThemeID, number); });
            cards.Add(card);
        }

        UserDataManager.Instance.ChangeMoneyAction += OnMoneyChanged;
        UserDataManager.Instance.OnRankChangedEvent += OnRankChanged;
        
        claimButton.onClick.RemoveAllListeners();
        claimButton.onClick.AddListener(OnClaim);
    }

    private void OnDestroy()
    {
        if (UserDataManager.Instance != null)
        {
            UserDataManager.Instance.ChangeMoneyAction -= OnMoneyChanged;
            UserDataManager.Instance.OnRankChangedEvent -= OnRankChanged;
        }

    }

    public void OnSupervisorHire()
    {
        int currentManager = UserDataManager.Instance.CurrentUserData.ThemeStatus[(ThemeIds)cursorThemeID].managerCount;
        if (currentManager < Constants.MAX_SUPERVISOR_COUNT)
        {
            cards[currentManager].ChangeToHireable();
        }
    }

    public void ChangeSupervisor(int themeID)
    {
        cursorThemeID = themeID;
        var currentTheme = (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme;
        switch (cursorThemeID)
        {
            case var theme when theme < currentTheme:
                ChangeToPrevRestaurantSupervisor(themeID);
                break;
            case var theme when theme == currentTheme:
                ChangeToCurrentRestaurantSupervisor(themeID);
                break;
            case var theme when theme > currentTheme:
                ChangeToPostRestaurantSupervisor(themeID);
                break;
        }
        
        UpdateClaimButton(true);
    }
    
    private void ChangeToPrevRestaurantSupervisor(int themeID)
    {
        var themeStatus = UserDataManager.Instance.CurrentUserData.ThemeStatus[(ThemeIds)themeID];
        for (int i = 1; i <= cards.Count; i++)
        {
            var info = CreateDefaultInfo(themeID, i);
            info.isHired = i <= themeStatus.managerCount;
            info.isHireable = i == themeStatus.managerCount + 1;
            info.icon = Addressables.LoadAssetAsync<Sprite>(String.Format(SupervisorIconStringFormat, i)).WaitForCompletion();
            cards[i-1].ChangeInfo(info);
        }
    }

    private void ChangeToCurrentRestaurantSupervisor(int themeID)
    {
        var themeStatus = UserDataManager.Instance.CurrentUserData.ThemeStatus[(ThemeIds)themeID];
        for (int i = 1; i <= cards.Count; i++)
        {
            var info = CreateDefaultInfo(themeID, i);
            info.isHired = i <= themeStatus.managerCount;
            info.isHireable = i == themeStatus.managerCount + 1 &&
                              UserDataManager.Instance.CurrentUserData.CurrentRank >=
                              Constants.SUPERVISOR_HIRE_REQUIREMENTS;
            info.icon = Addressables.LoadAssetAsync<Sprite>(String.Format(SupervisorIconStringFormat, i)).WaitForCompletion();
            cards[i-1].ChangeInfo(info);
        }
    }


    private void ChangeToPostRestaurantSupervisor(int themeID)
    {
        for (int i = 1; i <= cards.Count; i++)
        {
            var info = CreateDefaultInfo(themeID, i);
            info.isHired = false;
            info.isHireable = false;
            info.icon = Addressables.LoadAssetAsync<Sprite>(String.Format(SupervisorIconStringFormat, i)).WaitForCompletion();
            cards[i-1].ChangeInfo(info);
        }
    }

    private SupervisorInfo CreateDefaultInfo(int themeID, int number)
    {
        SupervisorInfo info = new();
        info.number = number;
        info.cost = SupervisorCost(number, themeID);
        info.name = LZString.GetUIString(ManagerStringKey, args: number.ToString());
        return info;
    }


    private int SupervisorCost(int number, int themeID)
    {
        var restaurantInfo = DataTableManager.Get<ThemeConditionDataTable>(DataTableIds.ThemeCondition.ToString())
            .GetConditionData(themeID);
        return restaurantInfo.Requirements2 * costMultiplier[number];
    }

    private void OnMoneyChanged(int? money)
    {
        foreach (var card in cards)
        {
            card.OnMoneyChanged(money ?? 0);
        }
    }

    private void OnRankChanged(int rank)
    {
        if (rank == Constants.SUPERVISOR_HIRE_REQUIREMENTS && cursorThemeID == (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme)
        {
            int currentManager = UserDataManager.Instance.CurrentUserData.ThemeStatus[(ThemeIds)cursorThemeID].managerCount;
            if (currentManager < Constants.MAX_SUPERVISOR_COUNT)
            {
                cards[currentManager].ChangeToHireable();
            }
        }
    }
}