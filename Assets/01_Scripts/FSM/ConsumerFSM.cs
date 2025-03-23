using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ConsumerFSM : MonoBehaviour
{
    //TODO : �ֹ��� ����, ���� ���̺�
    public Table currentTable = null;
    public enum ConsumerState
    {
        Waiting,
        BeforeOrder,
        AfterOrder,
        HappyMeal,
        
    }
    private NavMeshAgent agent;

    private ConsumerState currentStatus;

    public ConsumerState CurrentStatus
    {
        get { return currentStatus; }
        set
        {
            ConsumerState prevStatus = currentStatus;
            currentStatus = value;
            
        }
    }
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

}
