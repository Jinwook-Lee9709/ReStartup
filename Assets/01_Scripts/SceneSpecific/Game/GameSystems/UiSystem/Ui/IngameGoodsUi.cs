using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class IngameGoodsUi : MonoBehaviour
{
    public TextMeshProUGUI moneyText, goldText;
    public TextMeshProUGUI upgradeUIMoeny, upgradeUIGold;
    private UserDataManager userDataManager;

    private void Start()
    {
        userDataManager = UserDataManager.Instance;
        MoneyUiValueSet(userDataManager.CurrentUserData.Money);
        upgradeUIMoeny.text = userDataManager.CurrentUserData.Money.ToString();
        //upgradeUIRankingPoint.text = userDataManager.CurrentUserData.CurrentRankPoint.ToString();
        upgradeUIGold.text = userDataManager.CurrentUserData.Gold.ToString();
        userDataManager.ChangeMoneyAction += MoneyUiValueSet;
        userDataManager.ChangeGoldAction += GoldUiValueSet;
        SetCostUi();
        LocalizationSettings.SelectedLocaleChanged += OnLanguageChanged;
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        LocalizationSettings.SelectedLocaleChanged -= OnLanguageChanged;

        // 유저 데이터 이벤트도 해제
        userDataManager.ChangeMoneyAction -= MoneyUiValueSet;
        userDataManager.ChangeGoldAction -= GoldUiValueSet;
    }

    public void MoneyUiValueSet(int? money)
    {
        moneyText.text = money.ToString();
        upgradeUIMoeny.text = money.ToString();
    }
    public void GoldUiValueSet(int? gold)
    {
        goldText.text = gold.ToString();
        upgradeUIGold.text = gold.ToString();
    }
    public void SetCostUi()
    {
        MoneyUiValueSet(userDataManager.CurrentUserData.Money);
        GoldUiValueSet(userDataManager.CurrentUserData.Gold);
    }

    private void OnLanguageChanged(Locale locale)
    {
        SetCostUi();
    }
}