using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractPivot : MonoBehaviour
{
    [SerializeField] private InteractPermission permission;
    private bool isOccupied;
    public bool IsOccupied => isOccupied;


    public bool CanAccess(InteractPermission permissionToCheck)
    {
        return permission.HasFlag(permissionToCheck);
    }
    
}
