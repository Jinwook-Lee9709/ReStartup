using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

public class GuideUI : MonoBehaviour
{
    private readonly string guideCategoryController = "GuideCategory";
    
    [SerializeField] private Transform content;
    
    
    private Dictionary<int, GuideCategoryController> categoryControllers = new();

    public void Start()
    {
        Init();
    }
    
    public void Init()
    {
        var guideCategoryDataTable = DataTableManager.Get<GuideCategoryDataTable>(DataTableIds.GuideCategory.ToString());
        foreach (var data in guideCategoryDataTable)
        {
            var obj = Addressables.InstantiateAsync(guideCategoryController, content).WaitForCompletion();
            var categoryButton = obj.GetComponent<GuideCategoryController>();
            categoryButton.Init(data, OnClickCategoryButton, OnClickElementButton);
            categoryControllers.Add(data.CategoryID, categoryButton);
        }
    }

    private void OnClickCategoryButton(int categoryID)
    {
        if (categoryControllers[categoryID].IsOpen)
        {
            categoryControllers[categoryID].CloseElementPanel();
            return;
        }

        foreach (var categoryController in categoryControllers.Values)
        {
            if (categoryController.IsOpen)
            {
                categoryController.CloseElementPanel();
            }
        }
        categoryControllers[categoryID].OpenElementPanel();
    }

    private void OnClickElementButton(int elementID)
    {
        
    }
}
