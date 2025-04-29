using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MyRestaurantInfoUI : MonoBehaviour
{
    private UserData currentUserData;
    private ThemeIds currentThemeId;

    [VInspector.Foldout("CurrentUserInfo")]
    [SerializeField] private TextMeshProUGUI userName;
    [SerializeField] private TextMeshProUGUI themeName;

    [VInspector.Foldout("Reviews")]
    [SerializeField] private TextMeshProUGUI totalReviewTitle;
    [SerializeField] private TextMeshProUGUI totalReviewCount;
    [SerializeField] private TextMeshProUGUI positiveReviewCount;
    [SerializeField] private TextMeshProUGUI negativeReviewCount;

    [VInspector.Foldout("Rankings")]
    [SerializeField] private TextMeshProUGUI rankingTitle;
    [SerializeField] private TextMeshProUGUI themeRanking;
    [SerializeField] private TextMeshProUGUI globalRanking;
    [SerializeField] private PlayerClone themePlayerRankingClone;

    [VInspector.Foldout("BestMenu")]
    [SerializeField] private TextMeshProUGUI bestMenuTitle;
    [SerializeField] private List<BestMenuInfo> bestFoodSellingInfos = new();
    [VInspector.EndFoldout]

    #region Keys
    private string totalReviewTitleKey = "TotalReviewTitle";
    private string totalReviewCountKey = "TotalReviewCount";
    private string currentRankingTitleKey = "CurrentRankingTitle";
    private string currentThemeRankingKey = "CurrentThemeRanking";
    private string currentGlobalRankingKey = "CurrentGlobalRanking";
    private string bestMenuTitleKey = "BestMenuTitle";
    #endregion

    #region Format
    private string themeNameKeyFormat = "RestaurantName{0}";
    private string reviewCountFormat = "X {0}";
    private string totalReviewCountFormat;
    #endregion

    private void OnEnable()
    {
        currentUserData = UserDataManager.Instance.CurrentUserData;
        currentThemeId = ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme;
        Init();
        UpdateUI();
    }

    private void UpdateUI()
    {
        UpdateTotalReviewCount();
        UpdateCurrentRanking();
        UpdateBestFoodRanking();
    }

    private void Init()
    {
        userName.text = currentUserData.Name;
        themeName.text = LZString.GetUIString(string.Format(themeNameKeyFormat, (int)currentThemeId));

        totalReviewTitle.text = LZString.GetUIString(totalReviewTitleKey);
        rankingTitle.text = LZString.GetUIString(currentRankingTitleKey);
        bestMenuTitle.text = LZString.GetUIString(bestMenuTitleKey);

        totalReviewCountFormat = LZString.GetUIString(totalReviewCountKey);

        foreach (var item in bestFoodSellingInfos)
            item.Init();
    }

    private void UpdateBestFoodRanking()
    {
        var bestFoods = (from food in currentUserData.FoodSaveData.Values
                        where food.theme == currentThemeId
                        orderby food.sellCount descending
                        select food).ToList();

        for (int i = 0; i < bestFoodSellingInfos.Count; i++)
        {
            bestFoodSellingInfos[i].UpdateFoodInfo(bestFoods[i]);
        }
    }

    private void UpdateCurrentRanking()
    {
        UpdateCurrentThemeRanking();
        UpdateCurrentGlobalRanking().Forget();
    }
    private void UpdateCurrentThemeRanking()
    {
        themeRanking.text = themePlayerRankingClone?.playerData.rank.ToString();
    }

    private async UniTask UpdateCurrentGlobalRanking()
    {
        var rank = await RankerDataDAC.GetUserRank();
        globalRanking.text = rank.Data.rank.ToString();
    }

    private void UpdateTotalReviewCount()
    {
        UpdateTotalPositiveReviewCount();
        UpdateTotalNegativeReviewCount();
        totalReviewCount.text = string.Format(totalReviewCountFormat, currentUserData.reviewCountForTheme[currentThemeId].positive + currentUserData.reviewCountForTheme[currentThemeId].negative);
    }

    private void UpdateTotalPositiveReviewCount()
    {
        positiveReviewCount.text = string.Format(reviewCountFormat, currentUserData.reviewCountForTheme[currentThemeId].positive);
    }
    private void UpdateTotalNegativeReviewCount()
    {
        negativeReviewCount.text = string.Format(reviewCountFormat, currentUserData.reviewCountForTheme[currentThemeId].negative);
    }
    
}
