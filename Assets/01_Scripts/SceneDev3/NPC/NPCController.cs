using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
public enum NPCType
{
    Employee, //����
    Customer, //�մ�
}
public class NPCController : MonoBehaviour
{
    private BehaviorTree<NPCController> behaviorTree;

    [SerializeField]
    private NPCType NPCType;
    public Transform target;     //�۾���
    public float speed;
    [HideInInspector]
    public float targetDistance; //�۾���� ���� �Ÿ�
    public float workRange = 3f; //���� ������ �Ÿ�
    public float woringTimer = 3f; //���ϴ� �ð�

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
