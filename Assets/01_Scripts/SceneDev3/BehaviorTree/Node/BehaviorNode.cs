using UnityEngine;

public enum NodeStatus //���¹�ȯ�� ���� enum
{
    Success,
    Failure,
    Running // ������
}

public abstract class BehaviorNode<T> where T : MonoBehaviour //where ������ MonoBehaviour�� ��밡��
{
    protected readonly T context; // T == MonoBehaviour
    private bool isStarted;

    protected BehaviorNode(T context)
    {
        this.context = context;
    }

    public virtual void Reset()
    {
        isStarted = false;
    }

    protected virtual void OnStart()
    {
    }

    protected abstract NodeStatus OnUpdate();

    protected virtual void OnEnd()
    {
    }

    public NodeStatus Execute() // ���� ���� base���
    {
        if (!isStarted)
        {
            isStarted = true;
            OnStart();
        }

        var statues = OnUpdate();
        if (statues != NodeStatus.Running)
        {
            OnEnd();
            isStarted = false;
        }

        return statues;
    }
}