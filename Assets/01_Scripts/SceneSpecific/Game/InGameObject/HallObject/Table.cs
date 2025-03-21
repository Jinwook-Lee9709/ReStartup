using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Table : MonoBehaviour, IInteractable
{
    //References
    [SerializeField] private List<Transform> interactablePoint;
    private ProgressiveWork work;
    
    //Properties
    public float InteractProgress => interactProgress;
    public InteractStatus InteractStatus => interactStatus;
    public List<Transform> InteractablePoints => interactablePoint;
    
    //LocalVariables
    private float interactProgress = 0;
    private float interactionSpeed;
    private InteractStatus interactStatus;
    public event Action<float> OnInteracted;
    
    public void SetWork(ProgressiveWork work)
    {
        this.work = work;
    }

    public void OnInteractStarted(IInteractor interactor)
    {
        interactionSpeed = CalculateInteractionSpeed(interactor);
    }

    public InteractStatus OnInteract(IInteractor interactor)
    {
        bool interactionResult = IncreaseProgress(interactionSpeed);
        
        OnInteracted?.Invoke(interactProgress);

        interactStatus = interactionResult ? InteractStatus.Success : InteractStatus.Progressing;
        return interactStatus;
    }

    public void OnInteractCanceled()
    {
        interactionSpeed = 0;
        interactStatus = InteractStatus.Pending;
    }


    public void OnInteractCompleted()
    {
    }

    private bool IncreaseProgress(float interactionSpeed)
    {
        interactProgress += interactionSpeed * Time.deltaTime;
        Debug.Log(interactProgress);
        return interactProgress >= 1;
    }


    private float CalculateInteractionSpeed(IInteractor interactor)
    {
        if (work.workDuration == 0)
            return 1;
        
        float interactionAmount = 1 / interactor.InteractionSpeed / work.workDuration;
        return interactionAmount;
    }
}