using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PaymentText : MonoBehaviour
{
    private readonly string paymentFormat = "+ {0}";
    private readonly string tipFormat = "+ {0}!";
    private readonly Color targetColor = new Color(1, 1, 1, 0);
    [SerializeField] TextMeshPro paymentText, tipText;

    private void Start()
    {

    }

    public void Init(Consumer consumer, bool isTip)
    {
        gameObject.transform.DOLocalMoveY(consumer.transform.position.y + 1.5f, 2).OnComplete(() =>
        {
            Destroy(gameObject);
        });
        paymentText.text = string.Format(paymentFormat, consumer.needFood.SellingCost);
        paymentText.DOColor(targetColor, 2);
        if (isTip)
        {
            var tip = Mathf.CeilToInt(consumer.needFood.SellingCost * (consumer.FSM.consumerData.SellTipPercent / 100f));
            tipText.gameObject.SetActive(isTip);
            tipText.text = string.Format(tipFormat, tip);
            tipText.DOColor(targetColor, 2);
        }
    }
}
