using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;

public class EmployeeFSM : WorkerBase
{
    [SerializeField]
    private Transform idleArea;
    public enum EnployedState
    {
        Idle,
        ReturnidleArea,
        Working,
    }

    private EnployedState currentStatus;

    public EnployedState CurrentStatus
    {
        get { return currentStatus; }
        set
        {
            EnployedState prevStatus = currentStatus;
            currentStatus = value;
            switch (currentStatus)
            {
                case EnployedState.Idle:
                    //DataTable« ø‰
                    break;
                case EnployedState.ReturnidleArea:
                    if (currentWork != null)
                        currentStatus = EnployedState.Working;

                    agent.SetDestination(idleArea.position);
                    break;
                case EnployedState.Working:
                    if (currentWork == null)
                        currentStatus = EnployedState.ReturnidleArea;

                    currentWork.DoWork();
                    break;
            }
        }
    }
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
}
