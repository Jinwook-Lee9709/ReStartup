using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class EmployeeUIManager : MonoBehaviour
{
    public Transform contents;
    public AssetReference employeeListUi;
    private GameManager gameManager;
    [SerializeField] 

    public void Start()
    {
        gameManager = ServiceLocator.Instance.GetSceneService<GameManager>();
        var data = DataTableManager.Get<EmployeeDataTable>("Employee").Data;
        var dict = data
            .Where(x=>x.Value.Theme == (int)gameManager.CurrentTheme)
            .GroupBy(x=> x.Value.StaffType)
            .ToDictionary(group => group.Key, group => group.ToList());


        foreach (var pair in dict) //스탭 타입별 순회
        {
            var obj = Addressables.InstantiateAsync(employeeListUi, contents).WaitForCompletion();
            var line = obj.GetComponent<EmployeeUpgradeListUi>();
            line.SetWorkType((WorkType)pair.Key);
            foreach (var item in pair.Value)
            {
                line.AddEmployeeUpgradeItem(item.Value);
            }
        }
    }
}
