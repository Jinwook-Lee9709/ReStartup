using Newtonsoft.Json.Linq;
using System;
using UnityEngine;
using UnityEngine.AI;

public class Consumer : MonoBehaviour
{
    public ConsumerManager consumerManager;
    public ConsumerFSM FSM
    {
        get => GetComponent<ConsumerFSM>();
    }
    private Animator animator;
    private NavMeshAgent agent;
    //TODO : �ֹ��� ����, ���� ���̺�
    public Table currentTable = null;
    public FoodData needFood = new();
    private Transform nextTargetTransform;
    public Transform NextTargetTransform
    {
        get => nextTargetTransform;
        set
        {
            if (value != nextTargetTransform)
            {
                OnTargetTransformChanged(value);
            }
            nextTargetTransform = value;
        }
    }

    private void OnTargetTransformChanged(Transform transform)
    {
        agent.SetDestination(transform.position);
    }

    private void Awake()
    {
        //animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    public void SetTable(Table table)
    {
        if(table != null)
        {
            currentTable = table;
        }
    }
}
