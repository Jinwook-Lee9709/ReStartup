using UnityEngine;

public class VisitAction : ActionNode<NPCController>
{
    private readonly NPCController other;

    public VisitAction(NPCController context) : base(context)
    {
        other = context.GetComponent<NPCController>();
    }

    protected override void OnStart()
    {
        //�ִϸ��̼� ����
    }

    protected override NodeStatus OnUpdate()
    {
        if (other.targetDistance < other.workRange) return NodeStatus.Success;
        if (other.WorkTakenAway) return NodeStatus.Failure;
        var direction = (other.target.position - other.transform.position).normalized;
        other.transform.position += direction * other.speed * Time.deltaTime;
        return NodeStatus.Running;
    }
}