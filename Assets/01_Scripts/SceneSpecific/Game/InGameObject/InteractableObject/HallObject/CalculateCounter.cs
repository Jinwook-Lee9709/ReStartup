using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateCounter : InteractableObjectBase
{
    public override void OnInteractCompleted()
    {
        Debug.Log("Calculate");
        OnInteractFinishedEvent += UpMoney;
    }
    private void UpMoney()
    {
        //��� �÷��ִ� �ż��� ȣ��
        TestMoney.GetMoney(1);
    }
}
