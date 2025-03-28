using System.Linq;
using UnityEngine;
using Random = System.Random;

public class FoodUpgradeDataManager
{
    public void Init()
    {
        var saveData = UserDataManager.Instance.CurrentUserData.FoodSaveData;
    }

    public void OnFoodUnlock(int foodId)
    {
        var foodSaveData = UserDataManager.Instance.CurrentUserData.FoodSaveData;

        if (foodSaveData.ContainsKey(foodId))
        {
            Debug.LogError("Already Unlocked");
            return;
        }

        foodSaveData.Add(foodId, new FoodSaveData());
    }

    public void OnFoodUpgrade(int foodId)
    {
        var foodSaveData = UserDataManager.Instance.CurrentUserData.FoodSaveData;

        if (!foodSaveData.ContainsKey(foodId)) Debug.LogError("Food not found");
        if (foodSaveData[foodId].UpgradeLevel < Constants.MAX_UPGRADE_LEVEL)
            foodSaveData[foodId].UpgradeLevel++;
    }

    public int GetRandomFood(int foodId)
    {
        var foodSaveData = UserDataManager.Instance.CurrentUserData.FoodSaveData;

        if (foodSaveData.ContainsKey(foodId)) return 0;

        Random random = new();
        var randomKey = foodSaveData.Keys.ElementAt(random.Next(foodSaveData.Count));
        return randomKey;
    }
}