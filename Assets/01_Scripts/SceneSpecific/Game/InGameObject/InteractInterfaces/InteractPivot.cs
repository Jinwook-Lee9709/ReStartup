using UnityEngine;

public class InteractPivot : MonoBehaviour
{
    [SerializeField] private InteractPermission permission;
    public bool IsOccupied { get; }


    public bool CanAccess(InteractPermission permissionToCheck)
    {
        return permission.HasFlag(permissionToCheck);
    }
}