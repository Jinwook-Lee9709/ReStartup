using UnityEngine;

public class SequenceNode<T> : ComPositeNode<T> where T : MonoBehaviour
{
    private int currentChild; // �������� ������Ʈ�� �˱� ���� 

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
        if (children.Count == 0) return NodeStatus.Success;
        while (currentChild < children.Count)
        {
            var status = children[currentChild].Execute();

            if (status != NodeStatus.Success) return status;

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