using UnityEngine;

public abstract class ConditionNode<T> : BehaviorNode<T> where T : MonoBehaviour
{
    protected ConditionNode(T context) : base(context)
    {
    }

    protected abstract override NodeStatus OnUpdate();
}