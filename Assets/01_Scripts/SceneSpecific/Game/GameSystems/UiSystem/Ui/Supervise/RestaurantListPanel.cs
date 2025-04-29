using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VInspector;

public class RestaurantListPanel : MonoBehaviour
{
    private static readonly int CLEAR_RANK_CONDITION = 15;
    private static readonly string SPRITE_FORMAT = "RestaurantSprite{0}";
    private static readonly string NAME_FORMAT = "RestaurantName{0}";
    
    
    [SerializeField] private AssetReference restaurantInfoCard;
    [SerializeField] private Transform parent;
    private Dictionary<int, RestaurantInfoCard> cards = new();
    private int currentTheme;
    
    public void Init(Action action)
    {
        currentTheme = (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme;
        foreach (var id in Enum.GetValues(typeof(ThemeIds)))
        {
            InstantiateCardAndSetInfo(action, id);
        }

        UserDataManager.Instance.OnRankChangedEvent += OnRankChanged;
        UserDataManager.Instance.ChangeGoldAction += OnGoldChanged;

    }

    private void InstantiateCardAndSetInfo(Action action, object id)
    {
        int number = (int)id;
        var card = InstantiateCard();
        var sprite = LoadSprite(number);
        var shopName = GetShopName(number);
        var type = CalculateShopType(number);
        var cost = GetShopCost(number);
        
        card.SetInfo(shopName, cost, sprite, type);
        if(number == currentTheme)
            card.RegisterAction(
                () => { action(); }
            );
        cards.Add(number, card);
    }
    
    private RestaurantInfoCard InstantiateCard()
    {
        var obj = Addressables.InstantiateAsync(restaurantInfoCard, parent).WaitForCompletion();
        return obj.GetComponent<RestaurantInfoCard>();
    }
    
    private Sprite LoadSprite(int number)
    {
        return Addressables.LoadAssetAsync<Sprite>(string.Format(SPRITE_FORMAT, number)).WaitForCompletion();
    }
    
    private string GetShopName(int number)
    {
        string shopNameId = string.Format(NAME_FORMAT, number);
        return LZString.GetUIString(shopNameId);
    }

    private int GetShopCost(int number)
    {
        return DataTableManager.Get<ThemeConditionDataTable>(DataTableIds.ThemeCondition.ToString())
            .GetConditionData(number).Requirements1;
    }


    public void OnRankChanged(int rank)
    {
        UpdateInteractable();
    }

    public void OnGoldChanged(int? gold)
    {
        UpdateInteractable();
    }

    private void UpdateInteractable()
    {
        var interactable = CheckCondition();
        cards[(int)currentTheme].UpdateInteractable(interactable);
        
    }

    private bool CheckCondition()
    {
        bool isRankEnough = UserDataManager.Instance.CurrentUserData.CurrentRank >= CLEAR_RANK_CONDITION;
        bool isGoldEnough = UserDataManager.Instance.CurrentUserData.Gold >= 1000000;
        bool isManagerEnough = UserDataManager.Instance.CurrentUserData.ThemeStatus[(ThemeIds)currentTheme].managerCount > 0;
        return isRankEnough && isGoldEnough && isManagerEnough;
    }

    private RestaurantInfoCard.ShopType CalculateShopType(int themeId)
    {
        if (themeId < currentTheme) 
            return RestaurantInfoCard.ShopType.Previous;
        if (themeId == currentTheme)
            return RestaurantInfoCard.ShopType.Current;
        if (themeId == currentTheme + 1)
            return RestaurantInfoCard.ShopType.Next;
        return RestaurantInfoCard.ShopType.Post;
    }
}
