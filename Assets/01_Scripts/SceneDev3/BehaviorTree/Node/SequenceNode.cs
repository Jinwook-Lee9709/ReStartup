using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class SequenceNode<T> : ComPositeNode<T> where T : MonoBehaviour
{
    private int currentChild; // 러닝중인 오브젝트를 알기 위해 

    public SequenceNode(T context) : base(context)
    {

    }

    protected override void OnStart()
    {
        base.OnStart();
        currentChild = 0;

    }
    protected override NodeStatus OnUpdate()
    {
        if (children.Count == 0)
        {
            return NodeStatus.Success;
        }
        while (currentChild < children.Count)
        {
            NodeStatus status = children[currentChild].Execute();

            if (status != NodeStatus.Success)
            {
                return status;
            }

            ++currentChild;
        }
        return NodeStatus.Success;
    }
    public override void Reset()
    {
        base.Reset();
        currentChild = 0;
    }
}
