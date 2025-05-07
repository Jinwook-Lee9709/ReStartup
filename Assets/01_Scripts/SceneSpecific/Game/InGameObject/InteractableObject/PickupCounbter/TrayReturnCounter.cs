using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrayReturnCounter : MonoBehaviour, IInteractable
{
    //References
    [SerializeField] private List<InteractPivot> interactablePoint;
    public List<InteractPivot> InteractablePoints => interactablePoint;
    
    public InteractStatus InteractStatus { get; }
    public float InteractProgress { get; }

    public List<InteractPivot> GetInteractablePoints(InteractPermission permission)
    {
        var interactablePoints = new List<InteractPivot>();
        foreach (var pivot in interactablePoint)
            if (pivot.CanAccess(permission))
                interactablePoints.Add(pivot);
        return interactablePoints;
    }

    public void OnInteractStarted(IInteractor interactor)
    {
    }

    public InteractStatus OnInteract(IInteractor interactor)
    {
        return InteractStatus.Success;
    }

    public void OnInteractCanceled()
    {
    }

    public void OnInteractCompleted()
    {
    }
    
}
