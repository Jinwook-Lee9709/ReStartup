using System.Collections;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    public void OnMoveOrWork(bool work, Vector2 pos)
    {
        Debug.Log("OnMoveOrWork 클릭함 호출됨");
        if (work)
        {
            Debug.Log("Interection 클릭함 호출됨");
            //워커에 넣어줘야함.
        }
        else
        {
            agent.SetDestination(pos);
        }
    }
}
