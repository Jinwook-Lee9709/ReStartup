using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class IngameGoodsUi : MonoBehaviour
{
    public static readonly string GoldFormatId = "GoldFormat";

    public TextMeshProUGUI goldText;
    public TextMeshProUGUI upgradeUIMoeny;
    public TextMeshProUGUI upgradeUIRankingPoint;
    private UserDataManager userDataManager;

    private void Start()
    {
        userDataManager = UserDataManager.Instance;
        GoldUiValueSet(userDataManager.CurrentUserData.Gold);
        upgradeUIMoeny.text = userDataManager.CurrentUserData.Gold.ToString();
        upgradeUIRankingPoint.text = userDataManager.CurrentUserData.CurrentRankPoint.ToString();
        userDataManager.ChangeGoldAction += GoldUiValueSet;
        userDataManager.ChangeRankPointAction += RankPointValueSet;

        LocalizationSettings.SelectedLocaleChanged += OnLanguageChanged;
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        LocalizationSettings.SelectedLocaleChanged -= OnLanguageChanged;

        // 유저 데이터 이벤트도 해제
        userDataManager.ChangeGoldAction -= GoldUiValueSet;
        userDataManager.ChangeRankPointAction -= RankPointValueSet;
    }

    public void GoldUiValueSet(int? gold)
    {
        var goldString = LZString.GetUIString(GoldFormatId, args: gold.ToString());
        goldText.text = goldString;
        upgradeUIMoeny.text = goldString;
    }

    public void RankPointValueSet(int? rankPoint)
    {
        upgradeUIRankingPoint.text = rankPoint.ToString();
    }

    public void SetGoldUi()
    {
        GoldUiValueSet(userDataManager.CurrentUserData.Gold);
    }

    private void OnLanguageChanged(Locale locale)
    {
        SetGoldUi();
    }
}