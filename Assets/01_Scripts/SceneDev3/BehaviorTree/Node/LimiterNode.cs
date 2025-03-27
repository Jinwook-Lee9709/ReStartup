using UnityEngine;

public class LimiterNode<T> : DecoratorNode<T> where T : MonoBehaviour
{
    private float currentTime;
    private float duration;

    public LimiterNode(T context) : base(context)
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
        if (Time.time < duration + currentTime) return NodeStatus.Running;
        var status = child.Execute();
        if (status == NodeStatus.Running) return NodeStatus.Running;
        return status;
    }
}