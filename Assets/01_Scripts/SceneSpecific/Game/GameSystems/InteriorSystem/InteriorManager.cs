using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class InteriorManager
{
    private GameManager gameManager;
    private WorkStationManager workStationManager;
    private InteriorDataTable interiorTable;
    private CookwareDataTable cookwareTable;

    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
        this.workStationManager = gameManager.WorkStationManager;
        UserDataManager.Instance.OnInteriorUpgradeEvent += OnInteriorUpgrade;
    }

    public void Start()
    {
        InitTable();
        InitCookwares();
        InitSinkingStation();
    }
    
    public void InitTable()
    {
        interiorTable = DataTableManager.Get<InteriorDataTable>(DataTableIds.Interior.ToString());
        cookwareTable = DataTableManager.Get<CookwareDataTable>(DataTableIds.Cookware.ToString());
        var tableQuery = interiorTable.Where(x => x.RestaurantType == (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme && x.Category == InteriorCategory.테이블);
        var interiorUpgradeDictionary = UserDataManager.Instance.CurrentUserData.InteriorSaveData;
        foreach (var item in tableQuery)
        {
            interiorUpgradeDictionary.TryGetValue(item.InteriorID, out int level);
            if (level != 0)
            {
                workStationManager.AddTable(item.InteriorID % 10);
                workStationManager.UpgradeTable(item, level);
            }
        }
    }
    
    public void InitCookwares()
    {
        var interiorUpgradeDictionary = UserDataManager.Instance.CurrentUserData.InteriorSaveData;
        var cookwareQuery = cookwareTable.Where(x => x.RestaurantType == (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme);
        
        foreach (var item in cookwareQuery)
        {
            int upgradeLevel = interiorUpgradeDictionary[item.InteriorID];
            if (upgradeLevel != 0)
            {
                workStationManager.AddCookingStation(item.CookwareType, item.CookwareNB);
                workStationManager.UpgradeCookingStation(interiorTable.GetData(item.InteriorID), upgradeLevel);
            }
        }
    }
    
    private void InitSinkingStation()
    {
        workStationManager.AddSinkingStation();
    }
    
    private void OnInteriorUpgrade(int interiorId, int level)
    {
        var interiorData = interiorTable.GetData(interiorId);
        UserDataManager.Instance.OnRankPointUp(interiorData.Reward);
        switch (interiorData.Category)
        {
            case InteriorCategory.테이블:
                UpgradeTable(interiorData, level);
                break;
            case InteriorCategory.카운터:
                // Add implementation for 카운터 case
                break;
            case InteriorCategory.인테리어:
                UpgradeInterior(interiorData, level);
                break;
            case InteriorCategory.조리대:
                UpgradeCookware(interiorId, level);
                break;
            case InteriorCategory.주방가구:
                // Add implementation for 주방가구 case
                break;
            case InteriorCategory.싱크대:
                UpgradeSink(interiorData, level);
                break;
            default:
                // Add implementation for default case if necessary
                break;
        }
        
        
     
    }

    private void UpgradeTable(InteriorData interiorData, int level)
    {
        bool isCurrentThemeTable = interiorTable
            .Where(x => x.RestaurantType == (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme)
            .Any(x => x.InteriorID == interiorData.InteriorID && x.Category == InteriorCategory.테이블);
        if (isCurrentThemeTable)
        {
            if (level == 1)
            {
                workStationManager.AddTable(interiorData.InteriorID % 10);
            }
            else
            {
                workStationManager.UpgradeTable(interiorData, level);
            }
        }
    }

    private void UpgradeInterior(InteriorData interiorData, int level)
    {
        if (level == 1)
        {
          
        }
        else
        {
            UserDataManager.Instance.OnRankPointUp(interiorData.EffectQuantity);
        }
    }
    

    private void UpgradeCookware(int interiorId, int level)
    {
        bool isCurrentThemeCookware = cookwareTable
            .Where(x => x.RestaurantType == (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme)
            .Any(x => x.InteriorID == interiorId);

        if (isCurrentThemeCookware)
        {
            if (level == 1)
            {
                var data = cookwareTable.GetData(interiorId);
                workStationManager.AddCookingStation(data.CookwareType, data.CookwareNB);
            }
            else
            {
                workStationManager.UpgradeCookingStation(interiorTable.GetData(interiorId), level);
            }
        }
    }

    private void UpgradeSink(InteriorData interiorData, int level)
    {
        workStationManager.UpgradeSinkingStation(interiorData, level);
    }
    

}
