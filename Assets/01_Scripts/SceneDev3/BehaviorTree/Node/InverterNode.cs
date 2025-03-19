using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverterNode<T> : DecoratorNode<T> where T : MonoBehaviour
{
    public InverterNode(T context) : base(context)
    {

    }
    protected override NodeStatus ProcessChild()
    {
        NodeStatus status = child.Execute();

        switch (status)
        {
            case NodeStatus.Success:
                return NodeStatus.Failure;
            case NodeStatus.Failure:
                return NodeStatus.Success;
        }
        return NodeStatus.Running;
    }
}
