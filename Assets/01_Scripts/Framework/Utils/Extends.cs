using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class Extends
{
    public static bool IsArrive(this NavMeshAgent agent, Transform target)
    {
        Vector2 agentPosition = agent.transform.position;
        Vector2 targetPosition = target.position;
        return Vector2.SqrMagnitude(agentPosition - targetPosition) <= Mathf.Sqrt(agent.stoppingDistance);
    }

    public static bool IsArrive(this NavMeshAgent agent, Vector2 target)
    {
        Vector2 agentPosition = agent.transform.position;
        return Vector2.SqrMagnitude(agentPosition - target) <= Mathf.Sqrt(agent.stoppingDistance);
    }

    public static InteractPermission WorkTypeToPermission(this WorkType workType)
    {
        switch (workType)
        {
            case WorkType.All:
                var permission = InteractPermission.None;
                permission |= InteractPermission.HallEmployee;
                permission |= InteractPermission.KitchenEmployee;
                permission |= InteractPermission.PaymentEmployee;
                return permission;
            case WorkType.Payment:
                return InteractPermission.PaymentEmployee;
            case WorkType.Hall:
                return InteractPermission.HallEmployee;
            case WorkType.Kitchen:
                return InteractPermission.KitchenEmployee;
        }

        return InteractPermission.None;
    }
    
}