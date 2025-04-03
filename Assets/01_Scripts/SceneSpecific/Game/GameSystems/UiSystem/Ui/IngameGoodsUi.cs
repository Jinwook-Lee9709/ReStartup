using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IngameGoodsUi : MonoBehaviour
{
    public TextMeshProUGUI goldText;
    private UserDataManager userDataManager;
    private void Start()
    {
        userDataManager = UserDataManager.Instance;
        goldText.text = $"GOLD : {userDataManager.CurrentUserData.Gold}";
        userDataManager.getGoldAction += goldUiValueSet;
    }
    public void goldUiValueSet(int? gold)
    {
        Debug.Log(gold);
        goldText.text = $"GOLD : {gold}";
    }
    public void SetGoldUi()
    {
        goldText.text = $"GOLD : {userDataManager.CurrentUserData.Gold}";
    }
}
