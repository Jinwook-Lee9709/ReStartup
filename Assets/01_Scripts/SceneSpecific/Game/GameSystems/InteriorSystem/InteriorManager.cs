using System;
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
        ReferenceDataTable();
        InitTable();
        InitCookwares();
        InitSinkingStation();
        InitCounter();
        InitInterior();
        InitDecor();
    }

    public void ReferenceDataTable()
    {
        interiorTable = DataTableManager.Get<InteriorDataTable>(DataTableIds.Interior.ToString());
        cookwareTable = DataTableManager.Get<CookwareDataTable>(DataTableIds.Cookware.ToString());
    }

    public void InitTable()
    {
        var tableQuery = interiorTable.Where(x =>
            x.RestaurantType == (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme &&
            x.Category == InteriorCategory.Table);
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
            x.RestaurantType == (int)gameManager.CurrentTheme && x.Category == InteriorCategory.Sink);
        UpgradeSink(data, 1);
    }

    private void InitCounter()
    {
        workStationManager.AddCounter();
        var data = interiorTable.First(x =>
            x.RestaurantType == (int)gameManager.CurrentTheme && x.Category == InteriorCategory.Counter);
        UpgradeCounter(data, 1);
    }

    private void InitInterior()
    {
        var interiorUpgradeDictionary = UserDataManager.Instance.CurrentUserData.InteriorSaveData;
        var interiorQuery = interiorTable.Where(x =>
                x.RestaurantType == (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme)
            .Where(x =>
                x.Category == InteriorCategory.Interior ||
                x.Category == InteriorCategory.Wallpaper ||
                x.Category == InteriorCategory.Floor);
        foreach (var data in interiorQuery)
        {
            if (interiorUpgradeDictionary[data.InteriorID].Equals(0))
                continue;
            if (data.Category == InteriorCategory.Wallpaper)
            {
                AddWallpaper(data);
            }
            else if (data.Category == InteriorCategory.Floor)
            {
                AddFloor(data);
            }
            else
            {
                AddInterior(data);
            }
        }
    }

    private void InitDecor()
    {
        var parent = gameManager.ObjectPivotManager.GetDecorPivot();
        foreach (Transform child in parent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        var assetId = String.Format(Strings.DecorTileIdFormat, (int)gameManager.CurrentTheme);
        var objHandle = Addressables.InstantiateAsync(assetId, parent);
        objHandle.WaitForCompletion();
    }

    private void AddInterior(InteriorData data)
    {
        //ForTest
        if (data.IconID == "Dummy") return;
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

    private void AddWallpaper(InteriorData data)
    {
        Transform pivot;
        String assetId;
        pivot = gameManager.ObjectPivotManager.GetWallPivot(data.CookwareType);
        if (data.CookwareType == ObjectArea.Hall)
        {
            assetId = String.Format(Strings.HallWallTileIdFormat, data.RestaurantType, 1);
        }
        else
        {
            assetId = String.Format(Strings.KitchenWallTileIdFormat, data.RestaurantType, 1);
        }

        foreach (Transform child in pivot.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        var objHandle = Addressables.InstantiateAsync(assetId, pivot);
        objHandle.WaitForCompletion();
    }

    private void AddFloor(InteriorData data)
    {
        Transform pivot;
        String assetId;
        pivot = gameManager.ObjectPivotManager.GetFloorPivot(data.CookwareType);
        if (data.CookwareType == ObjectArea.Hall)
        {
            assetId = String.Format(Strings.HallFloorTileIdFormat, data.RestaurantType, 1);
        }
        else
        {
            assetId = String.Format(Strings.KitchenFloorTileIdFormat, data.RestaurantType, 1);
        }

        foreach (Transform child in pivot.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        var objHandle = Addressables.InstantiateAsync(assetId, pivot);
        objHandle.WaitForCompletion();
    }

    private void OnInteriorUpgrade(int interiorId, int level)
    {
        var interiorData = interiorTable.GetData(interiorId);
        UserDataManager.Instance.AddRankPointWithSave(interiorData.Reward).Forget();
        switch (interiorData.Category)
        {
            case InteriorCategory.Table:
                UpgradeTable(interiorData, level);
                break;
            case InteriorCategory.Counter:
                UpgradeCounter(interiorData, level);
                break;
            case InteriorCategory.Interior:
                UpgradeInterior(interiorData, level);
                break;
            case InteriorCategory.Cookware:
                UpgradeCookware(interiorId, level);
                break;
            case InteriorCategory.Sink:
                UpgradeSink(interiorData, level);
                break;
            case InteriorCategory.Wallpaper:
                break;
            case InteriorCategory.Floor:
                break;
        }
    }

    private void UpgradeTable(InteriorData interiorData, int level)
    {
        var isCurrentThemeTable = interiorTable
            .Where(x => x.RestaurantType == (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme)
            .Any(x => x.InteriorID == interiorData.InteriorID && x.Category == InteriorCategory.Table);
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