using System.Collections;
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

}
