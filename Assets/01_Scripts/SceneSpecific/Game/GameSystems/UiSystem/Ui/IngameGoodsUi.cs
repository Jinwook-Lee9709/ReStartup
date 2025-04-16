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
        MoneyUiValueSet(userDataManager.CurrentUserData.Money);
        upgradeUIMoeny.text = userDataManager.CurrentUserData.Money.ToString();
        upgradeUIRankingPoint.text = userDataManager.CurrentUserData.CurrentRankPoint.ToString();
        userDataManager.ChangeMoneyAction += MoneyUiValueSet;
        userDataManager.ChangeRankPointAction += RankPointValueSet;

        LocalizationSettings.SelectedLocaleChanged += OnLanguageChanged;
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        LocalizationSettings.SelectedLocaleChanged -= OnLanguageChanged;

        // 유저 데이터 이벤트도 해제
        userDataManager.ChangeMoneyAction -= MoneyUiValueSet;
        userDataManager.ChangeRankPointAction -= RankPointValueSet;
    }

    public void MoneyUiValueSet(int? gold)
    {
        var goldString = LZString.GetUIString(GoldFormatId, args: gold.ToString());
        goldText.text = goldString;
        upgradeUIMoeny.text = goldString;
    }

    public void RankPointValueSet(int rankPoint)
    {
        upgradeUIRankingPoint.text = rankPoint.ToString();
    }

    public void SetGoldUi()
    {
        MoneyUiValueSet(userDataManager.CurrentUserData.Money);
    }

    private void OnLanguageChanged(Locale locale)
    {
        SetGoldUi();
    }
}