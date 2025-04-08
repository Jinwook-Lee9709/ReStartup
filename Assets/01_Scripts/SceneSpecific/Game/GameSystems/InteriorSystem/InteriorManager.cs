using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteriorManager
{
    private GameManager gameManager;
    private WorkStationManager workStationManager;

    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
        this.workStationManager = gameManager.WorkStationManager;
        InitTable();
        InitCookwares();
        UserDataManager.Instance.OnInteriorUpgradeEvent += OnInteriorUpgrade;
    }
    
    public void InitTable()
    {
        var interiorTable = DataTableManager.Get<InteriorDataTable>(DataTableIds.Interior.ToString());
        var tableQuery = interiorTable.Where(x => x.RestaurantType == (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme && x.Category == InteriorCategory.테이블);
        var interiorUpgradeDictionary = UserDataManager.Instance.CurrentUserData.InteriorSaveData;
        foreach (var item in tableQuery)
        {
            interiorUpgradeDictionary.TryGetValue(item.InteriorID, out int level);
            if (level != 0)
            {
                workStationManager.AddTable(item.InteriorID % 10);
            }
        }
    }
    
    public void InitCookwares()
    {
        var cookwareTable = DataTableManager.Get<CookwareDataTable>(DataTableIds.Cookware.ToString());
        var interiorUpgradeDictionary = UserDataManager.Instance.CurrentUserData.InteriorSaveData;
        var cookwareQuery = cookwareTable.Where(x => x.RestaurantType == (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme);
        
        foreach (var item in cookwareQuery)
        {
            if (interiorUpgradeDictionary[item.InteriorID] != 0)
            {
                workStationManager.AddCookingStation(item.CookwareType, item.CookwareNB);
            }
        }
    }
    
    private void OnInteriorUpgrade(int interiorId, int level)
    {
        var cookwareTable = DataTableManager.Get<CookwareDataTable>(DataTableIds.Cookware.ToString());
        var interiorTable = DataTableManager.Get<InteriorDataTable>(DataTableIds.Interior.ToString());
        bool isCurrentThemeCookware = cookwareTable
            .Where(x => x.RestaurantType == (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme)
            .Any(x => x.InteriorID == interiorId);
        bool isCurrentThemeTable = interiorTable
            .Where(x => x.RestaurantType == (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme)
            .Any(x => x.InteriorID == interiorId && x.Category == InteriorCategory.테이블);
        if (isCurrentThemeCookware)
        {
            if (level == 1)
            {
                var data = cookwareTable.GetData(interiorId);
                workStationManager.AddCookingStation(data.CookwareType, data.CookwareNB);
            }
        }

        if (isCurrentThemeTable)
        {
            if (level == 1)
            {
                workStationManager.AddTable(interiorId % 10);
            }
        }
    }

}
