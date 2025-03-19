using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkingAction : ActionNode<NPCController>
{
    NPCController other;
    private float timer;
    
    public WorkingAction(NPCController context) : base(context)
    {
        other = context.GetComponent<NPCController>();
    }
    protected override void OnStart()
    {
        //애니메이션 변경
    }
    protected override NodeStatus OnUpdate()
    {

        return NodeStatus.Running;
    }
}
