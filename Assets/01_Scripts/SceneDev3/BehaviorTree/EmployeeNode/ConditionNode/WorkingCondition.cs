using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkingCondition : ConditionNode<NPCController>
{
    public WorkingCondition(NPCController context) : base(context)
    {
    }

    protected override NodeStatus OnUpdate()
    {
        return context.targetDistance < context.workRange ? NodeStatus.Success : NodeStatus.Failure;
    }
}
