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
        //animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        needFood = DataTableManager.Get<FoodDataTable>("Food").GetFoodData(301001);
    }
    private void OnEnable() // 박성민 만듬 ConsumerManager에 foodIds 필드 추가.
    {
        var gameManager = GameObject.FindWithTag("GameManager").gameObject;
        consumerManager = gameManager.GetComponent<GameManager>().consumerManager;
        foodIds = consumerManager.foodIds;
        var ran = Random.Range(0, foodIds.Count-1);
        needFood = DataTableManager.Get<FoodDataTable>("Food").GetFoodData(foodIds[ran]);
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