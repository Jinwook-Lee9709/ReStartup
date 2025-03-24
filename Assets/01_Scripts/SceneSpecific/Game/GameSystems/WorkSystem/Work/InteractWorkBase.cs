using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Debug = UnityEngine.Debug;

public abstract class InteractWorkBase : WorkBase
{
    //References
    private IInteractable target;
    private Transform targetTransform;
    protected WorkerBase worker;
    private IInteractor interactor;
    private NavMeshAgent workerAgent;

    //LocalVariables
    private WorkPhase workPhase;
    private float interactTime;
    
    
    //Properties
    public float InteractTime => interactTime;


    public InteractWorkBase(WorkManager workManager, WorkType workType, float interactTime) : base(workManager, workType)
    {
        this.interactTime = interactTime;
    }

    public void SetInteractable(IInteractable target)
    {
        this.target = target;
    }

    public override void OnWorkAssigned()
    {
    }

    public override void OnAssignWorker(WorkerBase worker)
    {
        this.worker = worker;
        workerAgent = worker.GetComponent<NavMeshAgent>();
        interactor = worker as IInteractor;

        targetTransform = GetClosestInteractablePoint();

        bool isArrive = IsArrive(workerAgent, targetTransform);
        if (!isArrive)
        {
            workerAgent.SetDestination(targetTransform.position);
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
                if (IsArrive(workerAgent, targetTransform))
                {
                    StartInteraction();
                }
                break;
            }
            case WorkPhase.Working:
            {
                var interactStatus = target.OnInteract(interactor);
                if (interactStatus == InteractStatus.Success)
                {
                    CompleteInteraction();
                }
                break;
            }
        }
    }

    public override void OnWorkStopped()
    {
        workManager.AddCanceledWork(workType, this);
        if (workPhase == WorkPhase.Working)
        {
            target.OnInteractCanceled();
        }
    }

    public override void OnWorkFinished()
    {
        if(nextWorker != worker)
            worker.OnWorkFinished();
        workManager.OnWorkFinished(this);
    }
    
    protected abstract void HandlePostInteraction();
    
    private void StartInteraction()
    {
        workPhase = WorkPhase.Working;
        target.OnInteractStarted(interactor);
        target.OnInteract(interactor);
    }
    
    private void CompleteInteraction()
    {
        target.OnInteractCompleted();
        HandlePostInteraction();
        OnWorkFinished();
    }
    
    private bool IsArrive(NavMeshAgent agent, Transform target)
    {
        Vector3 agentPosition = agent.transform.position;
        Vector3 targetPosition = target.position;
        return Vector3.SqrMagnitude(agentPosition - targetPosition) <= Mathf.Sqrt(agent.stoppingDistance);
    }
    
    private Transform GetClosestInteractablePoint()
    {
        return target.InteractablePoints
            .OrderBy(x => Vector3.Distance(workerAgent.transform.position, x.position))
            .First();
    }
}