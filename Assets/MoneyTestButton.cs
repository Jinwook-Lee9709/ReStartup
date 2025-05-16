using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyTestButton : MonoBehaviour
{
    public enum CostType
    {
        Money,
        Gold
    }
    public CostType costType;
    private Button button;

    private void Start()
    {
        // button = GetComponent<Button>();
        // button.onClick.AddListener(AddCost);
    }

    private void AddCost()
    {
        switch (costType)
        {
            case CostType.Money:
                UserDataManager.Instance.AdjustMoneyWithSave(1000000).Forget();
                break;
            case CostType.Gold:
                UserDataManager.Instance.AdjustGoldWithSave(1000000).Forget();
                break;
        }
    }
}
