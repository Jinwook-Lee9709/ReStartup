using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotEnoughCostPopup : PopUp
{
    protected override void Start()
    {
        base.Start();
        StartCoroutine(WaitTwoSeconds());
    }

    private IEnumerator WaitTwoSeconds()
    {
        yield return new WaitForSeconds(2f);
        OnCancle();
    }
}
