using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConsumerDisplay : Debugger
{
    [SerializeField] private ConsumerFSM target;
    [SerializeField] private TextMeshPro text;
    private void CheckAndSetText()
    {
        if (target is not null)
        {
            SetText();
        }
    }
    private void SetText()
    {
        if (text is not null)
        {
            text.enabled = true;
            text.text = WorkToString(target.CurrentStatus);
        }
    }

    private string WorkToString(ConsumerFSM.ConsumerState state)
    {
        switch (state)
        {
            case ConsumerFSM.ConsumerState.Waiting:
                return "입장 대기 중";
            case ConsumerFSM.ConsumerState.BeforeOrder:
                return "주문 대기 중";
            case ConsumerFSM.ConsumerState.AfterOrder:
                return "음식 대기 중";
            case ConsumerFSM.ConsumerState.Eatting:
                return "식사 중";
            case ConsumerFSM.ConsumerState.WaitForPay:
                return "결제 대기 중";
            case ConsumerFSM.ConsumerState.Paying:
                return "결제 중";
            case ConsumerFSM.ConsumerState.Exit:
                return "퇴장";
            default:
                return "문제가 생김";
        }
    }
    private void Update()
    {
        OnActiavted();
    }

    protected override void OnActiavted()
    {
        gameObject.SetActive(true);
        CheckAndSetText();
    }

    protected override void OnDeactivated()
    {
        gameObject.SetActive(false);
        DeactiveText();
    }
    private void DeactiveText()
    {
        if (text is not null)
        {
            text.text = null;
            text.enabled = false;
        }
    }
}
