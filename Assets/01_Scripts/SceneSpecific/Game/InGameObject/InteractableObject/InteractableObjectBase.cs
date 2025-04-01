using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObjectBase : MonoBehaviour, IInteractable, IComparable<InteractableObjectBase>
{
    //References
    [SerializeField] private List<InteractPivot> interactablePoint;

    //LocalVariables
    [SerializeField] private float interactProgress;
    private InteractWorkBase currentWork;
    private float interactionSpeed;

    //Properties
    public int Id { get; private set; }

    public int CompareTo(InteractableObjectBase other)
    {
        return Id.CompareTo(other.Id);
    }

    public float InteractProgress => interactProgress;
    public InteractStatus InteractStatus { get; private set; }
    
    
    public event Action<float> OnInteractedEvent;
    public event Action OnClearWorkEvent;
    public event Action OnInteractFinishedEvent;

    public List<InteractPivot> InteractablePoints => interactablePoint;
    
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
        interactionSpeed = CalculateInteractionSpeed(interactor);
    }

    public InteractStatus OnInteract(IInteractor interactor)
    {
        var interactionResult = IncreaseProgress(interactionSpeed);

        OnInteractedEvent?.Invoke(interactProgress);

        InteractStatus = interactionResult ? InteractStatus.Success : InteractStatus.Progressing;

        if (InteractStatus == InteractStatus.Success) OnInteractCompleted();

        return InteractStatus;
    }

    public virtual void OnInteractCanceled()
    {
        InteractStatus = InteractStatus.Pending;
    }

    public virtual void OnInteractCompleted()
    {
        OnInteractFinishedEvent?.Invoke();
        ClearWork();
    }

    public void SetWork(InteractWorkBase workBase)
    {
        currentWork = workBase;
    }

    public virtual void SetId(int id)
    {
        this.Id = id;
    }

    public void ClearWork()
    {
        currentWork = null;
        OnClearWorkEvent?.Invoke();
        interactProgress = 0f;
    }

    private bool IncreaseProgress(float interactionSpeed)
    {
        if (interactionSpeed == 0)
        {
            interactProgress = 1;
            return true;
        }
        interactProgress += interactionSpeed * Time.deltaTime;
        return interactProgress >= 1;
    }

    private float CalculateInteractionSpeed(IInteractor interactor)
    {
        if (interactor == null || currentWork.InteractTime == 0)
            return 0;

        return 1 / interactor.InteractionSpeed / currentWork.InteractTime;
    }
}