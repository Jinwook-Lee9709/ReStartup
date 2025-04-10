using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectSpawnTest : MonoBehaviour
{
    [SerializeField] private Transform contents;
    [SerializeField] private ObjectSpawnTestButton prefab;

    private void Start()
    {
        var pivotManager = ServiceLocator.Instance.GetSceneService<GameManager>().ObjectPivotManager;
        var pivotList = pivotManager.GetTablePivots();
        for (var i = 0; i < pivotList.Count; i++)
        {
            var obj = Instantiate(prefab, contents);
            obj.gameObject.name = $"Table{i + 1}";
            obj.transform.SetAsLastSibling();
            obj.text.text = $"Table{i + 1}";
            var currentTableNum = i;
            obj.button.onClick.AddListener(
                () =>
                {
                    ServiceLocator.Instance.GetSceneService<GameManager>().WorkStationManager.AddTable(currentTableNum);
                    obj.button.interactable = false;
                });
        }

        var table = DataTableManager.Get<FoodDataTable>(DataTableIds.Food.ToString());
        var gameManager = ServiceLocator.Instance.GetSceneService<GameManager>();
        var kvpList = table.GetSceneFoodDataList(gameManager.CurrentTheme);
        var cookwareTypeList = kvpList.Select(pair => pair.Value.CookwareType).Distinct().ToList();
        Dictionary<CookwareType, List<Transform>> cookwareDict = new();
        foreach (var cookwareType in cookwareTypeList)
        {
            var list = pivotManager.GetCookwarePivots(cookwareType);
            cookwareDict.Add(cookwareType, list);
        }

        foreach (var pair in cookwareDict)
            for (var i = 0; i < pair.Value.Count; i++)
            {
                var obj = Instantiate(prefab, contents);
                obj.gameObject.name = $"{pair.Key}{i + 1}";
                obj.transform.SetAsLastSibling();
                obj.text.text = $"{pair.Key}{i + 1}";
                var currentWareNum = i;
                obj.button.onClick.AddListener(
                    () =>
                    {
                        ServiceLocator.Instance.GetSceneService<GameManager>().WorkStationManager
                            .AddCookingStation(pair.Key, currentWareNum);
                        obj.button.interactable = false;
                    });
            }
    }
}