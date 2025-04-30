using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VInspector;

public class SupervisorListPanel : MonoBehaviour
{
    private static readonly string ManagerStringKey = "ManagerFormat";
    [SerializeField] private AssetReference supervisorInfoPanel;
    [SerializeField] private Transform supervisorInfoParent;
    [SerializeField] private SerializedDictionary<int, int> costMultiplier = new() { { 1, 1 }, { 2, 3 }, { 3, 4 } };
    private List<SupervisorInfoCard> cards = new();

    private int cursorThemeID = 1;

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
            card.Init(info, (number) => { action(cursorThemeID, number); });
            cards.Add(card);
        }

        UserDataManager.Instance.ChangeMoneyAction += OnMoneyChanged;
        UserDataManager.Instance.OnRankChangedEvent += OnRankChanged;
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
    }

    private void ChangeToPrevRestaurantSupervisor(int themeID)
    {
        var themeStatus = UserDataManager.Instance.CurrentUserData.ThemeStatus[(ThemeIds)themeID];
        for (int i = 1; i <= cards.Count; i++)
        {
            var info = CreateDefaultInfo(themeID, i);
            info.isHired = i <= themeStatus.managerCount;
            info.isHireable = i == themeStatus.managerCount + 1;
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
            cards[i-1].ChangeInfo(info);
        }
    }

    private SupervisorInfo CreateDefaultInfo(int themeID, int number)
    {
        SupervisorInfo info = new();
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
                cards[currentManager].OnRankReached();
            }
        }
    }
}