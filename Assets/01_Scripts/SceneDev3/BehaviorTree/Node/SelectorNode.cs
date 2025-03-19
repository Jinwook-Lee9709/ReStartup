using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class SelectorrNode<T> : ComPositeNode<T> where T : MonoBehaviour
{
    private int currentChild; // 러닝중인 오브젝트를 알기 위해 

    public SelectorrNode(T context) : base(context)
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
            return NodeStatus.Failure;
        }
        while (currentChild < children.Count)
        {
            NodeStatus status = children[currentChild].Execute();

            if (status != NodeStatus.Failure)
            {
                return status;
            }

            ++currentChild;
        }
        return NodeStatus.Failure;
    }
    public override void Reset()
    {
        base.Reset();
        currentChild = 0;
    }
}
