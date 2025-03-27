public class WorkingAction : ActionNode<NPCController>
{
    private NPCController other;
    private float timer;

    public WorkingAction(NPCController context) : base(context)
    {
        other = context.GetComponent<NPCController>();
    }

    protected override void OnStart()
    {
        //�ִϸ��̼� ����
    }

    protected override NodeStatus OnUpdate()
    {
        return NodeStatus.Running;
    }
}