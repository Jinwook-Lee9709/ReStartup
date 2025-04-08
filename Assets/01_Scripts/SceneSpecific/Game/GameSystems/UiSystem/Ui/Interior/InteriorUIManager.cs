using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class InteriorUIManager : MonoBehaviour
{
    [SerializeField] private List<InteriorCardGroup> hallCardGroups;
    [SerializeField] private List<InteriorCardGroup> kitchenCardGroups;

    [SerializeField] private Button hallButton;
    [SerializeField] private Button kitchenButton;
    
    [SerializeField] private AssetReference interiorCardGroupPrefab;

    private void Start()
    {
        List<InteriorData> hallList = new List<InteriorData>();
        List<InteriorData> kitchenList = new List<InteriorData>();
        
        var table = DataTableManager.Get<InteriorDataTable>(DataTableIds.Interior.ToString());
        var list = table
            .Where(x => x.RestaurantType == (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme)
            .ToList();

        foreach (var item in list)
        {
            if (item.CookwareType == ObjectArea.Hall)
            {
                hallList.Add(item);
            }
            else
            {
                kitchenList.Add(item);
            }
        }
    }

    private void OnBuy()
    {
    }

    private void InitButtonEvent()
    {
        kitchenButton.onClick.RemoveAllListeners();
        hallButton.onClick.RemoveAllListeners();
        kitchenButton.onClick.AddListener(() => ToggleCardGroups(kitchenCardGroups, hallCardGroups));
        hallButton.onClick.AddListener(() => ToggleCardGroups(hallCardGroups, kitchenCardGroups));
    }


    private void ToggleCardGroups(List<InteriorCardGroup> activeGroups, List<InteriorCardGroup> inactiveGroups)
    {
        foreach (var group in activeGroups)
        {
            group.gameObject.SetActive(true);
        }

        foreach (var group in inactiveGroups)
        {
            group.gameObject.SetActive(false);
        }
    }
}