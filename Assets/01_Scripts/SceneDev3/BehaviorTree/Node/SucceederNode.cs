using UnityEngine;

public class SucceederNode<T> : DecoratorNode<T> where T : MonoBehaviour
{
    public SucceederNode(T context) : base(context)
    {
    }

    protected override NodeStatus ProcessChild()
    {
        var status = child.Execute();
        if (status == NodeStatus.Running) return NodeStatus.Running;
        return NodeStatus.Success;
    }
}