using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IngameGoodsUi : MonoBehaviour
{
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI upgradeUIMoeny;
    public TextMeshProUGUI upgradeUIRankingPoint;
    private UserDataManager userDataManager;
    private void Start()
    {
        userDataManager = UserDataManager.Instance;
        goldText.text = $"GOLD : {userDataManager.CurrentUserData.Gold}";
        upgradeUIMoeny.text = userDataManager.CurrentUserData.Gold.ToString();
        upgradeUIRankingPoint.text = userDataManager.CurrentUserData.CurrentRankPoint.ToString();
        userDataManager.ChangeGoldAction += goldUiValueSet;
        userDataManager.ChangeRankPointAction += rankPointValueSet;
    }
    public void goldUiValueSet(int? gold)
    {
        Debug.Log(gold);
        goldText.text = $"GOLD : {gold}";
        upgradeUIMoeny.text = gold.ToString();
    }
    public void rankPointValueSet(int? rankPoint)
    {
        upgradeUIRankingPoint.text = rankPoint.ToString();
    }
    public void SetGoldUi()
    {
        goldText.text = $"GOLD : {userDataManager.CurrentUserData.Gold}";
    }
}
