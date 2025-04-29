using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class BestMenuInfo : MonoBehaviour
{
    [SerializeField] private Image foodImage;
    [SerializeField] private TextMeshProUGUI foodCount;
    private string bestMenuCountKey = "BestMenuCount";
    private string bestMenuCountFormat;

    public void Init()
    {
        bestMenuCountFormat = LZString.GetUIString(bestMenuCountKey);
    }


    public void UpdateFoodInfo(FoodSaveData food)
    {
        var foodData = DataTableManager.Get<FoodDataTable>(DataTableIds.Food.ToString()).GetFoodData(food.id);

        var spriteHandler = Addressables.LoadAssetAsync<Sprite>(foodData.IconID);
        spriteHandler.WaitForCompletion();
        foodImage.sprite = food.sellCount > 0 ? spriteHandler.Result : null;
        foodCount.text = string.Format(bestMenuCountFormat, food.sellCount > 0 ? food.sellCount : "-");
    }
}
