using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Serialization;

public class IngameGoodsUi : MonoBehaviour
{
    public TextMeshProUGUI moneyText, goldText;
    public TextMeshProUGUI upgradeUIMoney;
    public TextMeshProUGUI upgradeUIGold;
    public TextMeshProUGUI rankPointText;
    public TextMeshProUGUI businessModelUIMoney, businessModelUIGold, businessModelUIAdTicket;
    private UserDataManager userDataManager;

    private void Start()
    {
        userDataManager = UserDataManager.Instance;
        MoneyUiValueSet(userDataManager.CurrentUserData.Money);
        upgradeUIMoney.text = userDataManager.CurrentUserData.Money.ToString();
        //upgradeUIRankingPoint.text = userDataManager.CurrentUserData.CurrentRankPoint.ToString();
        upgradeUIGold.text = userDataManager.CurrentUserData.Gold.ToString();
        rankPointText.text = userDataManager.CurrentUserData.CurrentRankPoint.ToString();
        userDataManager.ChangeMoneyAction += MoneyUiValueSet;
        userDataManager.ChangeGoldAction += GoldUiValueSet;
        userDataManager.ChangeRankPointAction += RankPointUiValueSet;
        userDataManager.ChangeAdTicketAction += AddTicketSet;
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
        userDataManager.ChangeRankPointAction -= RankPointUiValueSet;
    }

    public void MoneyUiValueSet(int? money)
    {
        moneyText.text = money.ToString();
        upgradeUIMoney.text = money.ToString();
        businessModelUIMoney.text = money.ToString();
    }
    public void GoldUiValueSet(int? gold)
    {
        goldText.text = gold.ToString();
        upgradeUIGold.text = gold.ToString();
        businessModelUIGold.text = gold.ToString();
    }
    public void AddTicketSet(int ticket)
    {
        businessModelUIAdTicket.text = ticket.ToString();
    }
    public void SetCostUi()
    {
        MoneyUiValueSet(userDataManager.CurrentUserData.Money);
        GoldUiValueSet(userDataManager.CurrentUserData.Gold);
    }

    public void RankPointUiValueSet(int rankPoint)
    {
        rankPointText.text = rankPoint.ToString();
    }

    private void OnLanguageChanged(Locale locale)
    {
        SetCostUi();
    }
}