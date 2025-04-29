using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MyRestaurantInfoUI : MonoBehaviour
{
    private UserData currentUserData;

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

    [VInspector.Foldout("BestMenu")]
    [SerializeField] private TextMeshProUGUI bestMenuTitle;
    [SerializeField] private List<TextMeshProUGUI> bestFoodSellingCounts = new();
    [VInspector.EndFoldout]

    #region Keys
    private string themeNameKey = "ThemeName";
    private string totalReviewTitleKey = "TotalReviewTitle";
    private string totalReviewCountKey = "TotlaReviewCount";
    private string currentRankingTitleKey = "CurrentRankingTitle";
    private string currentThemeRankingKey = "CurrentThemeRanking";
    private string currentGlobalRankingKey = "CurrentGlobalRanking";
    private string bestMenuTitleKey = "BestMenuTitle";
    private string bestMenuCountKey = "BestMenuCount";
    #endregion

    #region Format
    private string reviewCountFormat = "X {0}";
    #endregion

    private void OnEnable()
    {
        UpdateUI();
    }

    private void Start()
    {
        currentUserData = UserDataManager.Instance.CurrentUserData;
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

    }

    private void UpdateBestFoodRanking()
    {

    }

    private void UpdateCurrentRanking()
    {
        UpdateCurrentThemeRanking();
        UpdateCurrentGlobalRanking();
    }
    private void UpdateCurrentThemeRanking()
    {

    }

    private void UpdateCurrentGlobalRanking()
    {

    }

    private void UpdateTotalReviewCount()
    {
        UpdateTotalPositiveReviewCount();
        UpdateTotalNegativeReviewCount();
    }

    private void UpdateTotalPositiveReviewCount()
    {

    }
    private void UpdateTotalNegativeReviewCount()
    {

    }

    private void UpdateThemeName()
    {

    }
}
