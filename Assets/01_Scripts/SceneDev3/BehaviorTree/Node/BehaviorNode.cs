using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeStatus //상태반환을 위한 enum
{
    Success,
    Failure,
    Running, // 실행중
}

public abstract class BehaviorNode<T> where T : MonoBehaviour //where 한정자 MonoBehaviour만 사용가능
{
    protected readonly T context; // T == MonoBehaviour
    private bool isStarted = false;

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

    public NodeStatus Execute() // 상태 관리 base노드
    {
        if (!isStarted)
        {
            isStarted = true;
            OnStart();
        }

        NodeStatus statues = OnUpdate();
        if (statues != NodeStatus.Running)
        {
            OnEnd();
            isStarted = false;
        }

        return statues;
    }


}
