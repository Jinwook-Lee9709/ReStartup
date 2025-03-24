using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Table : InteractableObjectBase
{
    public override void OnInteractCanceled()
    {
        throw new NotImplementedException();
    }

    public override void OnInteractCompleted()
    {
        //throw new NotImplementedException();
        Debug.Log("Job's Done");
    }

    public Table()
    {
    }
}