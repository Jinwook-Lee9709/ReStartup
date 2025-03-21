using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;

public class EmployeeFSM : MonoBehaviour
{
    [SerializeField]
    private Transform idleArea;
    public enum EnployedState
    {
        Idle,
        ReturnidleArea,
        Working,
    }
    private NavMeshAgent agent;

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
                    break;
                case EnployedState.ReturnidleArea:
                    agent.SetDestination(idleArea.position);
                    
                    break;
                case EnployedState.Working:
                    break;
            }
        }
    }
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
}
