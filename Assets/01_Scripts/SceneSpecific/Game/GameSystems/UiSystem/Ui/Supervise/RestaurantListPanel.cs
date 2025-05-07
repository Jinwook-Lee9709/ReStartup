using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;


public class RestaurantListPanel : MonoBehaviour
{
    private static readonly int CLEAR_RANK_CONDITION = 15;
    private static readonly string SPRITE_FORMAT = "RestaurantSprite{0}";
    private static readonly string NAME_FORMAT = "RestaurantName{0}";
    
    [SerializeField] private AssetReference restaurantInfoCard;
    [SerializeField] private Transform parent;
    [SerializeField] private Button lButton, rButton;
    
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private float animationDuration = 0.3f;

    private List<Image> indicators;
    
    private RestaurantSuperviseUIManager uiManager;
    
    private Dictionary<int, RestaurantInfoCard> cards = new();
    private int currentTheme;
    
    public void Init(Action action, RestaurantSuperviseUIManager manager)
    {
        uiManager = manager;
        currentTheme = (int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme;
        foreach (var id in Enum.GetValues(typeof(ThemeIds)))
        {
            InstantiateCardAndSetInfo(action, id);
        }

        UserDataManager.Instance.OnRankChangedEvent += OnRankChanged;
        UserDataManager.Instance.ChangeMoneyAction += OnMoneyChanged;
        lButton.onClick.AddListener(() => OnButtonClick(false));
        rButton.onClick.AddListener(() => OnButtonClick(true));
        SetButtonActive();
    }

    private void OnDestroy()
    {
        UserDataManager.Instance.OnRankChangedEvent -= OnRankChanged;
        UserDataManager.Instance.ChangeMoneyAction -= OnMoneyChanged;
    }

    private void Start()
    {
        MovePanelToCenterTask().Forget();
    }

    private async UniTask MovePanelToCenterTask()
    {
        await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);
        MovePanelToCenter(currentTheme);
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
        if(number == currentTheme + 1)
            card.RegisterAction(
                () =>
                {
                    action();
                    Debug.Log("action");
                }
            );
        cards.Add(number, card);
    }

    private void OnButtonClick(bool isRight)
    {
        currentTheme += isRight ? 1 : -1;
        uiManager.ChangeSupervisorList(currentTheme);
        MovePanelToCenter(currentTheme);
        SetButtonActive();
    }


    private void MovePanelToCenter(int index)
    {
        Vector3 contentPosition = content.anchoredPosition;
        Vector3 itemPosition = cards[index].transform.localPosition;
        float contentWidth = content.rect.size.x;           
        float viewportWidth = scrollRect.viewport.rect.size.x;
        float targetX = itemPosition.x - (viewportWidth / 2);
        float minX = 0;
        float maxX = contentWidth - viewportWidth / 2;
        targetX = Mathf.Clamp(targetX, minX, maxX);
        Vector2 targetPosition = new Vector2(-targetX, contentPosition.y);
        DOTween.To(() => content.anchoredPosition, x => content.anchoredPosition = x, targetPosition, animationDuration)
            .SetEase(Ease.OutCubic); 

    }

    private void SetButtonActive()
    {
        lButton.gameObject.SetActive(currentTheme != 1);
        rButton.gameObject.SetActive(currentTheme != cards.Count);
    }
    private void OnRankChanged(int rank)
    {
        UpdateInteractable();
        cards[(int)ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme].SetLayout(RestaurantInfoCard.ShopType.Current);
    }

    public void OnMoneyChanged(int? money)
    {
        UpdateInteractable();
    }

    public void UpdateInteractable()
    {
        foreach (var card in cards.Values)
        {
            card.UpdateInteractable();
        }
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
