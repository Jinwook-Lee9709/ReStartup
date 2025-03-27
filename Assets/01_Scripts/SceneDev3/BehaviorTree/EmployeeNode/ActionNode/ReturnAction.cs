using UnityEngine;

public class ReturnAction : ActionNode<NPCController>
{
    private readonly NPCController other;

    public ReturnAction(NPCController context) : base(context)
    {
        other = context.GetComponent<NPCController>();
    }

    protected override void OnStart()
    {
        //�ִϸ��̼� ����
    }

    protected override NodeStatus OnUpdate()
    {
        if (other.CanVisit) return NodeStatus.Success;
        if (other.WorkTakenAway) return NodeStatus.Failure;
        var direction = (other.idlePos - other.transform.position).normalized;
        other.transform.position += direction * other.speed * Time.deltaTime;
        return NodeStatus.Running;
    }
}