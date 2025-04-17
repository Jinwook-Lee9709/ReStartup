using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public abstract class InteractWorkBase : WorkBase
{
    public Func<Transform> customTarget;
    private IInteractor interactor;
    protected float interactTime = 1;
    protected IInteractable target;

    private Transform targetTransform;
    private bool isInteruptible;

    //References
    private NavMeshAgent workerAgent;

    //LocalVariables
    private WorkPhase workPhase;

    public bool IsInteruptible => isInteruptible;

    public InteractWorkBase(WorkManager workManager, WorkType workType, float interactTime = 1, bool isInteruptible = true, bool isStoppable = true) : base(workManager, workType, isStoppable)
    {
        this.interactTime = interactTime;
        this.isInteruptible = isInteruptible;
    }

    //Properties
    public float InteractTime => interactTime;

    public void SetInteractable(IInteractable target)
    {
        this.target = target;
    }

    public override void OnWorkRegistered()
    {
    }

    public override void OnAssignWorker(WorkerBase worker)
    {
        this.worker = worker;
        workerAgent = worker.GetComponent<NavMeshAgent>();
        interactor = worker as IInteractor;
        SetTarget();
        EvaluateWorkerArrival();
    }

    private void SetTarget()
    {
        if (customTarget == null)
            targetTransform = GetClosestInteractablePoint();
        else
            targetTransform = customTarget();
    }

    private void EvaluateWorkerArrival()
    {
        var isArrive = IsArrive(workerAgent, targetTransform);
        if (!isArrive)
        {
            workerAgent.SetDestination(targetTransform.position);
            interactor.PlayWalkAnimation();
            workPhase = WorkPhase.Moving;
        }
        else
        {
            workPhase = WorkPhase.Working;
            target.OnInteractStarted(interactor);
        }
    }

    public override void DoWork()
    {
        switch (workPhase)
        {
            case WorkPhase.Moving:
            {
                if (IsArrive(workerAgent, targetTransform)) StartInteraction();
                break;
            }
            case WorkPhase.Working:
            {
                var interactStatus = target.OnInteract(interactor);
                if (interactStatus == InteractStatus.Success) CompleteInteraction();
                break;
            }
        }
    }

    public override void OnWorkCanceled()
    {
        OnWorkFinished();
        Debug.Log($"OnWorkCanceled: {CreatedTime}");
    }

    public override void OnWorkStopped()
    {
        worker = null;
        workManager.AddStoppedWork(workType, this);
        if (workPhase == WorkPhase.Working) target.OnInteractCanceled();
    }

    public override void OnWorkFinished()
    {
        if (nextWorker != worker && worker is not null)
        {
            worker.OnWorkFinished();
        }
        workManager.OnWorkFinished(this);
    }

    protected abstract void HandlePostInteraction();

    private void StartInteraction()
    {
        workPhase = WorkPhase.Working;
        interactor.PlayWorkAnimation();
        target.OnInteractStarted(interactor);
        target.OnInteract(interactor);
    }

    private void CompleteInteraction()
    {
        isComplete = true;
        if(Worker.WorkType != WorkType.All)
        {
            var employee = Worker as EmployeeFSM;
            employee.DecreaseHp(Constants.HEALTH_DECREASE_AMOUNT_ONWORKFINISHED);
            employee.uiManager.EmployeeHpSet(employee);
        }
        HandlePostInteraction();
        OnWorkFinished();
    }

    private bool IsArrive(NavMeshAgent agent, Transform target)
    {
        var agentPosition = agent.transform.position;
        var targetPosition = target.position;
        return Vector3.SqrMagnitude(agentPosition - targetPosition) <= Mathf.Sqrt(agent.stoppingDistance);
    }

    private Transform GetClosestInteractablePoint()
    {
        return target.GetInteractablePoints(workType.WorkTypeToPermission())
            .OrderBy(x => Vector3.Distance(workerAgent.transform.position, x.transform.position))
            .First().transform;
    }
}