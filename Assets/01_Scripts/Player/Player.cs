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
        Debug.Log(work);
        if (work)
            Debug.Log("InteractableObjects Touch");
        else
            agent.SetDestination(pos);
    }
}