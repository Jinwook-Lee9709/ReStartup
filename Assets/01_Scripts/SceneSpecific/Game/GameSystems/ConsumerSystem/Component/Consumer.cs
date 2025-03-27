using Newtonsoft.Json.Linq;
using System;
using UnityEngine;
using UnityEngine.AI;

public class Consumer : MonoBehaviour
{
    public ConsumerManager consumerManager;
    public ConsumerPairData pairData = null;
    public ConsumerFSM FSM
    {
        get => GetComponent<ConsumerFSM>();
    }
    private Animator animator;
    private NavMeshAgent agent;
    //TODO : �ֹ��� ����, ���� ���̺�
    public Table currentTable = null;
    public FoodData needFood;
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

    public void OnTableVacated()
    {
        FSM.CurrentStatus = ConsumerFSM.ConsumerState.BeforeOrder;
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
        needFood = DataTableManager.Get<FoodDataTable>("Food").GetFoodData(301001);
    }

    public void SetTable(Table table)
    {
        if(table != null)
        {
            currentTable = table;
        }
    }
}
