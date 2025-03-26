using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;

public class WorkerBase : MonoBehaviour
{ 
    protected WorkBase currentWork;
    
    [SerializeField]protected WorkerManager workerManager;
    protected NavMeshAgent agent;
    
    public void Init(WorkerManager manager)
    {
        workerManager = manager;
    }

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    bool IsBusy => currentWork != null;
    
    public virtual void AssignWork(WorkBase work)
    {
        if (IsBusy)
        {
            Debug.LogWarning($"{gameObject.name} is already busy!");
            return;
        }

        currentWork = work;
        currentWork.OnAssignWorker(this);
    }

    public void ClearWork()
    {
        currentWork = null;
    }

    public virtual void OnWorkFinished()
    {
        currentWork = null;
        workerManager.ReturnWorker(this);   
    }
    
    public virtual void CancelWork()
    {
        if (currentWork != null)
        {
            currentWork.OnWorkStopped();
            OnWorkFinished();
        }
    }
}
