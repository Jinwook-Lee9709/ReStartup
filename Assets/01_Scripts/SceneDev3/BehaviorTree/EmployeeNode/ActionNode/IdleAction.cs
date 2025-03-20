using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleAction : ActionNode<NPCController>
{
    NPCController other;
    public IdleAction(NPCController context) : base(context)
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

        return NodeStatus.Running;
    }
}
