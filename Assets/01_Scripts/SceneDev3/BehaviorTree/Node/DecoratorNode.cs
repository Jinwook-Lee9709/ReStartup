using UnityEngine;

public abstract class DecoratorNode<T> : BehaviorNode<T> where T : MonoBehaviour
{
    protected BehaviorNode<T> child; //DecoratorNode: �ڽ��� �ϳ��� ���� ���
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
        if (child != null) child.Reset();
    }

    protected override NodeStatus OnUpdate()
    {
        if (child == null) return NodeStatus.Failure;
        return ProcessChild();
    }

    protected abstract NodeStatus ProcessChild();
}