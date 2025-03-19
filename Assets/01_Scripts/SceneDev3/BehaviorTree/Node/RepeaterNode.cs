using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeaterNode<T> : DecoratorNode<T> where T : MonoBehaviour
{
    public int runingCount;
    public RepeaterNode(T context) : base(context)
    {

    }
    public void SetRuningCoun(int count)
    {
        runingCount = count;
    }
    protected override NodeStatus ProcessChild()
    {
        NodeStatus status = child.Execute();
        if (status == NodeStatus.Running)
        {
            return NodeStatus.Running;
        }
        ++runingCount;
        
        if (runingCount == -1)
        {
            return NodeStatus.Success;
        }
        if (runingCount < repeatCount)
        {
            return NodeStatus.Success;
        }
        child.Reset();
        return NodeStatus.Running;
    }
}
