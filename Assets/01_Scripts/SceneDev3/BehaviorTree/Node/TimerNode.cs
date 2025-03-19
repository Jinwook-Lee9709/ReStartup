using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerNode<T> : DecoratorNode<T> where T : MonoBehaviour
{
    private float duration;
    private float currentTime;
    public TimerNode(T context) : base(context)
    {
    }
    protected override void OnStart()
    {
        base.OnStart();
        currentTime = Time.time;
    }
    public void SetDuration(float duration)
    {
        this.duration = duration;
    }
    protected override NodeStatus ProcessChild()
    {
        if (Time.time > duration + currentTime)
        {
            return NodeStatus.Running;
        }
        return NodeStatus.Failure;
    }
}
