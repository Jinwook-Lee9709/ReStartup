using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnAction : ActionNode<NPCController>
{
    NPCController other;
    public ReturnAction(NPCController context) : base(context)
    {
        other = context.GetComponent<NPCController>();
    }
    protected override void OnStart()
    {
        //애니메이션 변경
    }
    protected override NodeStatus OnUpdate()
    {

        if (other.CanVisit)
        {
            return NodeStatus.Success;
        }
        if (other.WorkTakenAway)
        {
            return NodeStatus.Failure;
        }
        Vector3 direction = (other.idlePos - other.transform.position).normalized;
        other.transform.position += direction * other.speed * Time.deltaTime;
        return NodeStatus.Running;
    }
}
