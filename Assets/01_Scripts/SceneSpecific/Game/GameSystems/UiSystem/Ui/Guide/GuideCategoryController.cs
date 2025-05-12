using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

public class GuideCategoryController : MonoBehaviour
{
    public static readonly string elementName = "GuideElementButton";
    
    [SerializeField] private GuideCategoryButton categoryButton;
    [SerializeField] private Transform elementParent;

    public bool IsOpen => elementParent.gameObject.activeSelf;

    public void Init(GuideCategoryData data, UnityAction<int> onCategoryButtonClicked,
        UnityAction<int> onElementButtonClicked)
    {
        categoryButton.Init(data, onCategoryButtonClicked);

        var guideElementTable = DataTableManager.Get<GuideElementDataTable>(DataTableIds.GuideElement.ToString());
        var query = guideElementTable.Where(x => x.CategoryId == data.CategoryId);

        foreach (var elementData in query)
        {
            var button = Addressables.InstantiateAsync(elementName, elementParent).WaitForCompletion()
                .GetComponent<GuideElementButton>();
            button.Init(elementData, onElementButtonClicked);
        }
    }

    public void OpenElementPanel()
    {
        elementParent.gameObject.SetActive(true);
    }

    public void CloseElementPanel()
    {
        elementParent.gameObject.SetActive(false);
    }
}
