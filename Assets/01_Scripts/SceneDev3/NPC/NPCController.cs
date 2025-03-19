using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
public enum NPCType
{
    Employee, //직원
    Customer, //손님
}
public class NPCController : MonoBehaviour
{
    private BehaviorTree<NPCController> behaviorTree;

    [SerializeField]
    private NPCType NPCType;
    public Transform target;     //작업대
    public float speed;
    [HideInInspector]
    public float targetDistance; //작업대와 나의 거리
    public float workRange = 3f; //일을 시작할 거리
    public float woringTimer = 3f; //일하는 시간

    private bool workTakenAway;
    public bool WorkTakenAway {  get { return workTakenAway; } set { workTakenAway = value; } }

    void Start()
    {
        switch (NPCType)
        {
            case NPCType.Employee:
                EmployeeInitBehaviorTree();
                break;
            case NPCType.Customer:
                CustomerInitBehaviorTree();
                break;
        }
    }

    public void EmployeeInitBehaviorTree()
    {

    }
    public void CustomerInitBehaviorTree()
    {

    }
    private void Update()
    {
        targetDistance = Vector3.Distance(transform.position, target.position);
    }
}
