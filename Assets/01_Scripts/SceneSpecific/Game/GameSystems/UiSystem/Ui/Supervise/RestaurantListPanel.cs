using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class RestaurantListPanel : MonoBehaviour
{
    [SerializeField] private AssetReference restaurantInfoCard;
    [SerializeField] private Transform parent;

    public void Init(Action<int> action)
    {
        foreach (var id in Enum.GetValues(typeof(ThemeIds)))
        {
            int number = (int)id;
            var obj = Addressables.InstantiateAsync(restaurantInfoCard, parent).WaitForCompletion();
            var sprite = Addressables.LoadAssetAsync<Sprite>("RestaurantSprite1" + (int)id).WaitForCompletion();
            var card = obj.GetComponent<RestaurantInfoCard>();
            card.SetInfo("베이커리", 1000000, sprite);
            card.RegisterAction(
                () => { action(number); }
            );
        }
        
    }
}
