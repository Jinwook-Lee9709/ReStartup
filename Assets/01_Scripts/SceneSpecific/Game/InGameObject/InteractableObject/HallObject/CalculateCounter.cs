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
        //골드 올려주는 매서드 호출
        TestMoney.GetMoney(1);
    }
}
