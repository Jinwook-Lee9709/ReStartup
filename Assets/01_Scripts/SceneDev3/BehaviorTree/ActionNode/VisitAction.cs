using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisitAction : ActionNode<NPCController>
{
    NPCController other;
    public VisitAction(NPCController context) : base(context)
    {
        other = context.GetComponent<NPCController>();
    }
    protected override void OnStart()
    {
        //애니메이션 변경
    }
    protected override NodeStatus OnUpdate()
    {

        if (other.targetDistance < other.workRange)
        {
            return NodeStatus.Success;
        }
        if (other.WorkTakenAway)
        {
            return NodeStatus.Failure;
        }
        Vector3 direction = (other.target.position - other.transform.position).normalized;
        other.transform.position += direction * other.speed * Time.deltaTime;
        return NodeStatus.Running;
    }
}
