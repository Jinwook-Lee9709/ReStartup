using Newtonsoft.Json.Linq;
using System;
using UnityEngine;
using UnityEngine.AI;

public class Consumer : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;
    //TODO : 주문한 음식, 현재 테이블
    public Table currentTable = null;
    //public Food needFood = null;
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
