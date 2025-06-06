using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Consumer : MonoBehaviour
{
    public ConsumerManager consumerManager;

    //TODO : �ֹ��� ����, ���� ���̺�
    public Table currentTable;
    private NavMeshAgent agent;
    private Animator animator;
    public FoodData needFood;
    private Transform nextTargetTransform;
    public ConsumerPairData pairData = null;
    public List<int> foodIds = new();
    public ConsumerFSM FSM => GetComponent<ConsumerFSM>();

    public bool isEndMeal = false;
    public bool isFoodReady = false;
    public Transform NextTargetTransform
    {
        get => nextTargetTransform;
        set
        {
            OnTargetTransformChanged(value);
            nextTargetTransform = value;
        }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    public void OnTableVacated()
    {
        FSM.CurrentStatus = ConsumerFSM.ConsumerState.BeforeOrder;
    }

    private void OnTargetTransformChanged(Transform transform)
    {
        if (transform != null)
            agent.SetDestination(transform.position);
    }

    public void SetTable(Table table)
    {
        if (table is not null)
        {
            currentTable = table;
            if (pairData != null)
            {
                pairData.partner.currentTable = table.PairTable;
            }
        }
    }
}