using System;
using TMPro;
using UnityEngine;

public class AgentWorkDisplay : Debugger
{
    [SerializeField] private EmployeeFSM target;
    [SerializeField] private TextMeshPro text;

    private WorkBase prevWork;
    
    private void Start()
    {
        gameObject.SetActive(false);
        if (target != null)
        {
            base.Start();
            prevWork = target.CurrentWork;
        }
    }

    private void Update()
    {
        if (target.CurrentWork != prevWork)
        {
            OnActiavted();
            prevWork = target.CurrentWork;
        }
    }
    protected override void OnActiavted()
    {
        gameObject.SetActive(true);
        CheckAndSetText();
    }
    private void CheckAndSetText()
    {
        if (target is not null)
        {
            if (target.CurrentWork != null)
            {
                SetText();
            }
            else
            {
                text.text = "대기중";
            }
        }
    }
    
    protected override void OnDeactivated()
    {
        gameObject.SetActive(false);
        DeactiveText();
    }


    private void SetText()
    {
        if (text is not null)
        {
            text.enabled = true;
            text.text = WorkToString(target.CurrentWork.GetType());
        }
    }


    private void DeactiveText()
    {
        if (text is not null)
        {
            text.text = null;
            text.enabled = false;
        }
    }

    private string WorkToString(Type type)
    {
        switch (type)
        {
            case not null when type == typeof(WorkCleanTable):
                return "테이블 청소 작업중";
            case not null when type == typeof(WorkFoodToTable):
                return "음식 테이블로 배달중";
            case not null when type == typeof(WorkGetOrder):
                return "주문 받는중";
            case not null when type == typeof(WorkGotoFoodPickupCounter):
                return "카운터 음식 가지러 이동중";
            case not null when type == typeof(WorkPayment):
                return "계산중";
            case not null when type == typeof(WorkCooking):
                return "요리중";
            case not null when type == typeof(WorkFoodToHall):
                return "홀로 요리 배달중";
            case not null when type == typeof(WorkGotoCookingStation):
                return "요리 가지러 이동중";
            default:
                return "대기중";
        }
    }
}
