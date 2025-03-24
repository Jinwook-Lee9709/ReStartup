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
        Debug.Log("OnMoveOrWork Ŭ���� ȣ���");
        if (work)
        {
            Debug.Log("Interection Ŭ���� ȣ���");
            //��Ŀ�� �־������.
        }
        else
        {
            agent.SetDestination(pos);
        }
    }
}
