using System;
using UnityEngine;
using UnityEngine.AI;

public class Player : WorkerBase, IInteractor, ITransportable
{
    [SerializeField] private Transform handPivot;
    [SerializeField] private Transform hallIdleArea, kitchenIdleArea;
    private WorkManager workManager;
    private float interactionSpeed = Constants.PLAYER_INTERACTION_SPEED;
    public float InteractionSpeed => interactionSpeed;
    public Transform HandPivot => handPivot;
    private bool isCameraOnHall;

    public void SetWorkManager(WorkManager workManager)
    {
        this.workManager = workManager;
    }

    public WorkerState CurrentStatus
    {
        get => currentStatus;
        private set
        {
            currentStatus = value;
            if (currentStatus == WorkerState.ReturnidleArea)
                agent.SetDestination(isCameraOnHall ? hallIdleArea.position : kitchenIdleArea.position);
        }
    }

    private WorkerState currentStatus;

    private void Update()
    {
        switch (currentStatus)
        {
            case WorkerState.Idle:
                UpdateIdle();
                break;
            case WorkerState.ReturnidleArea:
                UpdateReturnidleArea();
                break;
            case WorkerState.Working:
                UpdateWorking();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public override void AssignWork(WorkBase work)
    {
        base.AssignWork(work);
        CurrentStatus = WorkerState.Working;
    }

    private void UpdateIdle()
    {
    }

    private void UpdateReturnidleArea()
    {
        var distance = Vector3.Distance(transform.position, isCameraOnHall ? hallIdleArea.position : kitchenIdleArea.position);
        if (distance <= agent.stoppingDistance) CurrentStatus = WorkerState.Idle;
    }

    private void UpdateWorking()
    {
        if (currentWork == null)
        {
            CurrentStatus = WorkerState.ReturnidleArea;
            return;
        }

        currentWork.DoWork();
    }

    public void UpdateIdleArea(bool isCameraOnHall)
    {
        this.isCameraOnHall = isCameraOnHall;
        if (CurrentStatus != WorkerState.Working)
        {
            CurrentStatus = WorkerState.ReturnidleArea;
            agent.SetDestination(isCameraOnHall ? hallIdleArea.position : kitchenIdleArea.position);
        }
    }

    public void OnTouch(Vector2 pos)
    {
        if (currentWork != null)
        {
            return;
        }

        GameObject targetObject = null;

        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(pos);
        var hit = Physics2D.RaycastAll(worldPoint, Vector2.zero);
        for (var i = 0; i < hit.Length; i++)
            if (hit[i].collider != null)
                if (hit[i].collider.CompareTag(Strings.InteractableObjectTag))
                {
                    targetObject = hit[i].collider.gameObject;
                    break;
                }

        if (targetObject != null)
        {
            var interactableObject = targetObject.GetComponent<InteractableObjectBase>();
            InteractableObjectTouched(interactableObject);
        }
    }

    public void InteractableObjectTouched(InteractableObjectBase interactableObject)
    {
        if (interactableObject != null)
        {
            if (interactableObject.CurrentWork != null)
            {
                if (interactableObject.CurrentWork.IsInteruptable)
                    workManager.OnPlayerStartWork(interactableObject.CurrentWork, this);
            }
        }
    }

    public void LiftPackage(GameObject package)
    {
        package.transform.SetParent(handPivot);
        package.transform.localPosition = Vector3.zero;
    }

    public void DropPackage(Transform dropPoint)
    {
        if (handPivot.childCount > 0)
        {
            var package = handPivot.GetChild(0).gameObject;
            package.transform.SetParent(dropPoint);
            package.transform.localPosition = Vector3.zero;
        }
    }
}