using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObjectBase : MonoBehaviour, IInteractable, IComparable<InteractableObjectBase>
{
    //References
    [SerializeField] private List<Transform> interactablePoint;
    private InteractWorkBase currentWork;
    
    //LocalVariables
    private float interactProgress = 0;
    private float interactionSpeed;
    private InteractStatus interactStatus;
    
    //Properties
    public int Id { get; set; }
    public float InteractProgress => interactProgress;
    public InteractStatus InteractStatus => interactStatus;
    public List<Transform> InteractablePoints => interactablePoint;

    public event Action<float> OnInteractedEvent;
    public event Action OnInteractFinishedEvent;
    
    public void SetWork(InteractWorkBase workBase)
    {
        currentWork = workBase;
    }

    public void ClearWork()
    {
        currentWork = null;
        interactProgress = 0f;
    }
    
    public void OnInteractStarted(IInteractor interactor)
    {
        interactionSpeed = CalculateInteractionSpeed(interactor);
    }

    public InteractStatus OnInteract(IInteractor interactor)
    {
        bool interactionResult = IncreaseProgress(interactionSpeed);
        
        OnInteractedEvent?.Invoke(interactProgress);

        interactStatus = interactionResult ? InteractStatus.Success : InteractStatus.Progressing;

        if (interactStatus == InteractStatus.Success)
        {
            OnInteractCompleted();
        }

        return interactStatus;
    }

    public virtual void OnInteractCanceled()
    {
        interactStatus = InteractStatus.Pending;
    }

    public virtual void OnInteractCompleted()
    {
        OnInteractFinishedEvent?.Invoke();
        ClearWork();
    }
    
    private bool IncreaseProgress(float interactionSpeed)
    {
        interactProgress += interactionSpeed * Time.deltaTime;
        Debug.Log(interactProgress);
        return interactProgress >= 1;
    }

    private float CalculateInteractionSpeed(IInteractor interactor)
    {
        if (interactor == null)
            return 0;
        if (currentWork.InteractTime == 0)
            return 1;
        
        return 1 / interactor.InteractionSpeed / currentWork.InteractTime;
    }

    public int CompareTo(InteractableObjectBase other)
    {
        return Id.CompareTo(other.Id);
    }
}
