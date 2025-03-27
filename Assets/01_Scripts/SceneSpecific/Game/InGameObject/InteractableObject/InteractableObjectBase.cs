using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public abstract class InteractableObjectBase : MonoBehaviour, IInteractable, IComparable<InteractableObjectBase>
{
    //References
    [SerializeField] private List<InteractPivot> interactablePoint;
    private InteractWorkBase currentWork;
    
    //LocalVariables
    [SerializeField] private float interactProgress = 0;
    private float interactionSpeed;
    private InteractStatus interactStatus;
    
    //Properties
    public int Id { get; set; }
    public float InteractProgress => interactProgress;
    public InteractStatus InteractStatus => interactStatus;
    public List<InteractPivot> InteractablePoints => interactablePoint;

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

    public List<InteractPivot> GetInteractablePoints(InteractPermission permission)
    {
        List<InteractPivot> interactablePoints = new List<InteractPivot>();
        foreach(var pivot in interactablePoint)
        {
            if (pivot.CanAccess(permission))
                interactablePoints.Add(pivot);
        }
        return interactablePoints;
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
