public class IdleAction : ActionNode<NPCController>
{
    private readonly NPCController other;

    public IdleAction(NPCController context) : base(context)
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

        return NodeStatus.Running;
    }
}