using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class InteriorManager
{
    private static readonly string FurnitureStringID = "FurnitureBase";
    
    private CookwareDataTable cookwareTable;
    private GameManager gameManager;
    private InteriorDataTable interiorTable;
    private WorkStationManager workStationManager;

    private Dictionary<int, FurnitureBase> furnitureList = new();
        
    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
        workStationManager = gameManager.WorkStationManager;
        UserDataManager.Instance.OnInteriorUpgradeEvent += OnInteriorUpgrade;
    }

    public void Start()
    {
        InitTable();
        InitCookwares();
        InitSinkingStation();
        InitCounter();
        InitInterior();
    }

    public void InitTable()
    {
        interiorTable = DataTableManager.Get<InteriorDataTable>(DataTableIds.Interior.ToString());
        cookwareTable = DataTableManager.Get<CookwareDataTable>(DataTableIds.Cookware.ToString());
        var tableQuery = interiorTable.Where(x =>
            x.RestaurantType == (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme &&
            x.Category == InteriorCategory.테이블);
        var interiorUpgradeDictionary = UserDataManager.Instance.CurrentUserData.InteriorSaveData;
        foreach (var item in tableQuery)
        {
            interiorUpgradeDictionary.TryGetValue(item.InteriorID, out var level);
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
        var cookwareQuery = cookwareTable.Where(x =>
            x.RestaurantType == (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme);

        foreach (var item in cookwareQuery)
        {
            var upgradeLevel = interiorUpgradeDictionary[item.InteriorID];
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
        var data = interiorTable.First(x =>
            x.RestaurantType == (int)gameManager.CurrentTheme && x.Category == InteriorCategory.싱크대);
        UpgradeSink(data, 1);
    }

    private void InitCounter()
    {
        workStationManager.AddCounter();
        var data = interiorTable.First(x =>
            x.RestaurantType == (int)gameManager.CurrentTheme && x.Category == InteriorCategory.카운터);
        UpgradeCounter(data, 1);
    }

    private void InitInterior()
    {
        var interiorUpgradeDictionary = UserDataManager.Instance.CurrentUserData.InteriorSaveData;
        var interiorQuery = interiorTable.Where(x =>
                x.RestaurantType == (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme)
            .Where(x => x.Category == InteriorCategory.인테리어);
        foreach (var data in interiorQuery)
        {
            if(interiorUpgradeDictionary[data.InteriorID].Equals(0)) 
                continue;
            AddInterior(data);
        }
    }

    private void AddInterior(InteriorData data)
    {
        //ForTest
        if(data.IconID == "Dummy") return;
        //ForTest
        var parent = gameManager.ObjectPivotManager.GetInteriorPivot(data.InteriorID);
        var spriteHandle = Addressables.LoadAssetAsync<Sprite>(data.IconID + 1);
        var objHandle = Addressables.InstantiateAsync(FurnitureStringID, parent);
        spriteHandle.WaitForCompletion();
        objHandle.WaitForCompletion();
        var sprite = spriteHandle.Result;
        var obj = objHandle.Result;
        var furnitureBase = obj.GetComponent<FurnitureBase>();
        furnitureList.Add(data.InteriorID, furnitureBase); 
        furnitureBase.ChangeSpirte(sprite);
    }

    private void OnInteriorUpgrade(int interiorId, int level)
    {
        var interiorData = interiorTable.GetData(interiorId);
        UserDataManager.Instance.AddRankPointWithSave(interiorData.Reward).Forget();
        switch (interiorData.Category)
        {
            case InteriorCategory.테이블:
                UpgradeTable(interiorData, level);
                break;
            case InteriorCategory.카운터:
                UpgradeCounter(interiorData, level);
                break;
            case InteriorCategory.인테리어:
                UpgradeInterior(interiorData, level);
                break;
            case InteriorCategory.조리대:
                UpgradeCookware(interiorId, level);
                break;
            case InteriorCategory.싱크대:
                UpgradeSink(interiorData, level);
                break;
        }
    }

    private void UpgradeTable(InteriorData interiorData, int level)
    {
        var isCurrentThemeTable = interiorTable
            .Where(x => x.RestaurantType == (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme)
            .Any(x => x.InteriorID == interiorData.InteriorID && x.Category == InteriorCategory.테이블);
        if (isCurrentThemeTable)
        {
            if (level == 1) workStationManager.AddTable(interiorData.InteriorID % 10);
            workStationManager.UpgradeTable(interiorData, level);
        }
    }

    private void UpgradeInterior(InteriorData interiorData, int level)
    {
        if (level != 1)
        {
            var sprite = Addressables.LoadAssetAsync<Sprite>(interiorData.IconID + level).WaitForCompletion();
            furnitureList[interiorData.InteriorID].ChangeSpirte(sprite);
        }
        else
        {
            AddInterior(interiorData);
        }
    }

    private void UpgradeCounter(InteriorData interiorData, int level)
    {
        workStationManager.UpgradeCounter(interiorData, level);
    }


    private void UpgradeCookware(int interiorId, int level)
    {
        var isCurrentThemeCookware = cookwareTable
            .Where(x => x.RestaurantType == (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme)
            .Any(x => x.InteriorID == interiorId);

        if (isCurrentThemeCookware)
        {
            if (level == 1)
            {
                var data = cookwareTable.GetData(interiorId);
                workStationManager.AddCookingStation(data.CookwareType, data.CookwareNB);
            }

            workStationManager.UpgradeCookingStation(interiorTable.GetData(interiorId), level);
        }
    }

    private void UpgradeSink(InteriorData interiorData, int level)
    {
        workStationManager.UpgradeSinkingStation(interiorData, level);
    }
}