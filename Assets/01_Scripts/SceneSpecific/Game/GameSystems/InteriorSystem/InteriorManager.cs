using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
                workStationManager.UpdateTable(item, level);
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
                workStationManager.UpdateCookingStation(interiorTable.GetData(item.InteriorID), upgradeLevel);
            }
        }
    }
    
    private void OnInteriorUpgrade(int interiorId, int level)
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
                workStationManager.UpdateCookingStation(interiorTable.GetData(interiorId), level);
            }

            return;
        }
        
        bool isCurrentThemeTable = interiorTable
            .Where(x => x.RestaurantType == (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme)
            .Any(x => x.InteriorID == interiorId && x.Category == InteriorCategory.테이블);
        if (isCurrentThemeTable)
        {
            if (level == 1)
            {
                workStationManager.AddTable(interiorId % 10);
            }
            else
            {
                workStationManager.UpdateTable(interiorTable.GetData(interiorId), level);
            }

            return;
        }
    }

}
