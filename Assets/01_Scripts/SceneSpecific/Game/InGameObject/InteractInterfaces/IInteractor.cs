using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IInteractor
{
    float InteractionSpeed { get; }
    
    void InteractWithObject(IInteractable interactable);
    void OnInteractComplete(IInteractable interactable);
}
