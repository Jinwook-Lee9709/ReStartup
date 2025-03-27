using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IInteractable
{
    List<InteractPivot> InteractablePoints { get; }
    InteractStatus InteractStatus { get; }
    float InteractProgress { get; }
    
    List<InteractPivot> GetInteractablePoints(InteractPermission permission);
    void OnInteractStarted(IInteractor interactor);
    InteractStatus OnInteract(IInteractor interactor);
    void OnInteractCanceled();
    void OnInteractCompleted();   
}
