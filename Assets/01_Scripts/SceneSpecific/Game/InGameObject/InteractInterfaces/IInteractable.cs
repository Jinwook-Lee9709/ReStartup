using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IInteractable
{
    List<Transform> InteractablePoints { get; }
    InteractStatus InteractStatus { get; }
    float InteractProgress { get; }
    
    void OnInteractStarted(IInteractor interactor);
    InteractStatus OnInteract(IInteractor interactor);
    void OnInteractCanceled();
    void OnInteractCompleted();   
}
