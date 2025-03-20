using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
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

    public Vector3 idlePos;
    
    private bool canVisit = false;
    public bool CanVisit { get { return canVisit; } }

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
        behaviorTree = new BehaviorTree<NPCController>(this);
        var rootSelcector = new SelectorrNode<NPCController>(this);

        var workingSequence = new SequenceNode<NPCController>(this);
        workingSequence.AddChild(new WorkingCondition(this));
        workingSequence.AddChild(new WorkingAction(this));

        var returnSequence = new SequenceNode<NPCController>(this);
        returnSequence.AddChild(new CanReturnCondition(this));
        returnSequence.AddChild(new ReturnAction(this));

        var visitIdleSequence = new SequenceNode<NPCController>(this);
        visitIdleSequence.AddChild(new VisitAction(this));
        visitIdleSequence.AddChild(new IdleAction(this));

        rootSelcector.AddChild(workingSequence);
        rootSelcector.AddChild(returnSequence);
        rootSelcector.AddChild(visitIdleSequence);

        behaviorTree.SetRoot(rootSelcector);
    }
    public void CustomerInitBehaviorTree()
    {

    }
    private void Update()
    {
        targetDistance = Vector3.Distance(transform.position, target.position);
    }
}
