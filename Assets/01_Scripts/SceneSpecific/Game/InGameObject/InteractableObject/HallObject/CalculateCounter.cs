using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateCounter : InteractableObjectBase
{
    public override void OnInteractCompleted()
    {
        Debug.Log("Calculate");
        OnInteractFinishedEvent += UpGold;
    }
    private void UpGold()
    {
        //골드 올려주는 매서드 호출
    }
}
