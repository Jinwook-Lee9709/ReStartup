using UnityEngine;
using UnityEngine.AI;

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
    public ConsumerFSM FSM => GetComponent<ConsumerFSM>();

    public Transform NextTargetTransform
    {
        get => nextTargetTransform;
        set
        {
            if (value != nextTargetTransform) OnTargetTransformChanged(value);
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

    public void OnTableVacated()
    {
        FSM.CurrentStatus = ConsumerFSM.ConsumerState.BeforeOrder;
    }

    private void OnTargetTransformChanged(Transform transform)
    {
        if(transform != null)
            agent.SetDestination(transform.position);
    }

    public void SetTable(Table table)
    {
        if (table != null) currentTable = table;
    }
}