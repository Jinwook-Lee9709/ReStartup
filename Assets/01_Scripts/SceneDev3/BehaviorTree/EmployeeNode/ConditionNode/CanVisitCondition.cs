using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanReturnCondition : ConditionNode<NPCController>
{
    public CanReturnCondition(NPCController context) : base(context)
    {
    }

    protected override NodeStatus OnUpdate()
    {
        return context.WorkTakenAway ? NodeStatus.Success : NodeStatus.Failure;
    }
}
