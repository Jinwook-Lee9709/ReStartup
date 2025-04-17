using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObjectBase : MonoBehaviour, IInteractable, IComparable<InteractableObjectBase>
{
    //References
    [SerializeField] private List<InteractPivot> interactablePoint;

    //LocalVariables
    [SerializeField] private float interactProgress;
    private float calculatedInterationSpeed;
    private float interactionSpeed = 1f;
    public float InteractionSpeed => interactionSpeed;
    //Properties
    public int Id { get; private set; }
    public InteractWorkBase CurrentWork { get; private set; }

    public int CompareTo(InteractableObjectBase other)
    {
        return Id.CompareTo(other.Id);
    }

    public float InteractProgress => interactProgress;
    public InteractStatus InteractStatus { get; private set; }

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
        calculatedInterationSpeed = CalculateInteractionSpeed(interactor);
    }

    public InteractStatus OnInteract(IInteractor interactor)
    {
        var interactionResult = IncreaseProgress();

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


    public event Action<float> OnInteractedEvent;
    public event Action OnClearWorkEvent;
    public event Action OnInteractFinishedEvent;

    public void SetWork(InteractWorkBase workBase)
    {
        CurrentWork = workBase;
    }

    public virtual void SetId(int id)
    {
        Id = id;
    }

    public void SetInteractionSpeed(float speed)
    {
        interactionSpeed = speed;
    }

    public void ClearWork()
    {
        CurrentWork = null;
        OnClearWorkEvent?.Invoke();
        interactProgress = 0f;
    }

    private bool IncreaseProgress()
    {
        if (calculatedInterationSpeed == 0)
        {
            interactProgress = 1;
            return true;
        }

        interactProgress += calculatedInterationSpeed * Time.deltaTime;

        return interactProgress >= 1;
    }

    private float CalculateInteractionSpeed(IInteractor interactor)
    {
        if (interactor == null || CurrentWork.InteractTime == 0)
            return 0;

        return 1 / interactor.InteractionSpeed / CurrentWork.InteractTime / interactionSpeed;
    }

    public abstract bool ShowIcon(IconPivots pivot, Sprite icon, Sprite background = null, bool flipBackGround = false);
    public abstract void HideIcon();
}