using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Debug = UnityEngine.Debug;

public class ProgressiveWork : WorkBase
{
    private enum WorkPhase
    {
        Moving,
        Working
    }

    private WorkPhase workPhase;

    public float workDuration = 10f;
    public float WorkDuration { get; private set; }

    private WorkerBase worker;
    private IInteractor interactor;
    private NavMeshAgent workerAgent;

    private IInteractable target;
    private Transform targetTransform;

    public ProgressiveWork(WorkManager workManager, WorkType workType) : base(workManager, workType)
    {
    }

    public void SetInteractable(IInteractable target)
    {
        this.target = target;
    }

    public override void OnAssignWorker(WorkerBase worker)
    {
        this.worker = worker;
        workerAgent = worker.GetComponent<NavMeshAgent>();
        interactor = worker.GetComponent<IInteractor>();
        var stoppingDistance = workerAgent.stoppingDistance;

        targetTransform = target.InteractablePoints
            .OrderBy(x => Vector3.Distance(workerAgent.transform.position, x.position)).First();

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
                    workPhase = WorkPhase.Working;
                    target.OnInteractStarted(interactor);
                    target.OnInteract(interactor);
                }
                break;
            }
            case WorkPhase.Working:
            {
                var interactStatus = target.OnInteract(interactor);
                if (interactStatus == InteractStatus.Success)
                {
                    target.OnInteractCompleted();
                    OnWorkFinished();
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
        worker.OnWorkFinished();
    }

    private bool IsArrive(NavMeshAgent agent, Transform target)
    {
        Vector3 agentPosition = agent.transform.position;
        Vector3 targetPosition = target.position;
        return Vector3.SqrMagnitude(agentPosition - targetPosition) <= Mathf.Sqrt(agent.stoppingDistance);
    }
}