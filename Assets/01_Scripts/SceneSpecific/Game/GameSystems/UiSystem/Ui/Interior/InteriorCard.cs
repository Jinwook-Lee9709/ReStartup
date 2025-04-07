using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteriorCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image icon;
    [SerializeField] private Button button;

    public void Init(InteriorData data)
    {
        nameText.text = data.Name;
        costText.text = data.SellingCost.ToString();
    }
    
}
