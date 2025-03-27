using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingStation : InteractableObjectBase
{
    public override void OnInteractCompleted()
    {
        base.OnInteractCompleted();
        Debug.Log("Job's Done");
    }
}
