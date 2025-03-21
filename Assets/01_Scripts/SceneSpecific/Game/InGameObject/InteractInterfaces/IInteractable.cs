using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractStatus
{
    Progressing,
    Success
}

public interface IInteractable
{
    InteractStatus OnInteract(IInteractor interactor);
    void OnInteractComplete();   
}
