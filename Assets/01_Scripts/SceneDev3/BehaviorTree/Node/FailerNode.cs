using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailerNode<T> : DecoratorNode<T> where T : MonoBehaviour
{
    public FailerNode(T context) : base(context)
    {

    }
    protected override NodeStatus ProcessChild()
    {
        NodeStatus status = child.Execute();
        if (status == NodeStatus.Running)
        {
            return NodeStatus.Running;
        }
        return NodeStatus.Failure;
    }
}
