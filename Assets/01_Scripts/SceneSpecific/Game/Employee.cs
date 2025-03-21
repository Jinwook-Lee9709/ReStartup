using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Employee : WorkerBase, IInteractor
{
    private void Update()
    {
        if (currentWork != null)
        {
            currentWork.DoWork();
        }
    }

    private float interactionSpeed = 1f;
    public float InteractionSpeed => interactionSpeed;

}
