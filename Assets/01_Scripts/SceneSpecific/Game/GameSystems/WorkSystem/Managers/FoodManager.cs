using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodManager: MonoBehaviour
{
    public FoodUpgradeListUi upgradeListUi;
    public void Awake()
    {

    }
    public void Start()
    {
        var data = DataTableManager.Get<FoodDataTable>("Food").Data;
        foreach (var item in data.Values)
        {
            if (item.Type == (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme)
                upgradeListUi.AddFoodUpgradeItem(item);
        }
    }
}
