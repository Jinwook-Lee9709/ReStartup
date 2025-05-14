using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class BestMenuInfo : MonoBehaviour
{
    [SerializeField] private Image foodImage;
    [SerializeField] private TextMeshProUGUI foodCount;
    private string bestMenuCountKey = "BestMenuCount";
    private string faceKey = "GreyChefFace";
    private string bestMenuCountFormat;

    public void Init()
    {
        bestMenuCountFormat = LZString.GetUIString(bestMenuCountKey);
    }


    public void UpdateFoodInfo(FoodSaveData food)
    {
        var foodData = DataTableManager.Get<FoodDataTable>(DataTableIds.Food.ToString()).GetFoodData(food.id);

        var spriteHandler = Addressables.LoadAssetAsync<Sprite>(foodData.IconID);
        var spriteHandler2 = Addressables.LoadAssetAsync<Sprite>(faceKey);
        spriteHandler.WaitForCompletion();
        spriteHandler2.WaitForCompletion();
        foodImage.sprite = food.sellCount > 0 ? spriteHandler.Result : spriteHandler2.Result;
        foodCount.text = string.Format(bestMenuCountFormat, food.sellCount > 0 ? food.sellCount : "-");
    }
}
