using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IngameGoodsUi : MonoBehaviour
{
    public TextMeshProUGUI goldText;
    private void Start()
    {
        UserDataManager.Instance.action += goldUiValueSet;
    }
    public void goldUiValueSet(int? gold)
    {
        Debug.Log(gold);
        goldText.text = $"GOLD : {gold}";
    }
}
