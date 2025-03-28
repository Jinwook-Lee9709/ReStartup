using UnityEngine;

public class BehaviorTree<T> where T : MonoBehaviour
{
    private readonly T context;
    private BehaviorNode<T> rootNode;

    public BehaviorTree(T context)
    {
        this.context = context;
    }

    public void SetRoot(BehaviorNode<T> node)
    {
        rootNode = node;
    }

    public NodeStatus UpDate()
    {
        if (rootNode == null) return NodeStatus.Failure;
        return rootNode.Execute();
    }

    public void Reset()
    {
        if (rootNode != null) rootNode.Reset();
    }
}