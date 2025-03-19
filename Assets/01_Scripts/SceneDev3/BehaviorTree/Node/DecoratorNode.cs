using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public abstract class DecoratorNode<T> : BehaviorNode<T> where T : MonoBehaviour
{
    protected BehaviorNode<T> child; //DecoratorNode: 자식을 하나만 갖는 노드
    protected int repeatCount;
    public DecoratorNode(T context) : base(context)
    {

    }

    public void SetChild(BehaviorNode<T> child)
    {
        this.child = child;
    }
    public override void Reset()
    {
        base.Reset();
        if (child != null) { child.Reset(); }
    }
    protected override NodeStatus OnUpdate()
    {
        if (child == null)
        {
            return NodeStatus.Failure;
        }
        return ProcessChild();
    }
    protected abstract NodeStatus ProcessChild();

}
