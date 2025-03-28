using UnityEngine;

public class SelectorrNode<T> : ComPositeNode<T> where T : MonoBehaviour
{
    private int currentChild; // �������� ������Ʈ�� �˱� ���� 

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
        if (children.Count == 0) return NodeStatus.Failure;
        while (currentChild < children.Count)
        {
            var status = children[currentChild].Execute();

            if (status != NodeStatus.Failure) return status;

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